using EfSargable.Sample.Shared;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Bad;

// The "blanket varchar(256)" global fix from evals.json case 3.
public class BadConventionDbContext(DbContextOptions<BadConventionDbContext> options) : DbContext(options)
{
    public DbSet<Translation> Translations => Set<Translation>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<string>().AreUnicode(false).HaveMaxLength(256); // BAD: ESC004-blanket-convention (forces real Unicode Text nvarchar(400) to varchar(256))
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Translation>(e =>
        {
            e.ToTable("Translations");
            e.Property(t => t.Key); // NEGATIVE: convention gives varchar(256) which matches schema - must NOT be flagged
            e.HasIndex(t => t.Key);
            e.HasIndex(t => t.Text);
        });
    }
}