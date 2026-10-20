# Copilot CLI Session

> [!NOTE]
> - **Session ID:** `9d8f8729-26cd-4568-b797-d9a6545a2dd7`  
> - **Started:** 20/10/2026, 6:53:05 pm  
> - **Duration:** 5m 15s  
> - **Exported:** 20/10/2026, 6:58:20 pm  

---

<sub>26s</sub>

### User

Use the efcore-sargable-config skill (.github/skills/efcore-sargable-config). Apply it to the C# sources under samples/EfSargable.Sample (the DbContexts in Bad/ and Good/, the query classes, and schema.json, which describes the existing SQL Server columns). Do NOT open samples/expected-findings.json or anything under tests/, and ignore the // BAD, // GOOD and // NEGATIVE comments: judge only from the code and schema. There is no SQL Server access. Report every finding in the skill's fixed output format with rule id, file:line and label, then the Summary line. Do not edit any files.

---

<sub>33s</sub>

### `skill`

**efcore-sargable-config**

<details>
<summary>Arguments</summary>

```json
{
  "skill": "efcore-sargable-config"
}
```

</details>

```
Skill "efcore-sargable-config" loaded successfully. Follow the instructions in the skill context.
```


---

<sub>47s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\rules.md.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\rules.md

<details>
<summary>22 lines</summary>

```
# Rule catalogue

Stable ids. Sources: "EF Core's silent index killer: varchar vs nvarchar mismatches" (A1) and "Non-sargable predicates: why your index isn't being used" (A2), both by Darrel Schreyer.

## Mapping rules (fix the model)

| Id | Detect | Fix | Source |
|---|---|---|---|
| ESC001-unicode-mismatch | String property with no Unicode setting compared against a `varchar` column. EF sends `nvarchar(4000)` / `N'...'`; SQL Server converts the **column** (data type precedence): `CONVERT_IMPLICIT(nvarchar(256), [Email], 0)`. Default mapping is `nvarchar(max)`, or `nvarchar(450)` when the property is a key **or indexed**. | `[Unicode(false)]` + `[MaxLength(n)]` or `.IsUnicode(false).HasMaxLength(n)`; `HasColumnType("varchar(n)")` / `[Column(TypeName=...)]` also works but ties the model to SQL Server type names. | A1 |
| ESC002-maxlength-without-unicode | `HasMaxLength`/`[MaxLength]` set but Unicode not set on a `varchar` column -> still `nvarchar(n)`. | Add `IsUnicode(false)`. | A1, fixes.md |
| ESC003-length-mismatch | Unicode correct but length missing or wrong: `max` cannot be an index key; inflated memory grants; too short -> production truncation error (EF does not validate MaxLength before sending). Not a column-side conversion by itself. | `HasMaxLength(<real length>)`. "No string property without an explicit length." | A1 |
| ESC004-blanket-convention | `ConfigureConventions` `AreUnicode(false)`/`HaveMaxLength` applied to every string, including real Unicode columns (names, addresses, free text) and foreign keys. Pre-convention configuration **overrides data annotations** (`[MaxLength(1000)]` will not beat it). | Use the convention only when most columns are varchar; override exceptions with Fluent API in `OnModelCreating`; check FK lengths match their principal keys. | A1 |
| ESC007-reverse-unicode | `nvarchar` column with a `varchar` parameter. The parameter is converted, not the column: harmless. | None. Label `intentional-exception` / do not flag. | A1 |

## Predicate rules (label `other-cause`; rewrite the query)

| Id | Detect (LINQ -> SQL) | Fix | Source |
|---|---|---|---|
| ESC005-function-on-column | `YEAR(OrderDate) = 2025` (`o.OrderDate.Year == 2025`), `LEFT(Code,3)` / `SUBSTRING` (`Substring(0,3)`), `UPPER/LOWER(Email)` (`ToUpper()/ToLower()`). On a case-insensitive collation UPPER achieves nothing. Exception: `CAST(dt AS date) = @d` (`.Date ==`) can still seek - don't rely on it. | Half-open range `>= '20250101' AND < '20260101'`; `LIKE 'ABC%'` (`StartsWith`); drop UPPER/LOWER; or index the expression (computed column). | A2 |
| ESC006-leading-wildcard | `LIKE '%son'` / `'%son%'` (`EndsWith`, `Contains`). | Prefix `LIKE 'Smi%'`; full-text search; indexed computed column `REVERSE(LastName)` searched with `'nos%'`. | A2 |
| ESC008-arithmetic-on-column | `DATEADD(day, 30, CreatedAt) > SYSDATETIME()` (`o.CreatedAt.AddDays(30) > now`), `Price * 1.1 > 110`. | Move maths to the constant: `CreatedAt > DATEADD(day, -30, ...)`, `Price > 100`. Check equivalence (integer division, rounding, month arithmetic). | A2 |
| ESC009-implicit-conversion | Column converted to another type: `AccountNumber = 12345` on `varchar(20)` (LINQ `Convert.ToInt32(a.AccountNumber) == 12345` -> `CAST(... AS int)`). Can also fail on non-numeric rows. | Compare with the column's type: `AccountNumber = '12345'`. | A2 |
| ESC010-or-across-columns | `FirstName = @Name OR LastName = @Name`. | `UNION` of two single-column queries (`UNION`, not `UNION ALL`, if a row can match both) - LINQ `.Union(...)`. | A2 |
| ESC011-isnull-on-column | `ISNULL(Status,'New') = 'New'` / `COALESCE` (`(s.Status ?? "New") == "New"`). | `Status = 'New' OR Status IS NULL`; better, ask whether the column should allow NULLs. | A2 |
| ESC012-closed-date-range | `BETWEEN '20250101' AND '20251231'` (`<= new DateTime(2025,12,31)`) on a column with a time part: sargable but **silently misses rows after midnight**. Correctness, not speed. | Half-open range `< '20260101'`. | A2 |

## Out of scope / not statically testable

Collation behaviour (SQL_ vs Windows collation, dynamic seek via GetRangeThroughConvert), parameter sniffing, memory grants, plan-cache/Query Store evidence, PostgreSQL/Oracle/MySQL expression indexes (`lower(email)`, `created_at::date`). Report them as checks to run, not findings.
```

</details>


---

<sub>45s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\detection.md.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\detection.md

<details>
<summary>45 lines</summary>

````
# Detection and Confirmation

## Static inventory

1. Find `DbContext`, `DbSet`, `IEntityTypeConfiguration`, `OnModelCreating`, `ConfigureConventions`, property attributes, and converters. Include shadow and owned types if the query uses them.
2. Resolve the effective mapping per string property (`GetColumnType()`, `GetMaxLength()`, `IsUnicode()`). Defaults: `nvarchar(max)`, or `nvarchar(450)` for keys and indexed columns. Conventions override annotations.
3. List what EF thinks every string column is and diff it with `INFORMATION_SCHEMA.COLUMNS` / `sys.columns`:

```csharp
foreach (var entity in db.Model.GetEntityTypes())
foreach (var p in entity.GetProperties().Where(p => p.ClrType == typeof(string)))
    Console.WriteLine($"{entity.GetTableName()}.{p.GetColumnName()}: {p.GetColumnType()}");
```

Prefer this as a model-metadata integration test in CI over a syntax-only Roslyn analyzer (which cannot prove the effective mapping across files and conventions).

4. Scan LINQ predicates on indexed columns for the ESC005/006/008-012 shapes in [rules](./rules.md): a method call or arithmetic on the column member (`.Year`, `.ToLower()`, `.Substring`, `.AddDays`, `* 1.1`, `Convert.To*`), `EndsWith`/`Contains`, `||` across different columns, `??` on a column, `<=` an end-of-day date literal.

