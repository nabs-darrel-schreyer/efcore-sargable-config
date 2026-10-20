# Detection and Confirmation

## Static inventory

1. Find `DbContext`, `DbSet`, `IEntityTypeConfiguration`, `OnModelCreating`, `ConfigureConventions`, property attributes, and converters. Include shadow properties and owned/shared types if the target query uses them.
2. Evaluate the effective relational mapping for each relevant string property (`GetColumnType()`, `GetMaxLength()`, `IsUnicode()` and the column mapping where available); configuration from conventions, attributes, Fluent API, or provider defaults may differ from the entity declaration. Check runtime metadata or the model snapshot when source-level inspection is inconclusive.
3. For an optional Roslyn analyzer, use syntax/semantic symbols to find candidate `string` properties, then account for attributes and Fluent configuration; a syntax-only rule cannot prove the effective mapping across files, contexts, or provider conventions. Emit an informational diagnostic with a property location and an exception/suppression path. Prefer a model-metadata integration test for enforcing actual resolved mappings in CI.

## Schema comparison

Read column type, length, collation, and index from SQL Server system catalog or an approved schema dump. For example, replace the table name with the relevant schema-qualified object:

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

`max_length` is in bytes; for `nvarchar(n)` divide by two (except `-1`, which means `max`). Include schema and table identity in a multi-table report.

## Runtime checks

- `ToQueryString()` helps find the translated predicate and, depending on provider/version, parameter declarations. Check captured `DbCommand` SQL and parameter `DbType`, `Size`, and value for the executed command; avoid logging sensitive parameter values outside a controlled test.
- Obtain an **actual** execution plan for a representative parameter and inspect `CONVERT_IMPLICIT`, `PlanAffectingConvert`, Seek/Scan operators, seek vs residual predicates, row estimates, and reads (`SET STATISTICS IO ON` in a safe test session). Capture the same metrics before and after the fix.
- Cached plans are optional and incomplete (eviction, recompiles, different parameter shapes). If only static evidence is available, label the result `candidate` and list the missing checks; do not claim a plan regression.