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

## Verified on SQL Server 2022 (50k rows, test harness)

- A **seek is not proof of sargability**. EF parameterises `EndsWith(term)`/`Contains(term)` as `LIKE @p`; the plan shows an Index Seek (LikeRangeStart/LikeRangeEnd) that covers the whole index, with the same logical reads as a scan. Judge ESC006 by Seek Predicates bounds and reads, not the operator name.
- ESC010: with an index on each column the optimiser **did** combine two seeks itself (index union, Merge Join) - same reads as the UNION rewrite. Report ESC010 as a candidate and confirm with the plan; the UNION rewrite is not always a gain.
- Windows collation: dynamic seek with `GetRangeThroughConvert` confirmed, but no `PlanAffectingConvert` Cardinality Estimate warning appeared for a simple equality - absence of the warning does not rule out the conversion.
- ESC004: a blanket `AreUnicode(false)` sends a varchar parameter to a real nvarchar column. It still seeks (ESC007 shape), but non-Latin values are converted to `?` and the query returns **wrong results** (0 rows instead of 1).

## Not every scan is a bug

A scan may be optimal for low selectivity, a small table, or stale statistics. Investigate when a **selective** query scans.