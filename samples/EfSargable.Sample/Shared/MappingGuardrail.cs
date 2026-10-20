using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfSargable.Sample.Shared;

public record SchemaColumn(string Table, string Column, string Type, int Length, bool Indexed, string? Exception);
public record Finding(string Rule, string Entity, string Property, string Effective, string Expected, string Label);

/// <summary>
/// Model-metadata guardrail as recommended by detection.md: compares the EFFECTIVE
/// relational mapping (GetColumnType/IsUnicode/GetMaxLength) with the existing schema.
/// Static evidence only => every finding is labelled "candidate".
/// </summary>
public static partial class MappingGuardrail
{
    public static IReadOnlyList<SchemaColumn> LoadSchema(string path)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.GetProperty("columns").EnumerateArray().Select(c => new SchemaColumn(
            c.GetProperty("table").GetString()!, c.GetProperty("column").GetString()!,
            c.GetProperty("type").GetString()!, c.GetProperty("length").GetInt32(),
            c.GetProperty("indexed").GetBoolean(),
            c.TryGetProperty("exception", out var ex) ? ex.GetString() : null)).ToList();
    }

    public static IReadOnlyList<Finding> Check(DbContext db, IReadOnlyList<SchemaColumn> schema)
    {
        var findings = new List<Finding>();
        foreach (var et in db.Model.GetEntityTypes())
        {
            var table = "dbo." + et.GetTableName();
            foreach (var p in et.GetProperties().Where(p => p.ClrType == typeof(string)))
            {
                var col = schema.FirstOrDefault(s => s.Table == table && s.Column == p.GetColumnName());
                if (col is null) continue; // unknown schema -> not reported here
                var store = p.GetColumnType();
                var type = StoreType().Match(store).Groups[1].Value.ToLowerInvariant();
                var expected = $"{col.Type}({(col.Length < 0 ? "max" : col.Length.ToString())})";
                if (string.Equals(store, expected, StringComparison.OrdinalIgnoreCase)) continue;

                string rule = type == col.Type ? "ESC003-length-mismatch"
                    : col.Type == "varchar" ? (p.GetMaxLength() is not null ? "ESC002-maxlength-without-unicode" : "ESC001-unicode-mismatch")
                    : "ESC004-blanket-convention"; // real Unicode column forced to varchar
                findings.Add(new Finding(rule, et.ClrType.Name, p.Name, store, expected, "candidate"));
            }
        }
        return findings;
    }

    [GeneratedRegex(@"^(\w+)")] private static partial Regex StoreType();
}