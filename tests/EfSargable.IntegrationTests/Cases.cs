using System.Data;
using EfSargable.Sample.Bad;
using EfSargable.Sample.Good;
using EfSargable.Sample.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.IntegrationTests;

/// <summary>All BAD/GOOD sample queries, executed for real. Key = "{rule}:{bad|good}".</summary>
public sealed class Cases(SqlServerFixture fx)
{
    public const string Email = "user123@example.com";
    public static readonly DateTime Now = new(2027, 6, 1);

    T Ctx<T>() where T : DbContext => (T)Activator.CreateInstance(typeof(T), fx.Options<T>())!;
    PlanResult Run<T>(IQueryable<T> q) => PlanCapture.Run(q, fx.ConnectionString);

    public Dictionary<string, (string Column, Func<PlanResult> Run)> All() => new()
    {
        ["ESC001:bad"]  = ("Email", () => { var e = Email; using var d = Ctx<BadDbContext>(); return Run(d.Customers.Where(c => c.Email == e)); }),
        ["ESC001:good"] = ("Email", () => { var e = Email; using var d = Ctx<GoodDbContext>(); return Run(d.Customers.Where(c => c.Email == e)); }),
        ["ESC002:bad"]  = ("Sku", () => { var s = "TEA-123"; using var d = Ctx<BadDbContext>(); return Run(d.Products.Where(p => p.Sku == s)); }),
        ["ESC002:good"] = ("Sku", () => { var s = "TEA-123"; using var d = Ctx<GoodDbContext>(); return Run(d.Products.Where(p => p.Sku == s)); }),
        ["ESC003:bad"]  = ("Reference", () => { var r = "R123"; using var d = Ctx<BadDbContext>(); return Run(d.Orders.Where(o => o.Reference == r)); }),
        ["ESC003:good"] = ("Reference", () => { var r = "R123"; using var d = Ctx<GoodDbContext>(); return Run(d.Orders.Where(o => o.Reference == r)); }),
        ["ESC004:bad"]  = ("Text", () => { var t = "текст123"; using var d = Ctx<BadConventionDbContext>(); return Run(d.Translations.Where(x => x.Text == t)); }),
        ["ESC004:good"] = ("Text", () => { var t = "текст123"; using var d = Ctx<GoodDbContext>(); return Run(d.Translations.Where(x => x.Text == t)); }),
        ["ESC007:clean"] = ("Text", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.VarcharParamVsNvarcharColumn(d)); }),
        ["ESC005-lower:bad"] = ("Email", () => { using var d = Ctx<GoodDbContext>(); return Run(BadQueries.LowerOnColumn(d, Email)); }),
        ["ESC005-lower:good"] = ("Email", () => { using var d = Ctx<GoodDbContext>(); return Run(GoodQueries.EqualsOnColumn(d, Email)); }),
        ["ESC005-substring:bad"] = ("Sku", () => { using var d = Ctx<GoodDbContext>(); return Run(BadQueries.PrefixViaSubstring(d, "TEA")); }),
        ["ESC005-substring:good"] = ("Sku", () => { using var d = Ctx<GoodDbContext>(); return Run(GoodQueries.PrefixViaStartsWith(d, "TEA")); }),
        ["ESC005-year:bad"] = ("OrderDate", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.YearOnColumn(d)); }),
        ["ESC005-year:good"] = ("OrderDate", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.YearAsHalfOpenRange(d)); }),
        ["ESC005-upper:bad"] = ("LastName", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.UpperOnColumn(d, "L4242")); }),
        ["ESC005-upper:good"] = ("LastName", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.BareColumnEquals(d, "L4242")); }),
        ["ESC005-castdate:clean"] = ("OrderDate", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.CastToDate(d, new DateTime(2025, 6, 15))); }),
        ["ESC006-suffix:bad"] = ("LastName", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.SuffixSearch(d)); }),
        ["ESC006-contains:bad"] = ("LastName", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.ContainsSearch(d)); }),
        ["ESC006-suffix:good"] = ("ReversedLastName", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.SuffixViaReversedColumn(d)); }),
        ["ESC006-prefix:clean"] = ("LastName", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.PrefixSearch(d)); }),
        ["ESC006-email:bad"] = ("Email", () => { using var d = Ctx<GoodDbContext>(); return Run(BadQueries.LeadingWildcard(d, "r123@example.com")); }),
        ["ESC008-dateadd:bad"] = ("CreatedAt", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.DateAddOnColumn(d, Now)); }),
        ["ESC008-dateadd:good"] = ("CreatedAt", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.DateAddOnConstant(d, Now)); }),
        ["ESC008-maths:bad"] = ("Price", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.MathsOnColumn(d)); }),
        ["ESC008-maths:good"] = ("Price", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.MathsOnConstant(d)); }),
        ["ESC009:bad"] = ("AccountNumber", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.NumberVsVarchar(d)); }),
        ["ESC009:good"] = ("AccountNumber", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.StringVsVarchar(d)); }),
        ["ESC009-rawint:bad"] = ("AccountNumber", () => PlanCapture.Run(new SqlCommand("SELECT Id, AccountNumber FROM dbo.Accounts WHERE AccountNumber = 12345"), fx.ConnectionString)),
        ["ESC010:bad"] = ("FirstName", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.OrAcrossColumns(d, "F4242")); }),
        ["ESC010:good"] = ("FirstName", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.UnionPerColumn(d, "F4242")); }),
        ["ESC011:bad"] = ("Status", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.CoalesceOnColumn(d)); }),
        ["ESC011:good"] = ("Status", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.OrIsNull(d)); }),
        ["ESC012:bad"] = ("OrderDate", () => { using var d = Ctx<PredicateDbContext>(); return Run(BadPredicates.ClosedEndDate(d)); }),
        ["ESC012:good"] = ("OrderDate", () => { using var d = Ctx<PredicateDbContext>(); return Run(GoodPredicates.YearAsHalfOpenRange(d)); }),
        ["COLL-sql:bad"] = ("EmailSql", () => Coll("EmailSql")),
        ["COLL-win:bad"] = ("EmailWin", () => Coll("EmailWin")),
    };

    PlanResult Coll(string col)
    {
        var cmd = new SqlCommand($"SELECT Id, {col} FROM dbo.CollationProbe WHERE {col} = @e");
        cmd.Parameters.Add(new SqlParameter("@e", SqlDbType.NVarChar, 4000) { Value = "user123" });
        return PlanCapture.Run(cmd, fx.ConnectionString);
    }
}