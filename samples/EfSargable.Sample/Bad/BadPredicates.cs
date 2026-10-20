using EfSargable.Sample.Good;
using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Non-sargable predicates. Mapping is correct, so every finding here is label "other-cause".
public static class BadPredicates
{
    public static IQueryable<Sale> YearOnColumn(PredicateDbContext db) =>
        db.Sales.Where(s => s.OrderDate.Year == 2025); // BAD: ESC005-function-on-column (DATEPART(year, OrderDate) = 2025)

    public static IQueryable<Person> UpperOnColumn(PredicateDbContext db, string name) =>
        db.People.Where(p => p.LastName.ToUpper() == name); // BAD: ESC005-function-on-column (UPPER(LastName); pointless on a CI collation)

    public static IQueryable<Person> SuffixSearch(PredicateDbContext db) =>
        db.People.Where(p => p.LastName.EndsWith("son")); // BAD: ESC006-leading-wildcard (LIKE '%son')

    public static IQueryable<Person> ContainsSearch(PredicateDbContext db) =>
        db.People.Where(p => p.LastName.Contains("son")); // BAD: ESC006-leading-wildcard (LIKE '%son%')

    public static IQueryable<Sale> DateAddOnColumn(PredicateDbContext db, DateTime now) =>
        db.Sales.Where(s => s.CreatedAt.AddDays(30) > now); // BAD: ESC008-arithmetic-on-column (DATEADD(day, 30, CreatedAt))

    public static IQueryable<Sale> MathsOnColumn(PredicateDbContext db) =>
        db.Sales.Where(s => s.Price * 1.1m > 110m); // BAD: ESC008-arithmetic-on-column (Price * 1.1 > 110)

    public static IQueryable<Account> NumberVsVarchar(PredicateDbContext db) =>
        db.Accounts.Where(a => Convert.ToInt32(a.AccountNumber) == 12345); // BAD: ESC009-implicit-conversion (varchar column converted to int)

    public static IQueryable<Person> OrAcrossColumns(PredicateDbContext db, string name) =>
        db.People.Where(p => p.FirstName == name || p.LastName == name); // BAD: ESC010-or-across-columns

    public static IQueryable<Sale> CoalesceOnColumn(PredicateDbContext db) =>
        db.Sales.Where(s => (s.Status ?? "New") == "New"); // BAD: ESC011-isnull-on-column (COALESCE(Status, 'New'))

    public static IQueryable<Sale> ClosedEndDate(PredicateDbContext db) =>
        db.Sales.Where(s => s.OrderDate >= new DateTime(2025, 1, 1) && s.OrderDate <= new DateTime(2025, 12, 31)); // BAD: ESC012-closed-date-range (misses rows after midnight 31 Dec)
}