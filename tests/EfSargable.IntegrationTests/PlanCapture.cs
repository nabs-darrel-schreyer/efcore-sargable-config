using System.Data.Common;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfSargable.IntegrationTests;

public sealed record PlanResult(int Rows, long LogicalReads, XDocument Plan, string Sql)
{
    static readonly XNamespace Ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
    IEnumerable<XElement> RelOps => Plan.Descendants(Ns + "RelOp");
    IEnumerable<string> Scalars => Plan.Descendants(Ns + "ScalarOperator").Select(s => (string?)s.Attribute("ScalarString") ?? "");

    public IReadOnlyList<string> Operators => RelOps.Select(r => (string)r.Attribute("PhysicalOp")!).Distinct().ToList();

    /// <summary>True when an Index Seek / Clustered Index Seek has a Seek Predicate on the column.</summary>
    public bool SeeksOn(string column) => RelOps.Any(r => ((string)r.Attribute("PhysicalOp")!).Contains("Seek") &&
        r.Elements(Ns + "IndexScan").Elements(Ns + "SeekPredicates").Descendants(Ns + "ColumnReference").Any(c => (string?)c.Attribute("Column") == column));

    public bool Scans => RelOps.Any(r => ((string)r.Attribute("PhysicalOp")!).Contains("Scan"));
    public bool HasConvertImplicitOn(string column) => Scalars.Any(s => s.Contains("CONVERT_IMPLICIT") && s.Contains($"[{column}]"));
    public bool HasPlanAffectingConvert(string issue) => Plan.Descendants(Ns + "PlanAffectingConvert").Any(p => (string?)p.Attribute("ConvertIssue") == issue);
    public bool HasGetRangeThroughConvert => Scalars.Any(s => s.Contains("GetRangeThroughConvert"));
    public bool HasGetRangeWithMismatchedTypes => Scalars.Any(s => s.Contains("GetRangeWithMismatchedTypes"));

    public string Summary(string column) =>
        $"rows={Rows} reads={LogicalReads} ops=[{string.Join(", ", Operators)}] seekOn{column}={SeeksOn(column)} " +
        $"convertImplicit={HasConvertImplicitOn(column)} PAC(Seek)={HasPlanAffectingConvert("Seek Plan")} PAC(CE)={HasPlanAffectingConvert("Cardinality Estimate")} " +
        $"rangeThroughConvert={HasGetRangeThroughConvert} rangeMismatched={HasGetRangeWithMismatchedTypes}";
}

public static partial class PlanCapture
{
    /// <summary>Executes the exact command EF generates (same text, parameter types and sizes) with STATISTICS XML + IO on.</summary>
    public static PlanResult Run<T>(IQueryable<T> query, string connectionString)
    {
        using var efCmd = query.CreateDbCommand();
        var cmd = new SqlCommand(efCmd.CommandText);
        foreach (DbParameter p in efCmd.Parameters)
        {
            var sp = (SqlParameter)p;
            cmd.Parameters.Add(new SqlParameter(sp.ParameterName, sp.SqlDbType, sp.Size) { Value = sp.Value, Precision = sp.Precision, Scale = sp.Scale });
        }
        return Run(cmd, connectionString);
    }

    public static PlanResult Run(SqlCommand cmd, string connectionString)
    {
        using var conn = new SqlConnection(connectionString);
        long reads = 0;
        conn.InfoMessage += (_, e) => { foreach (Match m in LogicalReads().Matches(e.Message)) reads += long.Parse(m.Groups[1].Value); };
        conn.Open();
        using (var set = new SqlCommand("SET STATISTICS XML ON; SET STATISTICS IO ON;", conn)) set.ExecuteNonQuery();
        cmd.Connection = conn;
        int rows = 0; XDocument? plan = null;
        using (var r = cmd.ExecuteReader())
        {
            do
            {
                if (r.FieldCount == 1 && r.GetName(0).Contains("Showplan")) { while (r.Read()) plan = XDocument.Parse(r.GetString(0)); }
                else while (r.Read()) rows++;
            } while (r.NextResult());
        }
        return new PlanResult(rows, reads, plan ?? throw new InvalidOperationException("no plan"), cmd.CommandText);
    }

    [GeneratedRegex(@"logical reads (\d+)")] private static partial Regex LogicalReads();
}