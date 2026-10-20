using EfSargable.Sample.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EfSargable.Sample.Good;

public static class GoodPredicates
{
    public static IQueryable<Sale> YearAsHalfOpenRange(PredicateDbContext db) =>
        db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate < new DateTime(2026, 1, 1)); // GOOD: fixes ESC005 and ESC012

    public static IQueryable<Person> BareColumnEquals(PredicateDbContext db, string name) =>
        db.People.Where(p => p.LastName == name); // GOOD: fixes ESC005 (CI collation already ignores case)

    public static IQueryable<Person> SuffixViaReversedColumn(PredicateDbContext db) =>
        db.People.Where(p => p.ReversedLastName.StartsWith("nos")); // GOOD: fixes ESC006 (LIKE 'nos%' on indexed REVERSE computed column)

    public static IQueryable<Person> PrefixSearch(PredicateDbContext db) =>
        db.People.Where(p => p.LastName.StartsWith("Smi")); // NEGATIVE: LIKE 'Smi%' is sargable

    public static IQueryable<Sale> DateAddOnConstant(PredicateDbContext db, DateTime now) =>
        db.Sales.Where(s => s.CreatedAt > now.AddDays(-30)); // GOOD: fixes ESC008

    public static IQueryable<Sale> MathsOnConstant(PredicateDbContext db) =>
        db.Sales.Where(s => s.Price > 100m); // GOOD: fixes ESC008

    public static IQueryable<Account> StringVsVarchar(PredicateDbContext db) =>
        db.Accounts.Where(a => a.AccountNumber == "12345"); // GOOD: fixes ESC009

    public static IQueryable<Person> UnionPerColumn(PredicateDbContext db, string name) =>
        db.People.Where(p => p.FirstName == name).Union(db.People.Where(p => p.LastName == name)); // GOOD: fixes ESC010

    public static IQueryable<Sale> OrIsNull(PredicateDbContext db) =>
        db.Sales.Where(s => s.Status == "New" || s.Status == null); // GOOD: fixes ESC011

    public static IQueryable<Sale> CastToDate(PredicateDbContext db, DateTime d) =>
        db.Sales.Where(s => s.OrderDate.Date == d); // NEGATIVE: CAST(dt AS date) can still seek (optimiser rewrite) - mention, don't flag

    public static IQueryable<Translation> VarcharParamVsNvarcharColumn(PredicateDbContext db) =>
        db.Translations.FromSqlRaw("SELECT * FROM Translations WHERE Text = @p",
            new SqlParameter("@p", SqlDbType.VarChar, 400) { Value = "hello" }); // NEGATIVE: ESC007-reverse-unicode (parameter converted, not column - harmless)
}