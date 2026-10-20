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
| ESC006-leading-wildcard | `LIKE '%son'` / `'%son%'` (`EndsWith`, `Contains`). A parameterised `LIKE @p` can show as an Index Seek over the full range - check reads. | Prefix `LIKE 'Smi%'`; full-text search; indexed computed column `REVERSE(LastName)` searched with `'nos%'`. | A2 |
| ESC008-arithmetic-on-column | `DATEADD(day, 30, CreatedAt) > SYSDATETIME()` (`o.CreatedAt.AddDays(30) > now`), `Price * 1.1 > 110`. | Move maths to the constant: `CreatedAt > DATEADD(day, -30, ...)`, `Price > 100`. Check equivalence (integer division, rounding, month arithmetic). | A2 |
| ESC009-implicit-conversion | Column converted to another type: `AccountNumber = 12345` on `varchar(20)` (LINQ `Convert.ToInt32(a.AccountNumber) == 12345` -> `CAST(... AS int)`). Can also fail on non-numeric rows. | Compare with the column's type: `AccountNumber = '12345'`. | A2 |
| ESC010-or-across-columns | `FirstName = @Name OR LastName = @Name`. The optimiser may already index-union two seeks; confirm with the plan before rewriting. | `UNION` of two single-column queries (`UNION`, not `UNION ALL`, if a row can match both) - LINQ `.Union(...)`. | A2 |
| ESC011-isnull-on-column | `ISNULL(Status,'New') = 'New'` / `COALESCE` (`(s.Status ?? "New") == "New"`). | `Status = 'New' OR Status IS NULL`; better, ask whether the column should allow NULLs. | A2 |
| ESC012-closed-date-range | `BETWEEN '20250101' AND '20251231'` (`<= new DateTime(2025,12,31)`) on a column with a time part: sargable but **silently misses rows after midnight**. Correctness, not speed. | Half-open range `< '20260101'`. | A2 |

## Out of scope / not statically testable

Collation behaviour (SQL_ vs Windows collation, dynamic seek via GetRangeThroughConvert), parameter sniffing, memory grants, plan-cache/Query Store evidence, PostgreSQL/Oracle/MySQL expression indexes (`lower(email)`, `created_at::date`). Report them as checks to run, not findings.