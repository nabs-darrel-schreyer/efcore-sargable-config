using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Good;

// FIXED counterparts: every mapping matches schema.json. Nothing here should be flagged.
public class GoodDbContext(DbContextOptions<GoodDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Translation> Translations => Set<Translation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.Property(c => c.Email).IsUnicode(false).HasMaxLength(256); // GOOD: fixes ESC001
            e.HasIndex(c => c.Email);
            e.Property(c => c.Notes); // NEGATIVE: intentional-exception
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(p => p.Sku).IsUnicode(false).HasMaxLength(32); // GOOD: fixes ESC002
            e.HasIndex(p => p.Sku);
            e.Property(p => p.Code).HasColumnType("varchar(16)"); // NEGATIVE: store type alone is sufficient
            e.HasIndex(p => p.Code);
        });

        b.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(o => o.Reference).IsUnicode(false).HasMaxLength(20); // GOOD: fixes ESC003
            e.HasIndex(o => o.Reference);
        });

        b.Entity<Translation>(e =>
        {
            e.ToTable("Translations");
            e.Property(t => t.Key).IsUnicode(false).HasMaxLength(256); // GOOD: scoped mapping instead of ESC004 blanket convention
            e.HasIndex(t => t.Key);
            e.Property(t => t.Text).HasMaxLength(400); // NEGATIVE: real Unicode nvarchar(400) - must stay Unicode
            e.HasIndex(t => t.Text);
        });
    }
}