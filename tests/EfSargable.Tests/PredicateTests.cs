using EfSargable.Sample.Bad;
using EfSargable.Sample.Good;
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfSargable.Tests;

// Each pair: BAD wraps the column (non-sargable), GOOD leaves it bare. Mapping is correct in both,
// proving these are "other-cause" findings a mapping change cannot fix.
public class PredicateTests
{
    static PredicateDbContext Db() => new(ContextFactory.Options<PredicateDbContext>());
    static readonly DateTime Now = new(2026, 10, 20);

    [Fact] public void ESC005_year_on_column_vs_half_open_range()
    {
        using var db = Db();
        Assert.Contains("DATEPART(year, [s].[OrderDate])", BadPredicates.YearOnColumn(db).ToQueryString());
        var g = GoodPredicates.YearAsHalfOpenRange(db).ToQueryString();
        Assert.Contains("[s].[OrderDate] >= '2025-01-01", g); Assert.Contains("[s].[OrderDate] < '2026-01-01", g);
    }

    [Fact] public void ESC005_upper_on_column_vs_bare()
    {
        using var db = Db();
        Assert.Contains("UPPER([p].[LastName])", BadPredicates.UpperOnColumn(db, "x").ToQueryString());
        var g = GoodPredicates.BareColumnEquals(db, "x").ToQueryString();
        Assert.Contains("WHERE [p].[LastName] = @", g); Assert.DoesNotContain("UPPER", g);
    }

    [Fact] public void ESC006_suffix_and_contains_vs_reversed_prefix()
    {
        using var db = Db();
        Assert.Contains("LIKE N'%son'", BadPredicates.SuffixSearch(db).ToQueryString());
        Assert.Contains("LIKE N'%son%'", BadPredicates.ContainsSearch(db).ToQueryString());
        Assert.Contains("[p].[ReversedLastName] LIKE N'nos%'", GoodPredicates.SuffixViaReversedColumn(db).ToQueryString());
        Assert.Equal("REVERSE([LastName])", db.Model.FindEntityType(typeof(Person))!.FindProperty("ReversedLastName")!.GetComputedColumnSql());
    }

    [Fact] public void Negative_prefix_like_is_sargable()
    {
        using var db = Db();
        Assert.Contains("[p].[LastName] LIKE N'Smi%'", GoodPredicates.PrefixSearch(db).ToQueryString());
    }

    [Fact] public void ESC008_dateadd_on_column_vs_on_parameter()
    {
        using var db = Db();
        Assert.Contains("[s].[CreatedAt]) > @", BadPredicates.DateAddOnColumn(db, Now).ToQueryString());
        var g = GoodPredicates.DateAddOnConstant(db, Now).ToQueryString();
        Assert.Contains("WHERE [s].[CreatedAt] > @", g); Assert.DoesNotContain("DATEADD", g);
    }

    [Fact] public void ESC008_maths_on_column_vs_constant()
    {
        using var db = Db();
        Assert.Contains("[s].[Price] * 1.1 > 110", BadPredicates.MathsOnColumn(db).ToQueryString());
        Assert.Contains("WHERE [s].[Price] > 100", GoodPredicates.MathsOnConstant(db).ToQueryString());
    }

    [Fact] public void ESC009_column_converted_to_int_vs_string_literal()
    {
        using var db = Db();
        Assert.Contains("CONVERT(int, [a].[AccountNumber]) = 12345", BadPredicates.NumberVsVarchar(db).ToQueryString());
        Assert.Contains("[a].[AccountNumber] = '12345'", GoodPredicates.StringVsVarchar(db).ToQueryString());
    }

    [Fact] public void ESC010_or_across_columns_vs_union()
    {
        using var db = Db();
        Assert.Contains("[p].[FirstName] = @name OR [p].[LastName] = @name", BadPredicates.OrAcrossColumns(db, "x").ToQueryString());
        var g = GoodPredicates.UnionPerColumn(db, "x").ToQueryString();
        Assert.Contains("\nUNION\n", g.Replace("\r", "")); Assert.DoesNotContain("UNION ALL", g); Assert.DoesNotContain(" OR ", g);
    }

    [Fact] public void ESC011_coalesce_on_column_vs_or_is_null()
    {
        using var db = Db();
        Assert.Contains("COALESCE([s].[Status], 'New')", BadPredicates.CoalesceOnColumn(db).ToQueryString());
        var g = GoodPredicates.OrIsNull(db).ToQueryString();
        Assert.Contains("[s].[Status] IS NULL OR [s].[Status] = 'New'", g); Assert.DoesNotContain("COALESCE", g);
    }

    [Fact] public void ESC012_closed_end_date_vs_half_open()
    {
        using var db = Db();
        Assert.Contains("<= '2025-12-31T00:00:00", BadPredicates.ClosedEndDate(db).ToQueryString());
        var g = GoodPredicates.YearAsHalfOpenRange(db).ToQueryString();
        Assert.Contains("< '2026-01-01", g); Assert.DoesNotContain("<=", g);
    }

    [Fact] public void Negative_cast_to_date_is_the_seekable_exception()
    {
        using var db = Db();
        Assert.Contains("CONVERT(date, [s].[OrderDate]) = @d", GoodPredicates.CastToDate(db, Now).ToQueryString());
    }

    [Fact] public void ESC007_reverse_case_sends_varchar_param_to_nvarchar_column()
    {
        using var db = Db();
        Assert.Equal("nvarchar(400)", db.Model.FindEntityType(typeof(Translation))!.FindProperty("Text")!.GetColumnType());
        Assert.Contains("DECLARE @p varchar(400)", GoodPredicates.VarcharParamVsNvarcharColumn(db).ToQueryString());
    }

    [Fact] public void Predicate_context_has_no_mapping_findings()
    {
        using var db = Db();
        Assert.Empty(MappingGuardrail.Check(db, Fx.Schema));
    }
}