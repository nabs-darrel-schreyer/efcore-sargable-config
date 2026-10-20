using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Good;

public static class GoodQueries
{
    public static IQueryable<Shared.Customer> EqualsOnColumn(GoodDbContext db, string email) =>
        db.Customers.Where(c => c.Email == email); // GOOD: fixes ESC005 (normalise the parameter; rely on CI collation)

    public static IQueryable<Shared.Product> PrefixViaStartsWith(GoodDbContext db, string prefix) =>
        db.Products.Where(p => p.Sku.StartsWith(prefix)); // GOOD: fixes ESC005 (LIKE 'abc%' is seekable)

    public static IQueryable<Shared.Customer> ExactMatchInsteadOfSuffix(GoodDbContext db, string email) =>
        db.Customers.Where(c => c.Email == email); // NEGATIVE: ESC006 has no mapping fix; suffix search needs a reversed computed column / full-text
}