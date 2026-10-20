using Microsoft.EntityFrameworkCore;

namespace EfSargable.Sample.Shared;

public static class ContextFactory
{
    // No SQL Server is contacted: model building and ToQueryString() work offline.
    public const string FakeConnection = "Server=(none);Database=Sargable;Trusted_Connection=True;TrustServerCertificate=True";

    public static DbContextOptions<T> Options<T>() where T : DbContext =>
        new DbContextOptionsBuilder<T>().UseSqlServer(FakeConnection).Options;
}