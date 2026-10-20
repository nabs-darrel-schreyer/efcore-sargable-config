using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace EfSargable.IntegrationTests;

[CollectionDefinition(Name)]
public class SqlCollection : ICollectionFixture<SqlServerFixture> { public const string Name = "SqlServer"; }

/// <summary>One SQL Server 2022 container per test run. Schema from schema.json + predicate tables; 50k rows per table.</summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    public const int Rows = 50_000;
    readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
    public string ConnectionString { get; private set; } = "";
    public string ServerCollation { get; private set; } = "";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var master = _container.GetConnectionString();
        await Exec(master, "CREATE DATABASE Sargable");
        ConnectionString = new SqlConnectionStringBuilder(master) { InitialCatalog = "Sargable" }.ConnectionString;
        ServerCollation = (string)(await Scalar(ConnectionString, "SELECT CAST(SERVERPROPERTY('Collation') AS nvarchar(128))"))!;
        await Exec(ConnectionString, BuildDdl());
        await Exec(ConnectionString, Seed);
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public DbContextOptions<T> Options<T>() where T : DbContext =>
        new DbContextOptionsBuilder<T>().UseSqlServer(ConnectionString).Options;

    static string BuildDdl()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "schema.json")));
        var sb = new StringBuilder();
        foreach (var t in doc.RootElement.GetProperty("columns").EnumerateArray().GroupBy(c => c.GetProperty("table").GetString()!))
        {
            var cols = t.Select(c => (Name: c.GetProperty("column").GetString()!, Type: c.GetProperty("type").GetString()!,
                Len: c.GetProperty("length").GetInt32(), Indexed: c.GetProperty("indexed").GetBoolean())).ToList();
            sb.AppendLine($"CREATE TABLE {t.Key} (Id int IDENTITY PRIMARY KEY, " +
                string.Join(", ", cols.Select(c => $"[{c.Name}] {c.Type}({(c.Len < 0 ? "max" : c.Len)}) NULL")) + ");");
            foreach (var c in cols.Where(c => c.Indexed))
            {
                var inc = cols.Where(o => o.Name != c.Name).Select(o => $"[{o.Name}]").ToList();
                sb.AppendLine($"CREATE INDEX IX_{t.Key.Split('.')[1]}_{c.Name} ON {t.Key}([{c.Name}])" + (inc.Count > 0 ? $" INCLUDE({string.Join(",", inc)})" : "") + ";");
            }
        }
        // Predicate tables (article 2) and collation probe (article 1). Covering indexes so seek vs scan is about the predicate, not lookups.
        sb.AppendLine("""
            CREATE TABLE dbo.People (Id int IDENTITY PRIMARY KEY, FirstName nvarchar(100) NOT NULL, LastName nvarchar(100) NOT NULL,
                ReversedLastName AS CAST(REVERSE(LastName) AS nvarchar(100)) PERSISTED);
            CREATE INDEX IX_People_FirstName ON dbo.People(FirstName) INCLUDE(LastName, ReversedLastName);
            CREATE INDEX IX_People_LastName ON dbo.People(LastName) INCLUDE(FirstName, ReversedLastName);
            CREATE INDEX IX_People_ReversedLastName ON dbo.People(ReversedLastName) INCLUDE(FirstName, LastName);
            CREATE TABLE dbo.Sales (Id int IDENTITY PRIMARY KEY, OrderDate datetime2 NOT NULL, CreatedAt datetime2 NOT NULL,
                Price decimal(18,2) NOT NULL, Status varchar(10) NULL);
            CREATE INDEX IX_Sales_OrderDate ON dbo.Sales(OrderDate) INCLUDE(CreatedAt, Price, Status);
            CREATE INDEX IX_Sales_CreatedAt ON dbo.Sales(CreatedAt) INCLUDE(OrderDate, Price, Status);
            CREATE INDEX IX_Sales_Price ON dbo.Sales(Price) INCLUDE(OrderDate, CreatedAt, Status);
            CREATE INDEX IX_Sales_Status ON dbo.Sales(Status) INCLUDE(OrderDate, CreatedAt, Price);
            CREATE TABLE dbo.CollationProbe (Id int IDENTITY PRIMARY KEY,
                EmailSql varchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                EmailWin varchar(256) COLLATE Latin1_General_CI_AS NOT NULL);
            CREATE INDEX IX_CollationProbe_EmailSql ON dbo.CollationProbe(EmailSql);
            CREATE INDEX IX_CollationProbe_EmailWin ON dbo.CollationProbe(EmailWin);
            """);
        return sb.ToString();
    }

    const string Seed = """
        SET NOCOUNT ON;
        SELECT TOP (50000) n = ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) INTO #n FROM sys.all_objects a CROSS JOIN sys.all_objects b;
        INSERT dbo.Customers (Email, Notes) SELECT CONCAT('user', n, '@example.com'), NULL FROM #n;
        INSERT dbo.Products (Sku, Code) SELECT CONCAT(CHAR(65 + n % 26), CHAR(65 + (n / 26) % 26), CHAR(65 + (n / 676) % 26), '-', n), CONCAT('C', n) FROM #n;
        INSERT dbo.Orders (Reference) SELECT CONCAT('R', n) FROM #n;
        INSERT dbo.Translations ([Key], [Text]) SELECT CONCAT('k', n), CONCAT(N'текст', n) FROM #n;
        INSERT dbo.Accounts (AccountNumber) SELECT CAST(n AS varchar(20)) FROM #n;
        INSERT dbo.People (FirstName, LastName) SELECT CONCAT('F', n), CONCAT('L', n, CASE WHEN n % 1000 = 0 THEN 'son' ELSE '' END) FROM #n;
        INSERT dbo.Sales (OrderDate, CreatedAt, Price, Status)
            SELECT DATEADD(minute, n * 90, '2019-01-01'), DATEADD(minute, n * 90, '2019-01-01'), (n % 20000) / 100.0,
                   CASE WHEN n % 100 = 0 THEN NULL WHEN n % 100 = 1 THEN 'New' ELSE 'Done' END FROM #n;
        INSERT dbo.CollationProbe (EmailSql, EmailWin) SELECT CONCAT('user', n), CONCAT('user', n) FROM #n;
        EXEC sp_updatestats;
        """;

    static async Task Exec(string cs, string sql)
    {
        await using var c = new SqlConnection(cs); await c.OpenAsync();
        await using var cmd = new SqlCommand(sql, c) { CommandTimeout = 300 }; await cmd.ExecuteNonQueryAsync();
    }

    static async Task<object?> Scalar(string cs, string sql)
    {
        await using var c = new SqlConnection(cs); await c.OpenAsync();
        await using var cmd = new SqlCommand(sql, c); return await cmd.ExecuteScalarAsync();
    }
}