using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace EfSargable.IntegrationTests;

[Collection(SqlCollection.Name), Trait("Category", "Integration")]
public class ObservationTests(SqlServerFixture fx, ITestOutputHelper output)
{
    /// <summary>Records what SQL Server actually did for every case (no assertions) to plan-observations.md.</summary>
    [Fact]
    public void Record_actual_plans()
    {
        var sb = new StringBuilder($"# Actual plans (SQL Server 2022, server collation {fx.ServerCollation}, {SqlServerFixture.Rows} rows)\n\n| Case | Observed |\n|---|---|\n");
        foreach (var (key, (col, run)) in new Cases(fx).All())
        {
            var r = run();
            sb.AppendLine($"| {key} | {r.Summary(col)} |");
        }
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "samples"))) dir = dir.Parent;
        File.WriteAllText(Path.Combine(dir!.FullName, "samples", "plan-observations.md"), sb.ToString());
        output.WriteLine(sb.ToString());
    }
}