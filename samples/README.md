# Skill test harness
- `EfSargable.Sample/Bad` - seeded defects (`// BAD: <rule>`), `Good` - fixes, `// NEGATIVE` - must not be flagged.
- `schema.json` - simulated existing SQL Server columns. `expected-findings.json` - scoring manifest.
- Run: `dotnet test EfSargable.slnx`. No SQL Server needed; CONVERT_IMPLICIT / Seek-vs-Scan still needs a real SQL Server actual plan.