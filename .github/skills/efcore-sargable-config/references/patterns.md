# SQL Server String Predicate Patterns

## Strong evidence of a mapping problem

- Indexed `varchar(n)` column, EF `nvarchar` parameter, and a plan predicate like `CONVERT_IMPLICIT(nvarchar(...), [t].[Code], 0) = @__code_0`. The conversion is on the column; confirm it in the actual plan before assigning blame for a Scan.
- The reverse type mismatch may convert the parameter instead; inspect the actual plan rather than inferring conversion direction from the declared types alone.
- `PlanAffectingConvert` warnings are a lead, not a substitute for inspecting the indexed predicate, index choice, cardinality, and logical reads.

## Other reasons a query may scan

- `WHERE LOWER([t].[Code]) = @code` or `WHERE LEFT([t].[Code], 3) = @prefix` applies a function to the column. A mapping change will not remove that expression.
- `LIKE '%abc'` has a leading wildcard, unlike `LIKE 'abc%'`. Parameter collation, computed columns, and full-text requirements may also matter.
- A scan may be optimal for low selectivity, a small table, a covering index, or stale/missing statistics. A Seek can still read many rows.

## Collation caveat

Legacy `SQL_` collations and Windows collations can produce different plans for `varchar` column / `nvarchar` parameter comparisons. Neither guarantees a particular plan. Do not change collation or assert an Index Scan without measuring the target database.

## Length caveat

SQL Server `nvarchar(n)` and `varchar(n)` use different storage/length semantics; EF `MaxLength` specifies characters, while SQL Server catalog `sys.columns.max_length` is bytes (`-1` means `max`). A length mismatch can affect parameter declarations, index limits, and plan quality without necessarily causing a column-side conversion.