## Parameter checks

- `ToQueryString()`: `DECLARE @email nvarchar(4000) = N'...'` against a varchar column is the tell. Parameter names vary by EF version; look at the declared type and the `N'` prefix.
- Dev logging: `.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging()` (dev only). Non-Unicode parameters log as `DbType = AnsiString`.

## Schema comparison

```sql
SELECT c.name AS column_name, t.name AS data_type, c.max_length,
       c.collation_name, i.name AS index_name, ic.key_ordinal
FROM sys.columns AS c
JOIN sys.types AS t ON c.user_type_id = t.user_type_id
LEFT JOIN sys.index_columns AS ic
  ON ic.object_id = c.object_id AND ic.column_id = c.column_id AND ic.key_ordinal > 0
LEFT JOIN sys.indexes AS i
  ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE c.object_id = OBJECT_ID(N'dbo.Customers') AND t.name IN (N'varchar', N'nvarchar')
ORDER BY c.column_id, ic.key_ordinal;
```

`max_length` is bytes; halve it for `nvarchar(n)` (`-1` = max). Check `SELECT SERVERPROPERTY('Collation')` too.

## Plan checks (confirm)

- Actual plan: warning on the root operator with `CONVERT_IMPLICIT` / "Type conversion in expression"; Index Scan where a Seek was expected; filter under **Predicate** instead of **Seek Predicates**.
- `SET STATISTICS IO ON`: far more logical reads than the rows returned justify.
- Plan cache (off-peak; shredding XML is expensive). Remove the ConvertIssue filter to also catch "Cardinality Estimate" warnings (typical on Windows collations):

```sql
WITH XMLNAMESPACES (DEFAULT 'http://schemas.microsoft.com/sqlserver/2004/07/showplan')
SELECT TOP (50) qs.execution_count, qs.total_logical_reads,
       qs.total_worker_time / 1000 AS total_cpu_ms,
       SUBSTRING(st.text, 1, 400) AS query_text, qp.query_plan
FROM sys.dm_exec_query_stats AS qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS st
CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) AS qp
WHERE qp.query_plan.exist('//Warnings/PlanAffectingConvert[@ConvertIssue="Seek Plan"]') = 1
ORDER BY qs.total_logical_reads DESC;
```

- Query Store sorted by reads finds the queries that matter. Static evidence only -> `candidate`; never claim a scan without a plan.
````

</details>


---

<sub>43s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\patterns.md.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\patterns.md

<details>
<summary>13 lines</summary>

```
# SQL Server Plan Patterns

## Mapping mismatch (ESC001)

- Indexed `varchar(n)` column, `nvarchar` parameter, plan predicate `CONVERT_IMPLICIT(nvarchar(...), [t].[Email], 0) = @email`. nvarchar has higher data type precedence, so the column is converted.
- **SQL collations** (`SQL_Latin1_General_CP1_CI_AS`, default for US English installs and Azure SQL Database): generally no seek - you get the scan.
- **Windows collations** (`Latin1_General_CI_AS`): often still a "dynamic seek" (`GetRangeThroughConvert`) with a residual predicate - better than a scan, still extra reads and worse estimates.
- **Reverse case** (ESC007): `nvarchar` column, `varchar` parameter - the parameter is converted; harmless.
- Bad estimates compound parameter sniffing. Cost grows with the table, not the result set, so it passes tests on small databases.

## Length (ESC003)

`MaxLength` is characters; `sys.columns.max_length` is bytes. Parameters are declared at the provider cap (`nvarchar(4000)` / `varchar(8000)`) on purpose to avoid plan cache fragmentation - not a disaster on its own. A length mismatch alone does not cause a column-side conversion.

## Non-sargable shapes (other-cause)

`WHERE YEAR(OrderDate) = 2025`, `LEFT(Code,3) = 'ABC'`, `UPPER(Email) = @Email`, `DATEADD(day,30,CreatedAt) > SYSDATETIME()`, `Price * 1.1 > 110`, `AccountNumber = 12345`, `LIKE '%son'`, `FirstName = @Name OR LastName = @Name`, `ISNULL(Status,'New') = 'New'`. Typically Index Scan with a residual predicate. Exception: `CAST(dt AS date) = @d` can seek via an optimiser rewrite (estimates may be off).

## Not every scan is a bug

A scan may be optimal for low selectivity, a small table, or stale statistics. Investigate when a **selective** query scans.
```

</details>


---

<sub>48s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\fixes.md.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\.github\skills\efcore-sargable-config\references\fixes.md

<details>
<summary>36 lines</summary>

````
# Fixes

## Mapping (ESC001-ESC004)

Tell EF Core the truth about each existing column. Attributes:

```csharp
[Unicode(false)]
[MaxLength(256)]
public string Email { get; set; } = null!;
```

Fluent API:

```csharp
modelBuilder.Entity<Customer>().Property(c => c.Email).IsUnicode(false).HasMaxLength(256);
```

`HasColumnType("varchar(256)")` / `[Column(TypeName = "varchar(256)")]` pins the exact type but ties the model to SQL Server names; prefer Unicode + length. `HasMaxLength` alone does not set `IsUnicode(false)` (ESC002). `[Unicode]` needs EF Core 6+. `dotnet ef dbcontext scaffold` already emits `IsUnicode(false)` for varchar - drift comes from hand-written or later-changed mappings.

