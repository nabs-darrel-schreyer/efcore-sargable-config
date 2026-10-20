using EfSargable.Sample.Good;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Non-mapping causes of scans. These run against the FIXED mapping on purpose:
// the skill must report them as "other reason" and must NOT propose a mapping change.
public static class BadQueries
{
    public static IQueryable<Shared.Customer> LowerOnColumn(GoodDbContext db, string email) =>
        db.Customers.Where(c => c.Email.ToLower() == email); // BAD: ESC005-function-on-column (LOWER([c].[Email]) - mapping fix cannot help)

    public static IQueryable<Shared.Product> PrefixViaSubstring(GoodDbContext db, string prefix) =>
        db.Products.Where(p => p.Sku.Substring(0, 3) == prefix); // BAD: ESC005-function-on-column (SUBSTRING on column)

    public static IQueryable<Shared.Customer> LeadingWildcard(GoodDbContext db, string term) =>
        db.Customers.Where(c => c.Email.EndsWith(term)); // BAD: ESC006-leading-wildcard (LIKE '%term' - mapping fix cannot make it a seek)
}