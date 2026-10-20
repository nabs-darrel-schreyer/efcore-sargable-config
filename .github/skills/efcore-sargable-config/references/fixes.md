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