Pre-convention (EF Core 6+) only when most columns are varchar:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder builder)
{
    builder.Properties<string>().AreUnicode(false).HaveMaxLength(256);
}
```

It overrides data annotations and applies to every string including FKs; override exceptions with Fluent API in `OnModelCreating` (ESC004). Never flip genuinely Unicode data (names, addresses, free text) to varchar; consider a UTF-8 collation on SQL Server 2019+. If the database is the source of truth, only parameter types change; if migrations own the schema, the same change is an `ALTER COLUMN` - review, test and schedule it. Always set a max length; `nvarchar(max)` should be a decision.

## Predicates (ESC005-ESC012)

Keep the column bare; move work to the constant/parameter; match parameter types to the column; when you need an expression, index it (SQL Server indexed computed column - reference it directly).

| Rule | LINQ before | LINQ after |
|---|---|---|
| ESC005 | `o.OrderDate.Year == 2025` | `o.OrderDate >= new DateTime(2025,1,1) && o.OrderDate < new DateTime(2026,1,1)` |
| ESC005 | `c.Code.Substring(0,3) == "ABC"` | `c.Code.StartsWith("ABC")` |
| ESC005 | `c.Email.ToUpper() == email` | `c.Email == email` (CI collation) |
| ESC006 | `c.LastName.EndsWith("son")` | prefix search / full-text / `REVERSE` computed column with `StartsWith("nos")` |
| ESC008 | `o.CreatedAt.AddDays(30) > now` | `o.CreatedAt > now.AddDays(-30)` |
| ESC008 | `p.Price * 1.1m > 110` | `p.Price > 100` |
| ESC009 | `Convert.ToInt32(a.AccountNumber) == 12345` | `a.AccountNumber == "12345"` |
| ESC010 | `c.FirstName == n \|\| c.LastName == n` | `q.Where(c => c.FirstName == n).Union(q.Where(c => c.LastName == n))` |
| ESC011 | `(s.Status ?? "New") == "New"` | `s.Status == "New" \|\| s.Status == null` |
| ESC012 | `o.OrderDate <= new DateTime(2025,12,31)` | `o.OrderDate < new DateTime(2026,1,1)` |

Check rewrites are equivalent at the edges (integer division, rounding, month arithmetic). Verify the plan on the same representative query after the fix.
````

</details>


---

<sub>41s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample

<details>
<summary>7 lines</summary>

```
Bad
EfSargable.Sample.csproj
Good
Shared
bin
obj
schema.json
```

</details>


---

<sub>56s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad

```
BadConventionDbContext.cs
BadDbContext.cs
BadPredicates.cs
BadQueries.cs
```


---

<sub>57s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good

```
GoodDbContext.cs
GoodPredicates.cs
GoodQueries.cs
PredicateDbContext.cs
```


---

<sub>1m 1s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared

```
ContextFactory.cs
Entities.cs
MappingGuardrail.cs
PredicateEntities.cs
```


---

<sub>59s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\schema.json.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\schema.json

<details>
<summary>13 lines</summary>

```
{
  "_comment": "Simulated EXISTING SQL Server schema (what sys.columns would return). Lengths in characters. This is the 'intended column' the skill compares against.",
  "columns": [
    { "table": "dbo.Customers",    "column": "Email",     "type": "varchar",  "length": 256,  "indexed": true },
    { "table": "dbo.Customers",    "column": "Notes",     "type": "nvarchar", "length": -1,   "indexed": false, "exception": "intentional long Unicode text, unindexed" },
    { "table": "dbo.Products",     "column": "Sku",       "type": "varchar",  "length": 32,   "indexed": true },
    { "table": "dbo.Products",     "column": "Code",      "type": "varchar",  "length": 16,   "indexed": true },
    { "table": "dbo.Orders",       "column": "Reference", "type": "varchar",  "length": 20,   "indexed": true },
    { "table": "dbo.Translations", "column": "Key",       "type": "varchar",  "length": 256,  "indexed": true },
    { "table": "dbo.Translations", "column": "Text",      "type": "nvarchar", "length": 400,  "indexed": true },
    { "table": "dbo.Accounts",     "column": "AccountNumber", "type": "varchar", "length": 20, "indexed": true }
  ]
}
```

</details>


---

<sub>1m 12s</sub>

### Copilot

Now reading all the relevant C# source files in parallel.

---

<sub>1m 16s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadDbContext.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadDbContext.cs

<details>
<summary>33 lines</summary>

```
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Seeded BAD mappings. Each finding is marked with  // BAD: <rule-id>
public class BadDbContext(DbContextOptions<BadDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.Property(c => c.Email); // BAD: ESC001-unicode-mismatch (no config + index -> nvarchar(450); column is varchar(256) indexed)
            e.HasIndex(c => c.Email);
            e.Property(c => c.Notes); // NEGATIVE: intentional-exception (nvarchar(max) matches schema, unindexed long text)
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(p => p.Sku).HasMaxLength(32); // BAD: ESC002-maxlength-without-unicode (HasMaxLength alone -> nvarchar(32); column varchar(32))
            e.HasIndex(p => p.Sku);
            e.Property(p => p.Code).HasColumnType("varchar(16)"); // NEGATIVE: store type alone supplies varchar(16) - must NOT be flagged
            e.HasIndex(p => p.Code);
        });

        b.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(o => o.Reference).IsUnicode(false); // BAD: ESC003-length-mismatch (varchar(max) vs varchar(20); NOT a Unicode conversion)
        });
    }
}
```

</details>


---

<sub>1m 14s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadConventionDbContext.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadConventionDbContext.cs

<details>
<summary>22 lines</summary>

```
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// The "blanket varchar(256)" global fix from evals.json case 3.
public class BadConventionDbContext(DbContextOptions<BadConventionDbContext> options) : DbContext(options)
{
    public DbSet<Translation> Translations => Set<Translation>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<string>().AreUnicode(false).HaveMaxLength(256); // BAD: ESC004-blanket-convention (forces real Unicode Text nvarchar(400) to varchar(256))
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Translation>(e =>
        {
            e.ToTable("Translations");
            e.Property(t => t.Key); // NEGATIVE: convention gives varchar(256) which matches schema - must NOT be flagged
            e.HasIndex(t => t.Key);
            e.HasIndex(t => t.Text);
        });
    }
}
```

</details>


---

<sub>1m 22s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadPredicates.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadPredicates.cs

<details>
<summary>28 lines</summary>

```
using EfSargable.Sample.Good;
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Non-sargable predicates. Mapping is correct, so every finding here is label "other-cause".
public static class BadPredicates
{
    public static IQueryable<Sale> YearOnColumn(PredicateDbContext db) =>
        db.Sales.Where(s => s.OrderDate.Year == 2025); // BAD: ESC005-function-on-column (DATEPART(year, OrderDate) = 2025)

    public static IQueryable<Person> UpperOnColumn(PredicateDbContext db, string name) =>
        db.People.Where(p => p.LastName.ToUpper() == name); // BAD: ESC005-function-on-column (UPPER(LastName); pointless on a CI collation)

    public static IQueryable<Person> SuffixSearch(PredicateDbContext db) =>
        db.People.Where(p => p.LastName.EndsWith("son")); // BAD: ESC006-leading-wildcard (LIKE '%son')

    public static IQueryable<Person> ContainsSearch(PredicateDbContext db) =>
        db.People.Where(p => p.LastName.Contains("son")); // BAD: ESC006-leading-wildcard (LIKE '%son%')

    public static IQueryable<Sale> DateAddOnColumn(PredicateDbContext db, DateTime now) =>
        db.Sales.Where(s => s.CreatedAt.AddDays(30) > now); // BAD: ESC008-arithmetic-on-column (DATEADD(day, 30, CreatedAt))

    public static IQueryable<Sale> MathsOnColumn(PredicateDbContext db) =>
        db.Sales.Where(s => s.Price * 1.1m > 110m); // BAD: ESC008-arithmetic-on-column (Price * 1.1 > 110)

    public static IQueryable<Account> NumberVsVarchar(PredicateDbContext db) =>
        db.Accounts.Where(a => Convert.ToInt32(a.AccountNumber) == 12345); // BAD: ESC009-implicit-conversion (varchar column converted to int)

    public static IQueryable<Person> OrAcrossColumns(PredicateDbContext db, string name) =>
        db.People.Where(p => p.FirstName == name || p.LastName == name); // BAD: ESC010-or-across-columns

    public static IQueryable<Sale> CoalesceOnColumn(PredicateDbContext db) =>
        db.Sales.Where(s => (s.Status ?? "New") == "New"); // BAD: ESC011-isnull-on-column (COALESCE(Status, 'New'))

    public static IQueryable<Sale> ClosedEndDate(PredicateDbContext db) =>
        db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate <= new DateTime(2025, 12, 31)); // BAD: ESC012-closed-date-range (misses rows after midnight 31 Dec)
}
```

</details>


---

<sub>1m 18s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadQueries.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadQueries.cs

<details>
<summary>14 lines</summary>

```
using EfSargable.Sample.Good;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Non-mapping causes of scans. These run against the FIXED mapping on purpose:
// the skill must report them as "other reason" and must NOT propose a mapping change.
public static class BadQueries
{
    public static IQueryable<Shared.Customer> LowerOnColumn(GoodDbContext db, string email) =>
        db.Customers.Where(c => c.Email.ToLower() == email); // BAD: ESC005-function-on-column (LOWER([c].[Email]) - mapping fix cannot help)

    public static IQueryable<Shared.Product> PrefixViaSubstring(GoodDbContext db, string prefix) =>
        db.Products.Where(p => p.Sku.Substring(0, 3) == prefix); // BAD: ESC005-function-on-column (SUBSTRING on column)

