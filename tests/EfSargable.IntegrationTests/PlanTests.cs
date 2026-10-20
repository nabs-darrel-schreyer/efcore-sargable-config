using Xunit;

namespace EfSargable.IntegrationTests;

/// <summary>Plan-level proof against a real SQL Server 2022 container (server collation SQL_Latin1_General_CP1_CI_AS).</summary>
[Collection(SqlCollection.Name), Trait("Category", "Integration")]
public class PlanTests(SqlServerFixture fx)
{
    readonly Dictionary<string, (string Column, Func<PlanResult> Run)> _cases = new Cases(fx).All();
    PlanResult R(string key) => _cases[key].Run();

    [Fact] public void Server_uses_SQL_collation() => Assert.Equal("SQL_Latin1_General_CP1_CI_AS", fx.ServerCollation);

    [Theory]
    [InlineData("ESC001", "Email")]
    [InlineData("ESC002", "Sku")]
    public void Mapping_mismatch_converts_column_and_scans_fix_seeks(string rule, string col)
    {
        var bad = R($"{rule}:bad"); var good = R($"{rule}:good");
        Assert.True(bad.HasConvertImplicitOn(col)); Assert.True(bad.HasPlanAffectingConvert("Seek Plan"));
        Assert.False(bad.SeeksOn(col)); Assert.True(bad.Scans);
        Assert.True(good.SeeksOn(col)); Assert.False(good.HasConvertImplicitOn(col));
        Assert.True(bad.LogicalReads > 20 * good.LogicalReads, $"reads bad={bad.LogicalReads} good={good.LogicalReads}");
    }

    [Fact] // skill claim: a length mismatch alone does not cause a column-side conversion
    public void ESC003_length_mismatch_still_seeks_without_conversion()
    {
        var bad = R("ESC003:bad");
        Assert.True(bad.SeeksOn("Reference")); Assert.False(bad.HasConvertImplicitOn("Reference"));
    }

    [Fact] // ESC004: varchar parameter vs real nvarchar column still seeks, but Unicode data is lost -> WRONG RESULTS
    public void ESC004_blanket_convention_seeks_but_loses_unicode_match()
    {
        var bad = R("ESC004:bad"); var good = R("ESC004:good");
        Assert.True(bad.SeeksOn("Text")); Assert.Equal(0, bad.Rows); Assert.Equal(1, good.Rows);
    }

    [Fact] // A1: reverse case is harmless - parameter converted, not the column
    public void ESC007_reverse_case_seeks_without_column_conversion()
    {
        var r = R("ESC007:clean");
        Assert.True(r.SeeksOn("Text")); Assert.False(r.HasConvertImplicitOn("Text"));
    }

    [Theory]
    [InlineData("ESC005-lower", "Email")]
    [InlineData("ESC005-substring", "Sku")]
    [InlineData("ESC005-year", "OrderDate")]
    [InlineData("ESC005-upper", "LastName")]
    [InlineData("ESC006-suffix", "LastName")]
    [InlineData("ESC008-dateadd", "CreatedAt")]
    [InlineData("ESC008-maths", "Price")]
    [InlineData("ESC009", "AccountNumber")]
    [InlineData("ESC011", "Status")]
    public void Non_sargable_scans_and_rewrite_seeks(string rule, string col)
    {
        var bad = R($"{rule}:bad"); var good = R($"{rule}:good");
        var goodCol = rule == "ESC006-suffix" ? "ReversedLastName" : col;
        Assert.False(bad.SeeksOn(col)); Assert.True(bad.Scans);
        Assert.True(good.SeeksOn(goodCol));
        Assert.Equal(bad.Rows, good.Rows); // rewrite is equivalent on this data
        Assert.True(bad.LogicalReads > good.LogicalReads, $"reads bad={bad.LogicalReads} good={good.LogicalReads}");
    }

    [Fact] public void ESC006_contains_scans() => Assert.False(R("ESC006-contains:bad").SeeksOn("LastName"));

    [Fact] public void ESC006_prefix_like_seeks() => Assert.True(R("ESC006-prefix:clean").SeeksOn("LastName"));

    [Fact] // FLAG: EF parameterises EndsWith(term) -> LIKE @p; plan shows an Index Seek (LikeRangeStart/End over the whole index), reads match a scan
    public void ESC006_parameterised_EndsWith_is_a_full_range_seek()
    {
        var bad = R("ESC006-email:bad"); var scan = R("ESC005-lower:bad"); // same Customers index, true scan
        Assert.True(bad.SeeksOn("Email"));
        Assert.Equal(scan.LogicalReads, bad.LogicalReads);
    }

    [Fact] // A2 implicit conversion: raw SQL literal int vs varchar column - CONVERT_IMPLICIT on column, scan
    public void ESC009_int_literal_converts_varchar_column()
    {
        var r = R("ESC009-rawint:bad");
        Assert.True(r.HasConvertImplicitOn("AccountNumber")); Assert.False(r.SeeksOn("AccountNumber"));
    }

    [Fact] // FLAG: article says "often settles for a scan"; here the optimiser combined two seeks itself (index union)
    public void ESC010_or_across_columns_actually_index_unions_here()
    {
        var bad = R("ESC010:bad"); var good = R("ESC010:good");
        Assert.True(bad.SeeksOn("FirstName")); Assert.True(bad.SeeksOn("LastName")); Assert.False(bad.Scans && !bad.SeeksOn("FirstName"));
        Assert.True(good.SeeksOn("FirstName")); Assert.True(good.SeeksOn("LastName"));
        Assert.Equal(bad.LogicalReads, good.LogicalReads);
    }

    [Fact] // ESC012: closed end date misses 31 Dec rows after midnight (90-minute spacing -> 15 rows)
    public void ESC012_closed_range_returns_wrong_row_count()
    {
        var bad = R("ESC012:bad"); var good = R("ESC012:good");
        Assert.True(bad.SeeksOn("OrderDate"));
        Assert.Equal(15, good.Rows - bad.Rows);
    }

    [Fact] // A2 exception: CAST(dt AS date) still seeks (dynamic seek via GetRangeThroughConvert)
    public void CastToDate_still_seeks()
    {
        var r = R("ESC005-castdate:clean");
        Assert.True(r.SeeksOn("OrderDate")); Assert.True(r.HasGetRangeThroughConvert);
    }

    [Fact] // A1 collation: SQL_ collation scans; Windows collation does a dynamic seek with GetRangeThroughConvert
    public void Collation_SQL_scans_Windows_dynamic_seeks()
    {
        var sql = R("COLL-sql:bad"); var win = R("COLL-win:bad");
        Assert.True(sql.HasConvertImplicitOn("EmailSql")); Assert.False(sql.SeeksOn("EmailSql")); Assert.True(sql.HasPlanAffectingConvert("Seek Plan"));
        Assert.True(win.SeeksOn("EmailWin")); Assert.True(win.HasGetRangeThroughConvert); Assert.True(win.HasConvertImplicitOn("EmailWin"));
        Assert.False(win.HasPlanAffectingConvert("Cardinality Estimate")); // FLAG: article expects CE warnings on Windows collations; none here
        Assert.True(sql.LogicalReads > 20 * win.LogicalReads);
    }
}