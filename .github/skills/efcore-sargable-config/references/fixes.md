# Mapping Fixes

Match the existing column's type and declared length after checking data and domain constraints. For a confirmed `varchar(256)` email column:

```csharp
[Unicode(false)]
[MaxLength(256)]
public string Email { get; set; } = string.Empty;
```

Or at the existing Fluent mapping site:

```csharp
builder.Property(customer => customer.Email)
    .IsUnicode(false)
    .HasMaxLength(256);
```

If exact store type is required, `.HasColumnType("varchar(256)")` is another option. Confirm the resolved parameter type; `HasMaxLength` alone does not set `IsUnicode(false)`. For a real `nvarchar(256)` column, keep Unicode and configure its actual length instead.

Use a pre-convention only when most strings genuinely share that contract and exceptions are explicit:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder builder)
{
    builder.Properties<string>()
        .AreUnicode(false)
        .HaveMaxLength(256);
}
```

This affects unrelated properties, including string keys and foreign keys. Review every exception and effective mapping, including property attributes and later Fluent configuration; do not assume attributes win over pre-conventions. If changing a column type, plan a reviewed migration: check representable characters/collation, truncation, indexes, constraints, writes, and deployment compatibility. A model-only change should not be treated as permission to alter existing data.

For a code fix, locate the owning mapping and offer a change only when the intended column type/length is known; otherwise offer a diagnostic and request schema evidence. Verify the model, executed parameter, and actual plan on the same representative query after applying the fix.