    public static IQueryable<Shared.Customer> LeadingWildcard(GoodDbContext db, string term) =>
        db.Customers.Where(c => c.Email.EndsWith(term)); // BAD: ESC006-leading-wildcard (LIKE '%term' - mapping fix cannot make it a seek)
}
```

</details>


---

<sub>1m 25s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodDbContext.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodDbContext.cs

<details>
<summary>43 lines</summary>

```
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Good;

// FIXED counterparts: every mapping matches schema.json. Nothing here should be flagged.
public class GoodDbContext(DbContextOptions<GoodDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Translation> Translations => Set<Translation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.Property(c => c.Email).IsUnicode(false).HasMaxLength(256); // GOOD: fixes ESC001
            e.HasIndex(c => c.Email);
            e.Property(c => c.Notes); // NEGATIVE: intentional-exception
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(p => p.Sku).IsUnicode(false).HasMaxLength(32); // GOOD: fixes ESC002
            e.HasIndex(p => p.Sku);
            e.Property(p => p.Code).HasColumnType("varchar(16)"); // NEGATIVE: store type alone is sufficient
            e.HasIndex(p => p.Code);
        });

        b.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(o => o.Reference).IsUnicode(false).HasMaxLength(20); // GOOD: fixes ESC003
            e.HasIndex(o => o.Reference);
        });

        b.Entity<Translation>(e =>
        {
            e.ToTable("Translations");
            e.Property(t => t.Key).IsUnicode(false).HasMaxLength(256); // GOOD: scoped mapping instead of ESC004 blanket convention
            e.HasIndex(t => t.Key);
            e.Property(t => t.Text).HasMaxLength(400); // NEGATIVE: real Unicode nvarchar(400) - must stay Unicode
            e.HasIndex(t => t.Text);
        });
    }
}
```

</details>


---

<sub>1m 20s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodPredicates.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodPredicates.cs

<details>
<summary>31 lines</summary>

```
using EfSargable.Sample.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EfSargable.Sample.Good;

public static class GoodPredicates
{
    public static IQueryable<Sale> YearAsHalfOpenRange(PredicateDbContext db) =>
        db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate < new DateTime(2026, 1, 1)); // GOOD: fixes ESC005 and ESC012

    public static IQueryable<Person> BareColumnEquals(PredicateDbContext db, string name) =>
        db.People.Where(p => p.LastName == name); // GOOD: fixes ESC005 (CI collation already ignores case)

    public static IQueryable<Person> SuffixViaReversedColumn(PredicateDbContext db) =>
        db.People.Where(p => p.ReversedLastName.StartsWith("nos")); // GOOD: fixes ESC006 (LIKE 'nos%' on indexed REVERSE computed column)

    public static IQueryable<Person> PrefixSearch(PredicateDbContext db) =>
        db.People.Where(p => p.LastName.StartsWith("Smi")); // NEGATIVE: LIKE 'Smi%' is sargable

    public static IQueryable<Sale> DateAddOnConstant(PredicateDbContext db, DateTime now) =>
        db.Sales.Where(s => s.CreatedAt > now.AddDays(-30)); // GOOD: fixes ESC008

    public static IQueryable<Sale> MathsOnConstant(PredicateDbContext db) =>
        db.Sales.Where(s => s.Price > 100m); // GOOD: fixes ESC008

    public static IQueryable<Account> StringVsVarchar(PredicateDbContext db) =>
        db.Accounts.Where(a => a.AccountNumber == "12345"); // GOOD: fixes ESC009

    public static IQueryable<Person> UnionPerColumn(PredicateDbContext db, string name) =>
        db.People.Where(p => p.FirstName == name).Union(db.People.Where(p => p.LastName == name)); // GOOD: fixes ESC010

    public static IQueryable<Sale> OrIsNull(PredicateDbContext db) =>
        db.Sales.Where(s => s.Status == "New" || s.Status == null); // GOOD: fixes ESC011

    public static IQueryable<Sale> CastToDate(PredicateDbContext db, DateTime d) =>
        db.Sales.Where(s => s.OrderDate.Date == d); // NEGATIVE: CAST(dt AS date) can still seek (optimiser rewrite) - mention, don't flag

    public static IQueryable<Translation> VarcharParamVsNvarcharColumn(PredicateDbContext db) =>
        db.Translations.FromSqlRaw("SELECT * FROM Translations WHERE Text = @p",
            new SqlParameter("@p", SqlDbType.VarChar, 400) { Value = "hello" }); // NEGATIVE: ESC007-reverse-unicode (parameter converted, not column - harmless)
}
```

</details>


---

<sub>1m 28s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodQueries.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodQueries.cs

<details>
<summary>11 lines</summary>

```
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Good;

public static class GoodQueries
{
    public static IQueryable<Shared.Customer> EqualsOnColumn(GoodDbContext db, string email) =>
        db.Customers.Where(c => c.Email == email); // GOOD: fixes ESC005 (normalise the parameter; rely on CI collation)

    public static IQueryable<Shared.Product> PrefixViaStartsWith(GoodDbContext db, string prefix) =>
        db.Products.Where(p => p.Sku.StartsWith(prefix)); // GOOD: fixes ESC005 (LIKE 'abc%' is seekable)

    public static IQueryable<Shared.Customer> ExactMatchInsteadOfSuffix(GoodDbContext db, string email) =>
        db.Customers.Where(c => c.Email == email); // NEGATIVE: ESC006 has no mapping fix; suffix search needs a reversed computed column / full-text
}
```

</details>


---

<sub>1m 31s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\PredicateDbContext.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\PredicateDbContext.cs

<details>
<summary>42 lines</summary>

```
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Good;

// Correct mappings; used to show predicate rules are NOT fixed by mapping.
public class PredicateDbContext(DbContextOptions<PredicateDbContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Translation> Translations => Set<Translation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Person>(e =>
        {
            e.ToTable("People");
            e.Property(p => p.FirstName).HasMaxLength(100);
            e.Property(p => p.LastName).HasMaxLength(100);
            e.Property(p => p.ReversedLastName).HasMaxLength(100).HasComputedColumnSql("REVERSE([LastName])", stored: true); // GOOD: fixes ESC006 (indexed REVERSE computed column)
            e.HasIndex(p => p.FirstName); e.HasIndex(p => p.LastName); e.HasIndex(p => p.ReversedLastName);
        });
        b.Entity<Sale>(e =>
        {
            e.ToTable("Sales");
            e.Property(s => s.Price).HasPrecision(18, 2);
            e.Property(s => s.Status).IsUnicode(false).HasMaxLength(10);
            e.HasIndex(s => s.OrderDate); e.HasIndex(s => s.CreatedAt); e.HasIndex(s => s.Price); e.HasIndex(s => s.Status);
        });
        b.Entity<Account>(e =>
        {
            e.ToTable("Accounts");
            e.Property(a => a.AccountNumber).IsUnicode(false).HasMaxLength(20);
            e.HasIndex(a => a.AccountNumber);
        });
        b.Entity<Translation>(e =>
        {
            e.ToTable("Translations");
            e.Property(t => t.Key).IsUnicode(false).HasMaxLength(256);
            e.Property(t => t.Text).HasMaxLength(400);
            e.HasIndex(t => t.Text);
        });
    }
}
```

</details>


---

<sub>1m 24s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\Entities.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\Entities.cs

<details>
<summary>6 lines</summary>

```
namespace EfSargable.Sample.Shared;

