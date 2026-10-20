# efcore-sargable-config

An agent skill for finding why an indexed EF Core/SQL Server lookup scans: varchar/nvarchar and MaxLength mapping mismatches, and non-sargable predicates (functions, arithmetic, conversions, leading wildcards, OR across columns, ISNULL/COALESCE). Rules ESC001-ESC012 are listed in [rules.md](.github/skills/efcore-sargable-config/references/rules.md); every finding uses a fixed output format and is labelled confirmed, candidate, other-cause or intentional-exception. It separates missing configuration from a proven `varchar`/`nvarchar` mismatch and checks the actual query plan before claiming an Index Scan.

## Use the skill

The workspace skill is in [SKILL.md](.github/skills/efcore-sargable-config/SKILL.md). Open this repository as a VS Code workspace and ask Copilot to review an EF Core SQL Server string mapping, or invoke `/efcore-sargable-config` in chat. You can also copy the skill directory into another repository's `.github/skills/` directory to use it there.

For a useful review, provide the EF Core version, the entity property and its configuration, the affected query, and the SQL Server column's type, length, collation, and index. If available, include executed parameter metadata and an actual execution plan. For example:

> Review `Customer.Email` against `dbo.Customers.Email varchar(256)` and its index. Check whether EF Core sends an `nvarchar` parameter, identify any column-side `CONVERT_IMPLICIT`, and suggest the smallest safe mapping change.

The agent checks the effective mapping, compares it with the schema, and labels findings **confirmed**, **candidate**, or **intentional exception**. Without database or plan access, it should report candidates and the checks needed to confirm them, not claim that a Scan occurred. The detailed [patterns](.github/skills/efcore-sargable-config/references/patterns.md), [detection steps](.github/skills/efcore-sargable-config/references/detection.md), and [fix options](.github/skills/efcore-sargable-config/references/fixes.md) are available when needed. The skill is guidance; it does not install an analyzer, change mappings automatically, or connect to SQL Server.

## Install for all your VS Code workspaces

To make the skill available to GitHub Copilot in any workspace you open on this machine, copy the **entire** skill directory to your personal Copilot skills folder: `~/.copilot/skills/efcore-sargable-config/` (`$HOME\.copilot\skills\efcore-sargable-config\` on Windows). This is a per-user installation, not a repository-wide or organization-wide deployment.

From the root of this repository, run in PowerShell:

```powershell
$source = Join-Path (Get-Location) '.github\skills\efcore-sargable-config'
$target = Join-Path $HOME '.copilot\skills\efcore-sargable-config'
if (Test-Path $target) { throw "Skill already installed at $target; review it before replacing it." }
New-Item -ItemType Directory -Force -Path (Split-Path $target) | Out-Null
Copy-Item -Path $source -Destination $target -Recurse
Test-Path (Join-Path $target 'SKILL.md')
```

The final command should return `True`. Keep the `references/` directory with `SKILL.md`; the skill links to those files. To update an existing installation, compare your installed copy with this repository first, then replace the directory deliberately rather than nesting a second copy inside it.

Open a different EF Core project in VS Code and start a new Copilot Agent chat. Type `/efcore-sargable-config` to select the skill, or ask: "Check our indexed EF Core string mappings against SQL Server varchar columns." If the skill is not listed, reload the VS Code window and confirm that `$HOME\.copilot\skills\efcore-sargable-config\SKILL.md` exists and that its `name` matches the folder. It may also be selected automatically for relevant prompts. This only makes the guidance available; the other project still needs its own code, schema, and (for confirmed plan impact) execution-plan evidence.

## Use the evals

[evals.json](.github/skills/efcore-sargable-config/evals/evals.json) contains three cases. Each `prompt` is an input to give the agent, and `expected_output` describes the behavior to check in its answer. There is no automated eval runner in this repository; run them manually:

1. Open a new chat with this workspace so the skill is available. Send one `prompt` from `evals.json` at a time, either by invoking `/efcore-sargable-config` or by asking naturally and confirming the skill was used.
2. Compare the response with that case's `expected_output`. For the first case, look for a targeted mapping fix **and** a plan/parameter confirmation step. For the second, check that missing database access limits the finding to a candidate. For the third, check that real Unicode data and the leading-wildcard `LIKE` prevent a blanket `varchar(256)` fix.
3. Record pass/fail and the missing or incorrect behavior for each case. If an answer claims a proven Scan without a plan, changes Unicode columns blindly, or applies a default length without schema evidence, mark it as a failure and refine the skill before rerunning the case.

[triggers.json](.github/skills/efcore-sargable-config/evals/triggers.json) lists example prompts that should or should not lead an agent to use this skill. Test them in fresh chats to check skill selection separately from answer quality. This file is an optional test fixture, not a registered trigger mechanism or a CI integration. The `description` in `SKILL.md` is the discovery surface for automatic skill selection.
## Test harness

`samples/` holds seeded BAD, GOOD and clean C# cases (one per rule), `schema.json` (simulated SQL Server columns) and `expected-findings.json` (scoring manifest generated from the `// BAD` / `// GOOD` / `// NEGATIVE` markers). Two test sets:

- **Unit (no Docker, ~3 s):** `dotnet test EfSargable.slnx --filter "Category!=Integration"` - effective EF mappings, generated SQL, model-vs-schema guardrail, manifest markers.
- **Integration (Docker required):** `dotnet test tests/EfSargable.IntegrationTests` - starts one `mcr.microsoft.com/mssql/server:2022-latest` container per run (Testcontainers.MsSql), builds the schema from `schema.json`, seeds 50k rows, runs every BAD/GOOD query with `SET STATISTICS XML/IO ON` and asserts the actual plans (CONVERT_IMPLICIT, PlanAffectingConvert, Seek vs Scan, logical reads, row counts, collation). `ObservationTests` writes `samples/plan-observations.md` with the raw result for every case.
- **Everything:** `dotnet test EfSargable.slnx` (Docker must be running).
