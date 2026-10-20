using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// Seeded BAD mappings. Each finding is marked with  // BAD: <rule-id>
public class BadDbContext(DbContextOptions<BadDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.Property(c => c.Email); // BAD: ESC001-unicode-mismatch (no config + index -> nvarchar(450); column is varchar(256) indexed)
            e.HasIndex(c => c.Email);
            e.Property(c => c.Notes); // NEGATIVE: intentional-exception (nvarchar(max) matches schema, unindexed long text)
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(p => p.Sku).HasMaxLength(32); // BAD: ESC002-maxlength-without-unicode (HasMaxLength alone -> nvarchar(32); column varchar(32))
            e.HasIndex(p => p.Sku);
            e.Property(p => p.Code).HasColumnType("varchar(16)"); // NEGATIVE: store type alone supplies varchar(16) - must NOT be flagged
            e.HasIndex(p => p.Code);
        });

        b.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(o => o.Reference).IsUnicode(false); // BAD: ESC003-length-mismatch (varchar(max) vs varchar(20); NOT a Unicode conversion)
        });
    }
}