using EfSargable.Sample.Bad;
using EfSargable.Sample.Good;
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfSargable.Tests;

static class Fx
{
    public static BadDbContext Bad() => new(ContextFactory.Options<BadDbContext>());
    public static BadConventionDbContext BadConv() => new(ContextFactory.Options<BadConventionDbContext>());
    public static GoodDbContext Good() => new(ContextFactory.Options<GoodDbContext>());
    public static IReadOnlyList<SchemaColumn> Schema => MappingGuardrail.LoadSchema(Path.Combine(AppContext.BaseDirectory, "schema.json"));
    public static string Decl(string sql) => string.Join("\n", sql.Split('\n').Where(l => l.StartsWith("DECLARE")));
}

public class EffectiveMappingTests
{
    [Theory] // proves each seeded defect exists in the RESOLVED model
    [InlineData(typeof(Customer), "Email", "nvarchar(450)")]   // ESC001: indexed + unconfigured -> EF picks nvarchar(450) (index key limit), not max
    [InlineData(typeof(Product), "Sku", "nvarchar(32)")]       // ESC002: HasMaxLength does NOT imply IsUnicode(false)
    [InlineData(typeof(Order), "Reference", "varchar(max)")]   // ESC003
    [InlineData(typeof(Product), "Code", "varchar(16)")]       // negative: HasColumnType alone is enough
    public void Bad_context_effective_mapping(Type entity, string prop, string store)
    {
        using var db = Fx.Bad();
        Assert.Equal(store, db.Model.FindEntityType(entity)!.FindProperty(prop)!.GetColumnType());
    }

    [Fact]
    public void ESC004_blanket_convention_forces_unicode_text_to_varchar()
    {
        using var db = Fx.BadConv();
        var et = db.Model.FindEntityType(typeof(Translation))!;
        Assert.Equal("varchar(256)", et.FindProperty("Text")!.GetColumnType()); // schema: nvarchar(400) -> data loss risk
        Assert.Equal("varchar(256)", et.FindProperty("Key")!.GetColumnType());  // negative: correct by convention
    }

    [Theory]
    [InlineData(typeof(Customer), "Email", "varchar(256)")]
    [InlineData(typeof(Customer), "Notes", "nvarchar(max)")]
    [InlineData(typeof(Product), "Sku", "varchar(32)")]
    [InlineData(typeof(Product), "Code", "varchar(16)")]
    [InlineData(typeof(Order), "Reference", "varchar(20)")]
    [InlineData(typeof(Translation), "Key", "varchar(256)")]
    [InlineData(typeof(Translation), "Text", "nvarchar(400)")]
    public void Good_context_matches_schema(Type entity, string prop, string store)
    {
        using var db = Fx.Good();
        Assert.Equal(store, db.Model.FindEntityType(entity)!.FindProperty(prop)!.GetColumnType());
    }
}

public class GeneratedSqlTests
{
    // ToQueryString() previews parameter declarations; the skill rightly says this is not plan proof.
    [Fact]
    public void ESC001_bad_nvarchar_param_vs_good_varchar256_param()
    {
        var email = "a@b.com";
        using var bad = Fx.Bad(); using var good = Fx.Good();
        Assert.Contains("nvarchar", Fx.Decl(bad.Customers.Where(c => c.Email == email).ToQueryString()));
        var g = Fx.Decl(good.Customers.Where(c => c.Email == email).ToQueryString());
        Assert.Contains("varchar(256)", g);
        Assert.DoesNotContain("nvarchar", g);
    }

    [Fact]
    public void ESC002_bad_nvarchar32_vs_good_varchar32()
    {
        var sku = "ABC";
        using var bad = Fx.Bad(); using var good = Fx.Good();
        Assert.Contains("nvarchar(32)", Fx.Decl(bad.Products.Where(p => p.Sku == sku).ToQueryString()));
        var g = Fx.Decl(good.Products.Where(p => p.Sku == sku).ToQueryString());
        Assert.Contains("varchar(32)", g); Assert.DoesNotContain("nvarchar", g);
    }

    [Fact]
    public void Negative_HasColumnType_only_sends_varchar_param()
    {
        var code = "X"; using var db = Fx.Bad();
        var d = Fx.Decl(db.Products.Where(p => p.Code == code).ToQueryString());
        Assert.Contains("varchar(16)", d); Assert.DoesNotContain("nvarchar", d);
    }

    [Fact]
    public void ESC005_function_on_column_survives_the_mapping_fix()
    {
        using var db = Fx.Good();
        Assert.Contains("LOWER([c].[Email])", BadQueries.LowerOnColumn(db, "x").ToQueryString());
        Assert.Contains("SUBSTRING([p].[Sku]", BadQueries.PrefixViaSubstring(db, "abc").ToQueryString());
        var good = GoodQueries.EqualsOnColumn(db, "x").ToQueryString();
        Assert.Contains("WHERE [c].[Email] = @", good);
        Assert.DoesNotContain("LOWER(", good);
    }

    [Fact]
    public void ESC006_leading_wildcard_vs_prefix()
    {
        using var db = Fx.Good();
        var bad = BadQueries.LeadingWildcard(db, "term").ToQueryString();
        var good = GoodQueries.PrefixViaStartsWith(db, "abc").ToQueryString();
        Assert.Contains("LIKE", bad); Assert.Contains("'%term'", bad);  // leading % -> no seek, regardless of mapping
        Assert.Contains("LIKE", good); Assert.Contains("'abc%'", good); // trailing % -> seekable
    }
}

public class GuardrailTests
{
    [Fact]
    public void Bad_contexts_produce_exactly_the_seeded_mapping_findings()
    {
        using var bad = Fx.Bad(); using var conv = Fx.BadConv();
        var found = MappingGuardrail.Check(bad, Fx.Schema).Concat(MappingGuardrail.Check(conv, Fx.Schema))
            .Select(f => $"{f.Rule}:{f.Entity}.{f.Property}").OrderBy(x => x, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[] {
            "ESC001-unicode-mismatch:Customer.Email",
            "ESC002-maxlength-without-unicode:Product.Sku",
            "ESC003-length-mismatch:Order.Reference",
            "ESC004-blanket-convention:Translation.Text" }, found);
    }

    [Fact]
    public void Good_context_produces_no_findings()
    {
        using var good = Fx.Good();
        Assert.Empty(MappingGuardrail.Check(good, Fx.Schema));
    }

    [Fact]
    public void Static_findings_are_candidates_not_confirmed()
    {
        using var bad = Fx.Bad();
        Assert.All(MappingGuardrail.Check(bad, Fx.Schema), f => Assert.Equal("candidate", f.Label));
    }
}

public class ManifestTests
{
    [Fact]
    public void Every_manifest_line_points_at_its_marker_comment()
    {
        using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "expected-findings.json")));
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d != null && !Directory.Exists(Path.Combine(d.FullName, "samples"))) d = d.Parent;
        foreach (var f in doc.RootElement.GetProperty("findings").EnumerateArray())
        {
            var line = File.ReadAllLines(Path.Combine(d!.FullName, f.GetProperty("file").GetString()!))[f.GetProperty("line").GetInt32() - 1];
            var ok = f.GetProperty("should_flag").GetBoolean() ? line.Contains("BAD: " + f.GetProperty("rule").GetString()) : (line.Contains("// NEGATIVE") || line.Contains("// GOOD"));
            Assert.True(ok, $"manifest mismatch at {f}: '{line}'");
        }
    }
}