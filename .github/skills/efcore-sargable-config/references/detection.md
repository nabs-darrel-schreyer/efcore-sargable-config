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