// Plain POCOs: no attributes here so each DbContext's mapping is the ONLY source of truth.
public class Customer { public int Id { get; set; } public string Email { get; set; } = ""; public string? Notes { get; set; } }
public class Product  { public int Id { get; set; } public string Sku { get; set; } = ""; public string Code { get; set; } = ""; }
public class Order    { public int Id { get; set; } public string Reference { get; set; } = ""; }
public class Translation { public int Id { get; set; } public string Key { get; set; } = ""; public string Text { get; set; } = ""; }
```

</details>


---

<sub>1m 35s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\PredicateEntities.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\PredicateEntities.cs

<details>
<summary>5 lines</summary>

```
namespace EfSargable.Sample.Shared;

// Entities for the non-sargable predicate rules (article: "Non-sargable predicates").
public class Person  { public int Id { get; set; } public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public string ReversedLastName { get; private set; } = ""; }
public class Sale    { public int Id { get; set; } public DateTime OrderDate { get; set; } public DateTime CreatedAt { get; set; } public decimal Price { get; set; } public string? Status { get; set; } }
public class Account { public int Id { get; set; } public string AccountNumber { get; set; } = ""; }
```

</details>


---

<sub>1m 33s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\MappingGuardrail.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\MappingGuardrail.cs

```
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfSargable.Sample.Shared;

public record SchemaColumn(string Table, string Column, string Type, int Length, bool Indexed, string? Exception);
public record Finding(string Rule, string Entity, string Property, string Effective, string Expected, string Label);

/// <summary>
/// Model-metadata guardrail as recommended by detection.md: compares the EFFECTIVE
/// relational mapping (GetColumnType/IsUnicode/GetMaxLength) with the existing schema.
/// Static evidence only => every finding is labelled "candidate".
/// </summary>
public static partial class MappingGuardrail
{
    public static IReadOnlyList<SchemaColumn> LoadSchema(string path)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.GetProperty("columns").EnumerateArray().Select(c => new SchemaColumn(
            c.GetProperty("table").GetString()!, c.GetProperty("column").GetString()!,
            c.GetProperty("type").GetString()!, c.GetProperty("length").GetInt32(),
            c.GetProperty("indexed").GetBoolean(),
            c.TryGetProperty("exception", out var ex) ? ex.GetString() : null)).ToList();
    }

    public static IReadOnlyList<Finding> Check(DbContext db, IReadOnlyList<SchemaColumn> schema)
    {
        var findings = new List<Finding>();
        foreach (var et in db.Model.GetEntityTypes())
        {
            var table = "dbo." + et.GetTableName();
            foreach (var p in et.GetProperties().Where(p => p.ClrType == typeof(string)))
            {
                var col = schema.FirstOrDefault(s => s.Table == table && s.Column == p.GetColumnName());
                if (col is null) continue; // unknown schema -> not reported here
                var store = p.GetColumnType();
                var type = StoreType().Match(store).Groups[1].Value.ToLowerInvariant();
                var expected = $"{col.Type}({(col.Length < 0 ? "max" : col.Length.ToString())})";
                if (string.Equals(store, expected, StringComparison.OrdinalIgnoreCase)) continue;

                string rule = type == col.Type ? "ESC003-length-mismatch"
                    : col.Type == "varchar" ? (p.GetMaxLength() is not null ? "ESC002-maxlength-without-unicode" : "ESC001-unicode-mismatch")
                    : "ESC004-blanket-convention"; // real Unicode column forced to varchar
                findings.Add(new Finding(rule, et.ClrType.Name, p.Name, store, expected, "candidate"));
            }
        }
        return findings;
    }

    [GeneratedRegex(@"^(\w+)")] private static partial Regex StoreType();
}
```


---

<sub>1m 37s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\ContextFactory.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Shared\ContextFactory.cs

<details>
<summary>9 lines</summary>

```
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Shared;

public static class ContextFactory
{
    // No SQL Server is contacted: model building and ToQueryString() work offline.
    public const string FakeConnection = "Server=(none);Database=Sargable;Trusted_Connection=True;TrustServerCertificate=True";

