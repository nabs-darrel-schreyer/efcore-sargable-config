using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Good;

// Correct mappings; used to show predicate rules are NOT fixed by mapping.
public class PredicateDbContext(DbContextOptions<PredicateDbContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Translation> Translations => Set<Translation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Person>(e =>
        {
            e.ToTable("People");
            e.Property(p => p.FirstName).HasMaxLength(100);
            e.Property(p => p.LastName).HasMaxLength(100);
            e.Property(p => p.ReversedLastName).HasMaxLength(100).HasComputedColumnSql("REVERSE([LastName])", stored: true); // GOOD: fixes ESC006 (indexed REVERSE computed column)
            e.HasIndex(p => p.FirstName); e.HasIndex(p => p.LastName); e.HasIndex(p => p.ReversedLastName);
        });
        b.Entity<Sale>(e =>
        {
            e.ToTable("Sales");
            e.Property(s => s.Price).HasPrecision(18, 2);
            e.Property(s => s.Status).IsUnicode(false).HasMaxLength(10);
            e.HasIndex(s => s.OrderDate); e.HasIndex(s => s.CreatedAt); e.HasIndex(s => s.Price); e.HasIndex(s => s.Status);
        });
        b.Entity<Account>(e =>
        {
            e.ToTable("Accounts");
            e.Property(a => a.AccountNumber).IsUnicode(false).HasMaxLength(20);
            e.HasIndex(a => a.AccountNumber);
        });
        b.Entity<Translation>(e =>
        {
            e.ToTable("Translations");
            e.Property(t => t.Key).IsUnicode(false).HasMaxLength(256);
            e.Property(t => t.Text).HasMaxLength(400);
            e.HasIndex(t => t.Text);
        });
    }
}