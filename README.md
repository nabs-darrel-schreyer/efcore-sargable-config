# efcore-sargable-config

An agent skill for GitHub Copilot that finds why your EF Core queries on SQL Server **scan an index instead of seeking it**, and tells you the smallest safe fix.

It covers two families of problems:

- **String mapping mismatches.** The column is `varchar`, but EF Core thinks it is `nvarchar`.
- **Non-sargable predicates.** The indexed column is wrapped in a function, arithmetic or a conversion.

Background reading (Darrel Schreyer, LinkedIn):

- [EF Core's silent index killer: varchar vs nvarchar mismatches](https://www.linkedin.com/pulse/ef-cores-silent-index-killer-varchar-vs-nvarchar-darrel-schreyer-adzlc/)
- [Non-sargable predicates: why your index isn't being used](https://www.linkedin.com/pulse/non-sargable-predicates-why-your-index-isnt-being-used-schreyer-oyezc/)

> [!IMPORTANT]
> **This is an experimental skill.** Treat its findings as leads to verify with an actual execution plan, not as verdicts, and expect the rules and output format to change between versions. Feedback and issues are welcome.
>
> **It is built for thoroughness, not efficiency.** It reads your mappings, queries and references in depth, so a full run takes time and uses AI credits. A Copilot CLI run against the sample in this repo took about 10 minutes and used 88 credits. Cost depends on your plan, the model and the size of your codebase, so point it at the DbContext and the slow queries you care about rather than a whole solution.

## The problem in 60 seconds

An index is sorted by the raw column value. SQL Server can only *seek* into it (jump straight to the matching rows) when the column is left **bare** on its side of the comparison. If the column is converted, wrapped in a function or used in arithmetic, SQL Server has to read the whole index and test every row. Nothing errors and tests on small databases pass. The query just gets slower as the table grows.

**Example 1: mapping mismatch.** `Customers.Email` is an indexed `varchar(256)` column, but the model doesn't say so:

```csharp
// Before: EF sends @email as nvarchar, so SQL Server converts the COLUMN: CONVERT_IMPLICIT(nvarchar, [Email]) -> Index Scan
e.Property(c => c.Email);

// After: the parameter is varchar(256), so no conversion -> Index Seek
e.Property(c => c.Email).IsUnicode(false).HasMaxLength(256);
```

**Example 2: non-sargable predicate.**

```csharp
// Before: WHERE DATEPART(year, [OrderDate]) = 2025 -> Index Scan
db.Sales.Where(s => s.OrderDate.Year == 2025);

// After: WHERE [OrderDate] >= '2025-01-01' AND [OrderDate] < '2026-01-01' -> Index Seek
db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate < new DateTime(2026, 1, 1));
```

Measured in this repo's SQL Server 2022 harness (50k rows): 219 vs 3 logical reads for example 1, and 268 vs 34 for example 2.

## What it detects

Full detail is in [rules.md](.github/skills/efcore-sargable-config/references/rules.md).

| Rule | One line |
|---|---|
| ESC001-unicode-mismatch | String with no Unicode setting against a `varchar` column: EF sends `nvarchar` and the column gets converted |
| ESC002-maxlength-without-unicode | `HasMaxLength` without `IsUnicode(false)` still sends `nvarchar(n)` |
| ESC003-length-mismatch | Length missing or wrong: `max` can't be indexed, inflates memory grants, and too short causes truncation errors |
| ESC004-blanket-convention | A global `AreUnicode(false)` convention breaks real Unicode columns (**wrong results**) and foreign keys |
| ESC005-function-on-column | `YEAR()`, `LEFT`/`SUBSTRING`, `UPPER`/`LOWER` on the column |
| ESC006-leading-wildcard | `LIKE '%son'` / `'%son%'` (`EndsWith`, `Contains`) |
| ESC007-reverse-unicode | `nvarchar` column with a `varchar` parameter: harmless, not flagged |
| ESC008-arithmetic-on-column | `DATEADD(..., Column)`, `Price * 1.1 > 110` |
| ESC009-implicit-conversion | Column converted to another type, e.g. `AccountNumber = 12345` on `varchar` |
| ESC010-or-across-columns | `FirstName = @n OR LastName = @n` |
| ESC011-isnull-on-column | `ISNULL`/`COALESCE(Status, 'New') = 'New'` |
| ESC012-closed-date-range | `<= '2025-12-31'` on a datetime misses rows after midnight (**correctness**) |

Every finding gets a label:

- **confirmed:** an actual plan proves it.
- **candidate:** static evidence only.
- **other-cause:** the slow query is not caused by string mapping.
- **intentional-exception:** a deliberate choice.

Each finding also gets a severity (high / medium / low / info) and a **correctness** flag for bugs that return wrong results.

## Quick start

```powershell
git clone https://github.com/nabs-darrel-schreyer/efcore-sargable-config.git
```

**Use it in your own repository.** Copy the whole skill folder, including `references/` and `evals/`:

```powershell
Copy-Item -Recurse efcore-sargable-config\.github\skills\efcore-sargable-config <your-repo>\.github\skills\
```

**Or install it for every workspace on your machine** (personal skills folder):

```powershell
$target = Join-Path $HOME '.copilot\skills\efcore-sargable-config'
if (Test-Path $target) { throw "Already installed at $target; review before replacing." }
New-Item -ItemType Directory -Force -Path (Split-Path $target) | Out-Null
Copy-Item -Recurse efcore-sargable-config\.github\skills\efcore-sargable-config $target
```

**VS Code (Copilot Chat, Agent mode).** Open your repo and type `/efcore-sargable-config`, or just ask. Copilot picks the skill up from its description. If it doesn't appear, reload the window and check that `SKILL.md` exists.

**Copilot CLI.** From your repo root:

```powershell
copilot -p "Use the efcore-sargable-config skill to review src/MyApp.Data against our SQL Server schema in db/schema.sql. Write sargable-report.md." --allow-all-tools --deny-tool=shell
```

Or run `copilot` interactively and ask the same thing.

**Example prompt:**

> Our `Customer.Email` property has no string configuration, but `dbo.Customers.Email` is `varchar(256)` with an index, and the login lookup is slow. Use efcore-sargable-config to find the problem, suggest the smallest fix, and tell me how to confirm it with the execution plan.

For better results, give the skill:

- your EF Core version
- the DbContext and entity configuration
- the slow query
- the real column types, lengths, collation and indexes
- an actual execution plan, if you have one

Without schema or plan access, the skill reports **candidates** and tells you exactly what to check.

## What the output looks like

The skill writes a Markdown report (default `sargable-report.md`) using [report-template.md](.github/skills/efcore-sargable-config/references/report-template.md), and summarises it in chat. A real report generated by the Copilot CLI against this repo's samples is in [samples/example-report.md](samples/example-report.md). Excerpt (from that run):

```markdown
## Executive summary
| Label | Count |  | Severity | Count |
|---|---|---|---|---|
| confirmed | 0 |  | high | 3 |
| candidate | 4 |  | medium | 12 |
**Correctness issues (wrong results or failures): 3** - ESC004 Translation.Text, ESC009 Convert.ToInt32(AccountNumber), ESC012 ClosedEndDate.
**Top 3 fixes**
1. ESC004 Translation.Text - drop the blanket varchar convention; map Text as nvarchar(400)
...
```

Each finding also has a machine-readable block, so runs can be scored automatically:

```
[ESC001-unicode-mismatch] samples/EfSargable.Sample/Bad/BadDbContext.cs:18  Customer.Email
Label: candidate
Severity: medium   Correctness: no
Effective mapping: nvarchar(450)   Column: varchar(256), indexed
Evidence: static only - not measured
Fix: .IsUnicode(false).HasMaxLength(256)
Validate: ToQueryString() shows varchar(256); actual plan shows Index Seek, no CONVERT_IMPLICIT
```

## Test harness: proving the skill works

`samples/` holds deliberately seeded C# cases:

| Folder or file | Contents |
|---|---|
| `Bad/` | One case per rule, each marked `// BAD: <rule-id>` |
| `Good/` | The fixed versions |
| clean cases | Marked `// NEGATIVE`; these must not be flagged |
| `schema.json` | A stand-in for the real SQL Server columns |
| `expected-findings.json` | The scoring manifest, built from the markers |

| Test set | Needs | Command | Time |
|---|---|---|---|
| Unit: effective EF mappings, generated SQL, model-vs-schema guardrail, manifest markers | .NET 10 SDK | `dotnet test EfSargable.slnx --filter "Category!=Integration"` | ~3 s |
| Integration: real execution plans | Docker running | `dotnet test tests/EfSargable.IntegrationTests` | ~15 s with the image cached |
| Everything | Docker running | `dotnet test EfSargable.slnx` | ~15-20 s |

The integration tests use Testcontainers.MsSql with one shared `mcr.microsoft.com/mssql/server:2022-latest` container per run. For each run they:

1. Create the schema from `schema.json` with real `varchar`/`nvarchar` columns and indexes.
2. Seed 50,000 rows.
3. Execute every BAD and GOOD query exactly as EF generates it, with `SET STATISTICS XML ON` and `SET STATISTICS IO ON`.
4. Assert the actual plan: `CONVERT_IMPLICIT`, `PlanAffectingConvert`, Seek vs Scan, logical reads and row counts.

`ObservationTests` writes the raw results to `samples/plan-observations.md`.

### Verified on SQL Server 2022

Server collation was `SQL_Latin1_General_CP1_CI_AS`.

**Proven as the articles describe:**

- **ESC001/002:** `CONVERT_IMPLICIT` on the column, a "Seek Plan" warning and an Index Scan. The fix seeks.
- **Predicate rules:** ESC005, ESC006 (literal patterns), ESC008, ESC009 and ESC011 scan, and the rewrites seek with the same rows and far fewer reads.
- **ESC012** returns 5,825 rows instead of 5,840.
- **`CAST(dt AS date)`** still seeks (a dynamic seek).
- **Collation:** a `SQL_` collation column scans; a `Latin1_General_CI_AS` column does a dynamic seek (`GetRangeThroughConvert`).
- **ESC003 / ESC007:** a length mismatch alone, or the reverse Unicode case, still seeks with no column conversion.

**Where reality was more nuanced than the articles:**

- **OR across columns (ESC010):** the optimiser combined two seeks itself (index union), with the same reads as the `UNION` rewrite. It's flagged low and should be confirmed with a plan before rewriting.
- **A seek isn't proof of sargability:** EF parameterises `EndsWith(term)` as `LIKE @p`. The plan shows an *Index Seek* that covers the whole index, with the same reads as a scan. Judge by seek ranges and reads, not by the operator name.
- **Windows collation:** no "Cardinality Estimate" warning appeared for a simple equality.
- **ESC004 is a correctness bug as well as a performance one.** A varchar parameter against real Unicode text returned 0 rows instead of 1.

## Limits and scope

- **SQL Server only.** PostgreSQL, Oracle and MySQL expression indexes are mentioned but not analysed.
- **Not general EF Core performance.** It doesn't cover N+1 queries, tracking or projections.
- **It never changes your database.** Column type changes are always treated as reviewed migrations, and it never flips real Unicode data to `varchar`.
- **Static findings are only candidates.** Only an actual plan (or executed parameters) makes a finding **confirmed**. Not every scan is a bug; low-selectivity queries can scan legitimately.
- **It's guidance, not an analyzer.** For CI, use a model-metadata test like `MappingGuardrail` in `samples/EfSargable.Sample/Shared`.

## Evals

[evals.json](.github/skills/efcore-sargable-config/evals/evals.json) has 7 prompt/expected-behaviour cases. [triggers.json](.github/skills/efcore-sargable-config/evals/triggers.json) lists prompts that should and shouldn't select the skill. Run them in fresh chats and record pass/fail. To score a run against the sample, compare its machine-readable blocks with `samples/expected-findings.json`.

## Contributing

Issues and PRs are welcome. When you add or change a rule:

1. Add or update its row in `references/rules.md` with a stable `ESCnnn` id.
2. Add a BAD case, a GOOD fix and a clean case under `samples/`, with `// BAD: <id>` / `// GOOD` / `// NEGATIVE` markers. Then regenerate `expected-findings.json` (line numbers are checked by `ManifestTests`).
3. Add unit assertions and, where plan behaviour matters, an integration test that asserts what SQL Server **actually** does. If it contradicts the guidance, update the guidance rather than the test.
4. Add an eval if behaviour changes, and add an entry to `CHANGELOG.md`.
5. Make sure `dotnet test EfSargable.slnx` passes with Docker running.