    public static DbContextOptions<T> Options<T>() where T : DbContext =>
        new DbContextOptionsBuilder<T>().UseSqlServer(FakeConnection).Options;
}
```

</details>


---

<sub>1m 55s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadDbContext.cs.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadDbContext.cs (lines 1-10)

<details>
<summary>8 lines</summary>

```
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Seeded BAD mappings. Each finding is marked with  // BAD: <rule-id>
public class BadDbContext(DbContextOptions<BadDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
```

</details>


---

<sub>2m 7s</sub>

### `grep` — Failed

**.**

"." (["C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadDbContext.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadConventionDbContext.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadPredicates.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadQueries.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodDbContext.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodPredicates.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodQueries.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\PredicateDbContext.cs"])

```
Search paths do not exist: ["C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadDbContext.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadConventionDbContext.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadPredicates.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Bad\BadQueries.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodDbContext.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodPredicates.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\GoodQueries.cs","C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\EfSargable.Sample\Good\PredicateDbContext.cs"]
```


---

<sub>2m 19s</sub>

### `grep`

**.**

"." (C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Bad/BadDbContext.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Bad/BadConventionDbContext.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Bad/BadPredicates.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Bad/BadQueries.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Good/GoodDbContext.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Good/GoodPredicates.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Good/GoodQueries.cs, C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample/Good/PredicateDbContext.cs)

<details>
<summary>233 matches</summary>

```
[grep content: 224 matches across 8 file(s) under C:/Dev/nabs-darrel-schreyer/efcore-sargable-config/samples/EfSargable.Sample]

Bad/BadConventionDbContext.cs (22 match(es)):
  1: using EfSargable.Sample.Shared;
  2: using Microsoft.EntityFrameworkCore;
  4: namespace EfSargable.Sample.Bad;
  6: // The "blanket varchar(256)" global fix from evals.json case 3.
  7: public class BadConventionDbContext(DbContextOptions<BadConventionDbContext> options) : DbContext(options)
  8: {
  9:     public DbSet<Translation> Translations => Set<Translation>();
  11:     protected override void ConfigureConventions(ModelConfigurationBuilder builder)
  12:     {
  13:         builder.Properties<string>().AreUnicode(false).HaveMaxLength(256); // BAD: ESC004-blanket-convention (forces real Unicode Text nvarchar(400) to varchar(256))
  14:     }
  16:     protected override void OnModelCreating(ModelBuilder b)
  17:     {
  18:         b.Entity<Translation>(e =>
  19:         {
  20:             e.ToTable("Translations");
  21:             e.Property(t => t.Key); // NEGATIVE: convention gives varchar(256) which matches schema - must NOT be flagged
  22:             e.HasIndex(t => t.Key);
  23:             e.HasIndex(t => t.Text);
  24:         });
  25:     }
  26: }

Good/PredicateDbContext.cs (42 match(es)):
  1: using EfSargable.Sample.Shared;
  2: using Microsoft.EntityFrameworkCore;
  4: namespace EfSargable.Sample.Good;
  6: // Correct mappings; used to show predicate rules are NOT fixed by mapping.
  7: public class PredicateDbContext(DbContextOptions<PredicateDbContext> options) : DbContext(options)
  8: {
  9:     public DbSet<Person> People => Set<Person>();
  10:     public DbSet<Sale> Sales => Set<Sale>();
  11:     public DbSet<Account> Accounts => Set<Account>();
  12:     public DbSet<Translation> Translations => Set<Translation>();
  14:     protected override void OnModelCreating(ModelBuilder b)
  15:     {
  16:         b.Entity<Person>(e =>
  17:         {
  18:             e.ToTable("People");
  19:             e.Property(p => p.FirstName).HasMaxLength(100);
  20:             e.Property(p => p.LastName).HasMaxLength(100);
  21:             e.Property(p => p.ReversedLastName).HasMaxLength(100).HasComputedColumnSql("REVERSE([LastName])", stored: true); // GOOD: fixes ESC006 (indexed REVERSE computed column)
  22:             e.HasIndex(p => p.FirstName); e.HasIndex(p => p.LastName); e.HasIndex(p => p.ReversedLastName);
  23:         });
  24:         b.Entity<Sale>(e =>
  25:         {
  26:             e.ToTable("Sales");
  27:             e.Property(s => s.Price).HasPrecision(18, 2);
  28:             e.Property(s => s.Status).IsUnicode(false).HasMaxLength(10);
  29:             e.HasIndex(s => s.OrderDate); e.HasIndex(s => s.CreatedAt); e.HasIndex(s => s.Price); e.HasIndex(s => s.Status);
  30:         });
  31:         b.Entity<Account>(e =>
  32:         {
  33:             e.ToTable("Accounts");
  34:             e.Property(a => a.AccountNumber).IsUnicode(false).HasMaxLength(20);
  35:             e.HasIndex(a => a.AccountNumber);
  36:         });
  37:         b.Entity<Translation>(e =>
  38:         {
  39:             e.ToTable("Translations");
  40:             e.Property(t => t.Key).IsUnicode(false).HasMaxLength(256);
  41:             e.Property(t => t.Text).HasMaxLength(400);
  42:             e.HasIndex(t => t.Text);
  43:         });
  44:     }
  45: }

Bad/BadQueries.cs (14 match(es)):
  1: using EfSargable.Sample.Good;
  2: using Microsoft.EntityFrameworkCore;
  4: namespace EfSargable.Sample.Bad;
  6: // Non-mapping causes of scans. These run against the FIXED mapping on purpose:
  7: // the skill must report them as "other reason" and must NOT propose a mapping change.
  8: public static class BadQueries
  9: {
  10:     public static IQueryable<Shared.Customer> LowerOnColumn(GoodDbContext db, string email) =>
  11:         db.Customers.Where(c => c.Email.ToLower() == email); // BAD: ESC005-function-on-column (LOWER([c].[Email]) - mapping fix cannot help)
  13:     public static IQueryable<Shared.Product> PrefixViaSubstring(GoodDbContext db, string prefix) =>
  14:         db.Products.Where(p => p.Sku.Substring(0, 3) == prefix); // BAD: ESC005-function-on-column (SUBSTRING on column)
  16:     public static IQueryable<Shared.Customer> LeadingWildcard(GoodDbContext db, string term) =>
  17:         db.Customers.Where(c => c.Email.EndsWith(term)); // BAD: ESC006-leading-wildcard (LIKE '%term' - mapping fix cannot make it a seek)
  18: }

Bad/BadPredicates.cs (28 match(es)):
  1: using EfSargable.Sample.Good;
  2: using EfSargable.Sample.Shared;
  3: using Microsoft.EntityFrameworkCore;
  5: namespace EfSargable.Sample.Bad;
  7: // Non-sargable predicates. Mapping is correct, so every finding here is label "other-cause".
  8: public static class BadPredicates
  9: {
  10:     public static IQueryable<Sale> YearOnColumn(PredicateDbContext db) =>
  11:         db.Sales.Where(s => s.OrderDate.Year == 2025); // BAD: ESC005-function-on-column (DATEPART(year, OrderDate) = 2025)
  13:     public static IQueryable<Person> UpperOnColumn(PredicateDbContext db, string name) =>
  14:         db.People.Where(p => p.LastName.ToUpper() == name); // BAD: ESC005-function-on-column (UPPER(LastName); pointless on a CI collation)
  16:     public static IQueryable<Person> SuffixSearch(PredicateDbContext db) =>
  17:         db.People.Where(p => p.LastName.EndsWith("son")); // BAD: ESC006-leading-wildcard (LIKE '%son')
  19:     public static IQueryable<Person> ContainsSearch(PredicateDbContext db) =>
  20:         db.People.Where(p => p.LastName.Contains("son")); // BAD: ESC006-leading-wildcard (LIKE '%son%')
  22:     public static IQueryable<Sale> DateAddOnColumn(PredicateDbContext db, DateTime now) =>
  23:         db.Sales.Where(s => s.CreatedAt.AddDays(30) > now); // BAD: ESC008-arithmetic-on-column (DATEADD(day, 30, CreatedAt))
  25:     public static IQueryable<Sale> MathsOnColumn(PredicateDbContext db) =>
  26:         db.Sales.Where(s => s.Price * 1.1m > 110m); // BAD: ESC008-arithmetic-on-column (Price * 1.1 > 110)
  28:     public static IQueryable<Account> NumberVsVarchar(PredicateDbContext db) =>
  29:         db.Accounts.Where(a => Convert.ToInt32(a.AccountNumber) == 12345); // BAD: ESC009-implicit-conversion (varchar column converted to int)
  31:     public static IQueryable<Person> OrAcrossColumns(PredicateDbContext db, string name) =>
  32:         db.People.Where(p => p.FirstName == name || p.LastName == name); // BAD: ESC010-or-across-columns
  34:     public static IQueryable<Sale> CoalesceOnColumn(PredicateDbContext db) =>
  35:         db.Sales.Where(s => (s.Status ?? "New") == "New"); // BAD: ESC011-isnull-on-column (COALESCE(Status, 'New'))
  37:     public static IQueryable<Sale> ClosedEndDate(PredicateDbContext db) =>
  38:         db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate <= new DateTime(2025, 12, 31)); // BAD: ESC012-closed-date-range (misses rows after midnight 31 Dec)
  39: }

Bad/BadDbContext.cs (33 match(es)):
  1: using EfSargable.Sample.Shared;
  2: using Microsoft.EntityFrameworkCore;
  4: namespace EfSargable.Sample.Bad;
  6: // Seeded BAD mappings. Each finding is marked with  // BAD: <rule-id>
  7: public class BadDbContext(DbContextOptions<BadDbContext> options) : DbContext(options)
  8: {
  9:     public DbSet<Customer> Customers => Set<Customer>();
  10:     public DbSet<Product> Products => Set<Product>();
  11:     public DbSet<Order> Orders => Set<Order>();
  13:     protected override void OnModelCreating(ModelBuilder b)
  14:     {
  15:         b.Entity<Customer>(e =>
  16:         {
  17:             e.ToTable("Customers");
  18:             e.Property(c => c.Email); // BAD: ESC001-unicode-mismatch (no config + index -> nvarchar(450); column is varchar(256) indexed)
  19:             e.HasIndex(c => c.Email);
  20:             e.Property(c => c.Notes); // NEGATIVE: intentional-exception (nvarchar(max) matches schema, unindexed long text)
  21:         });
  23:         b.Entity<Product>(e =>
  24:         {
  25:             e.ToTable("Products");
  26:             e.Property(p => p.Sku).HasMaxLength(32); // BAD: ESC002-maxlength-without-unicode (HasMaxLength alone -> nvarchar(32); column varchar(32))
  27:             e.HasIndex(p => p.Sku);
  28:             e.Property(p => p.Code).HasColumnType("varchar(16)"); // NEGATIVE: store type alone supplies varchar(16) - must NOT be flagged
  29:             e.HasIndex(p => p.Code);
  30:         });
  32:         b.Entity<Order>(e =>
  33:         {
  34:             e.ToTable("Orders");
  35:             e.Property(o => o.Reference).IsUnicode(false); // BAD: ESC003-length-mismatch (varchar(max) vs varchar(20); NOT a Unicode conversion)
  36:         });
  37:     }
  38: }

Good/GoodDbContext.cs (43 match(es)):
  1: using EfSargable.Sample.Shared;
  2: using Microsoft.EntityFrameworkCore;
  4: namespace EfSargable.Sample.Good;
  6: // FIXED counterparts: every mapping matches schema.json. Nothing here should be flagged.
  7: public class GoodDbContext(DbContextOptions<GoodDbContext> options) : DbContext(options)
  8: {
  9:     public DbSet<Customer> Customers => Set<Customer>();
  10:     public DbSet<Product> Products => Set<Product>();
  11:     public DbSet<Order> Orders => Set<Order>();
  12:     public DbSet<Translation> Translations => Set<Translation>();
  14:     protected override void OnModelCreating(ModelBuilder b)
  15:     {
  16:         b.Entity<Customer>(e =>
  17:         {
  18:             e.ToTable("Customers");
  19:             e.Property(c => c.Email).IsUnicode(false).HasMaxLength(256); // GOOD: fixes ESC001
  20:             e.HasIndex(c => c.Email);
  21:             e.Property(c => c.Notes); // NEGATIVE: intentional-exception
  22:         });
  24:         b.Entity<Product>(e =>
  25:         {
  26:             e.ToTable("Products");
  27:             e.Property(p => p.Sku).IsUnicode(false).HasMaxLength(32); // GOOD: fixes ESC002
  28:             e.HasIndex(p => p.Sku);
  29:             e.Property(p => p.Code).HasColumnType("varchar(16)"); // NEGATIVE: store type alone is sufficient
  30:             e.HasIndex(p => p.Code);
  31:         });
  33:         b.Entity<Order>(e =>
  34:         {
  35:             e.ToTable("Orders");
  36:             e.Property(o => o.Reference).IsUnicode(false).HasMaxLength(20); // GOOD: fixes ESC003
  37:             e.HasIndex(o => o.Reference);
  38:         });
  40:         b.Entity<Translation>(e =>
  41:         {
  42:             e.ToTable("Translations");
  43:             e.Property(t => t.Key).IsUnicode(false).HasMaxLength(256); // GOOD: scoped mapping instead of ESC004 blanket convention
  44:             e.HasIndex(t => t.Key);
  45:             e.Property(t => t.Text).HasMaxLength(400); // NEGATIVE: real Unicode nvarchar(400) - must stay Unicode
  46:             e.HasIndex(t => t.Text);
  47:         });
  48:     }
  49: }

Good/GoodQueries.cs (11 match(es)):
  1: using Microsoft.EntityFrameworkCore;
  3: namespace EfSargable.Sample.Good;
  5: public static class GoodQueries
  6: {
  7:     public static IQueryable<Shared.Customer> EqualsOnColumn(GoodDbContext db, string email) =>
  8:         db.Customers.Where(c => c.Email == email); // GOOD: fixes ESC005 (normalise the parameter; rely on CI collation)
  10:     public static IQueryable<Shared.Product> PrefixViaStartsWith(GoodDbContext db, string prefix) =>
  11:         db.Products.Where(p => p.Sku.StartsWith(prefix)); // GOOD: fixes ESC005 (LIKE 'abc%' is seekable)
  13:     public static IQueryable<Shared.Customer> ExactMatchInsteadOfSuffix(GoodDbContext db, string email) =>
  14:         db.Customers.Where(c => c.Email == email); // NEGATIVE: ESC006 has no mapping fix; suffix search needs a reversed computed column / full-text
  15: }

Good/GoodPredicates.cs (31 match(es)):
  1: using EfSargable.Sample.Shared;
  2: using Microsoft.Data.SqlClient;
  3: using Microsoft.EntityFrameworkCore;
  4: using System.Data;
  6: namespace EfSargable.Sample.Good;
  8: public static class GoodPredicates
  9: {
  10:     public static IQueryable<Sale> YearAsHalfOpenRange(PredicateDbContext db) =>
  11:         db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate < new DateTime(2026, 1, 1)); // GOOD: fixes ESC005 and ESC012
  13:     public static IQueryable<Person> BareColumnEquals(PredicateDbContext db, string name) =>
  14:         db.People.Where(p => p.LastName == name); // GOOD: fixes ESC005 (CI collation already ignores case)
  16:     public static IQueryable<Person> SuffixViaReversedColumn(PredicateDbContext db) =>
  17:         db.People.Where(p => p.ReversedLastName.StartsWith("nos")); // GOOD: fixes ESC006 (LIKE 'nos%' on indexed REVERSE computed column)
  19:     public static IQueryable<Person> PrefixSearch(PredicateDbContext db) =>
  20:         db.People.Where(p => p.LastName.StartsWith("Smi")); // NEGATIVE: LIKE 'Smi%' is sargable
  22:     public static IQueryable<Sale> DateAddOnConstant(PredicateDbContext db, DateTime now) =>
  23:         db.Sales.Where(s => s.CreatedAt > now.AddDays(-30)); // GOOD: fixes ESC008
  25:     public static IQueryable<Sale> MathsOnConstant(PredicateDbContext db) =>
  26:         db.Sales.Where(s => s.Price > 100m); // GOOD: fixes ESC008
  28:     public static IQueryable<Account> StringVsVarchar(PredicateDbContext db) =>
  29:         db.Accounts.Where(a => a.AccountNumber == "12345"); // GOOD: fixes ESC009
  31:     public static IQueryable<Person> UnionPerColumn(PredicateDbContext db, string name) =>
  32:         db.People.Where(p => p.FirstName == name).Union(db.People.Where(p => p.LastName == name)); // GOOD: fixes ESC010
  34:     public static IQueryable<Sale> OrIsNull(PredicateDbContext db) =>
  35:         db.Sales.Where(s => s.Status == "New" || s.Status == null); // GOOD: fixes ESC011
  37:     public static IQueryable<Sale> CastToDate(PredicateDbContext db, DateTime d) =>
  38:         db.Sales.Where(s => s.OrderDate.Date == d); // NEGATIVE: CAST(dt AS date) can still seek (optimiser rewrite) - mention, don't flag
  40:     public static IQueryable<Translation> VarcharParamVsNvarcharColumn(PredicateDbContext db) =>
  41:         db.Translations.FromSqlRaw("SELECT * FROM Translations WHERE Text = @p",
  42:             new SqlParameter("@p", SqlDbType.VarChar, 400) { Value = "hello" }); // NEGATIVE: ESC007-reverse-unicode (parameter converted, not column - harmless)
  43: }
```

</details>


---

<sub>5m 15s</sub>

### Copilot

Findings from static review (no DB access; LINQ/attributes compared against `schema.json`).

```
[ESC001-unicode-mismatch] samples/EfSargable.Sample/Bad/BadDbContext.cs:18  Customer.Email
Label: candidate
Effective mapping: nvarchar(450) (no Unicode/MaxLength set, property is indexed)   Column: varchar(256), indexed
Evidence: not measured; expected CONVERT_IMPLICIT(nvarchar(450), [c].[Email], 0) against the varchar column (data type precedence)
Fix: .IsUnicode(false).HasMaxLength(256)
Validate: re-check GetColumnType()/IsUnicode() metadata; confirm CONVERT_IMPLICIT disappears from the actual plan

[ESC002-maxlength-without-unicode] samples/EfSargable.Sample/Bad/BadDbContext.cs:26  Product.Sku
Label: candidate
Effective mapping: nvarchar(32) (HasMaxLength set, Unicode defaults true)   Column: varchar(32), indexed
Evidence: not measured; HasMaxLength alone does not set IsUnicode(false)
Fix: add .IsUnicode(false) (keep HasMaxLength(32))
Validate: metadata check; confirm Seek on Sku in an actual plan

[ESC003-length-mismatch] samples/EfSargable.Sample/Bad/BadDbContext.cs:35  Order.Reference
Label: candidate
Effective mapping: varchar(max) (IsUnicode(false) set, no MaxLength)   Column: varchar(20), indexed
Evidence: not measured; same type family (varchar), so no column-side conversion expected — risk is oversized memory grant / column unusable as an index key at max
Fix: add .HasMaxLength(20)
Validate: metadata check for GetMaxLength()==20; confirm no values are being silently affected by the length change

[ESC004-blanket-convention] samples/EfSargable.Sample/Bad/BadConventionDbContext.cs:13  Translation.Text
Label: candidate
Effective mapping: varchar(256) (ConfigureConventions blanket rule, no per-property override for Text)   Column: nvarchar(400), indexed
Evidence: not measured; convention runs pre-model-building and silently overrides the real Unicode column
Fix: scope the convention, or add e.Property(t => t.Text).HasMaxLength(400); (leave Unicode true) in OnModelCreating
Validate: metadata check for IsUnicode()==true, GetMaxLength()==400; audit all other string properties under this convention

[ESC005-function-on-column] samples/EfSargable.Sample/Bad/BadPredicates.cs:11  Sale.OrderDate (YearOnColumn)
Label: other-cause
Effective mapping: n/a   Column: unknown (Sales not in schema.json)
Evidence: not measured; expected YEAR([s].[OrderDate]) = 2025
Fix: s.OrderDate >= new DateTime(2025,1,1) && s.OrderDate < new DateTime(2026,1,1)
Validate: compare ToQueryString() before/after; actual plan shows Seek Predicates, not residual Predicate

[ESC005-function-on-column] samples/EfSargable.Sample/Bad/BadPredicates.cs:14  Person.LastName (UpperOnColumn)
Label: other-cause
Effective mapping: n/a   Column: unknown (People not in schema.json)
Evidence: not measured; expected UPPER([p].[LastName]) = @name
Fix: drop .ToUpper(), compare p.LastName == name directly (relies on CI collation — confirm)
Validate: confirm server/column collation is case-insensitive; actual plan for Seek

[ESC006-leading-wildcard] samples/EfSargable.Sample/Bad/BadPredicates.cs:17  Person.LastName (SuffixSearch)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; expected LIKE '%son'
Fix: prefix search, full-text, or indexed REVERSE(LastName) computed column searched with StartsWith("nos")
Validate: actual plan confirms Seek on the computed column index

[ESC006-leading-wildcard] samples/EfSargable.Sample/Bad/BadPredicates.cs:20  Person.LastName (ContainsSearch)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; expected LIKE '%son%'
Fix: same as above (reversed computed column / full-text)
Validate: same

[ESC008-arithmetic-on-column] samples/EfSargable.Sample/Bad/BadPredicates.cs:23  Sale.CreatedAt (DateAddOnColumn)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; expected DATEADD(day, 30, [s].[CreatedAt]) > @now
Fix: s.CreatedAt > now.AddDays(-30)
Validate: actual plan Seek; verify date-math equivalence at edges

[ESC008-arithmetic-on-column] samples/EfSargable.Sample/Bad/BadPredicates.cs:26  Sale.Price (MathsOnColumn)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; expected [s].[Price] * 1.1 > 110
Fix: s.Price > 100m
Validate: verify rounding equivalence; actual plan Seek

[ESC009-implicit-conversion] samples/EfSargable.Sample/Bad/BadPredicates.cs:29  Account.AccountNumber (NumberVsVarchar)
Label: other-cause
Effective mapping: n/a   Column: varchar(20), indexed
Evidence: not measured; expected CAST([a].[AccountNumber] AS int) = 12345; also fails on non-numeric values
Fix: a.AccountNumber == "12345"
Validate: actual plan Seek; confirm no non-numeric AccountNumber rows depend on the old cast behavior

[ESC010-or-across-columns] samples/EfSargable.Sample/Bad/BadPredicates.cs:32  Person.FirstName / Person.LastName (OrAcrossColumns)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; expected [p].[FirstName] = @name OR [p].[LastName] = @name, defeating single-index seek
Fix: db.People.Where(p => p.FirstName == name).Union(db.People.Where(p => p.LastName == name))
Validate: confirm UNION (not UNION ALL) needed if a row can match both predicates; actual plan shows two Seeks

[ESC011-isnull-on-column] samples/EfSargable.Sample/Bad/BadPredicates.cs:35  Sale.Status (CoalesceOnColumn)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; expected COALESCE([s].[Status], 'New') = 'New'
Fix: s.Status == "New" || s.Status == null
Validate: actual plan Seek; confirm whether Status should simply disallow NULL

[ESC012-closed-date-range] samples/EfSargable.Sample/Bad/BadPredicates.cs:38  Sale.OrderDate (ClosedEndDate)
Label: other-cause
Effective mapping: n/a   Column: unknown
Evidence: not measured; closed upper bound <= '2025-12-31' silently excludes rows with a time part after midnight — correctness, not speed
Fix: s.OrderDate < new DateTime(2026,1,1)
Validate: diff row counts between closed and half-open range on representative data

[ESC005-function-on-column] samples/EfSargable.Sample/Bad/BadQueries.cs:11  Customer.Email (LowerOnColumn)
Label: other-cause
Effective mapping: varchar(256) (GoodDbContext mapping already matches schema)   Column: varchar(256), indexed
Evidence: not measured; expected LOWER([c].[Email]) = @email — mapping fix cannot help here
Fix: drop .ToLower(), compare c.Email == email directly
Validate: confirm CI collation; actual plan Seek

[ESC005-function-on-column] samples/EfSargable.Sample/Bad/BadQueries.cs:14  Product.Sku (PrefixViaSubstring)
Label: other-cause
Effective mapping: varchar(32) (GoodDbContext mapping already matches schema)   Column: varchar(32), indexed
Evidence: not measured; expected SUBSTRING([p].[Sku], 1, 3) = @prefix
Fix: p.Sku.StartsWith(prefix)
Validate: actual plan Seek

[ESC006-leading-wildcard] samples/EfSargable.Sample/Bad/BadQueries.cs:17  Customer.Email (LeadingWildcard)
Label: other-cause
Effective mapping: varchar(256) (GoodDbContext mapping already matches schema)   Column: varchar(256), indexed
Evidence: not measured; expected LIKE '%' + @term — mapping fix cannot make this a seek
Fix: none via mapping; needs reversed computed column / full-text search
Validate: actual plan confirms residual Predicate/Scan remains even after any mapping change

[ESC007-reverse-unicode] samples/EfSargable.Sample/Good/GoodPredicates.cs:42  Translation.Text (VarcharParamVsNvarcharColumn)
Label: intentional-exception
Effective mapping: nvarchar(400) (PredicateDbContext: HasMaxLength(400), Unicode left default true)   Column: nvarchar(400), indexed
Evidence: not measured; SqlParameter declared SqlDbType.VarChar(400) against an nvarchar column — the parameter, not the column, gets converted (harmless by data type precedence)
Fix: none - exception
Validate: confirm via actual plan that only the parameter is converted, not Text

Summary: 18 findings (confirmed 0, candidate 4, other-cause 13, exception 1)
```

Note (not a finding): `Good/GoodPredicates.cs:38` (`s.OrderDate.Date == d`) and `Good/GoodDbContext.cs`/`Good/GoodQueries.cs` showed no mismatches — all mappings match `schema.json` and all predicates keep the indexed column bare.

---

<sub>Generated by [GitHub Copilot CLI](https://github.com/features/copilot/cli)</sub>
