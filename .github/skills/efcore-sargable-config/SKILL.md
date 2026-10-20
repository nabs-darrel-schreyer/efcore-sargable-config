---
name: efcore-sargable-config
description: "Detect, verify, and fix EF Core SQL Server string mapping mismatches (varchar/nvarchar, Unicode, MaxLength) that can cause CONVERT_IMPLICIT, Index Scans, or slow string predicates. Use for EF Core model reviews, query-plan investigations, and CI mapping guardrails."
---

# EF Core Sargable Configuration

Use this workflow to separate potential mapping risks from proven SQL Server plan regressions. A missing annotation alone does not prove a scan.

## Inputs

- EF Core version, DbContext/model code, and the query or indexed string column under review.
- SQL Server schema (type, length, collation, index) and execution plan when available. Continue with a qualified static report when they are not.

## Workflow

1. Locate the owning DbContext, entity property, Fluent configuration, and any `ConfigureConventions` defaults. Check the effective mapping before suggesting a change; scaffolded code, attributes, value converters, and shared defaults can already configure it.
2. Inventory string properties and compare effective Unicode, store type, and max length with the intended column. Flag absent or inconsistent settings as **candidates**, especially indexed/filter/join columns and unbounded mappings. Do not require both annotations when another configuration already supplies the correct mapping. For a repeatable CI guardrail, use a Roslyn analyzer or a model-metadata check; see [detection](./references/detection.md).
3. Compare candidates with the actual schema: `varchar` vs `nvarchar`, declared length (including `max`), column collation, and index keys. Separate confirmed mismatches from properties with unknown schema. Distinguish a length mismatch from a Unicode conversion: neither a different length nor an unbounded property alone proves a column-side `CONVERT_IMPLICIT`.
4. Reproduce the relevant predicate and inspect EF SQL/parameter declarations plus an **actual** SQL Server execution plan when accessible. Look for a conversion on the indexed column, plan-affecting convert warnings, and Seek vs Scan with row counts. `ToQueryString()` is a preview, not proof of the runtime plan; see [patterns](./references/patterns.md) and [detection](./references/detection.md). Do not attribute every scan to string mappings.
5. Change the narrowest owning mapping to match the **existing** database column, using attributes or Fluent API as appropriate. Set a verified length rather than an arbitrary default; use a global pre-convention only when the schema and exceptions justify it. If the database type must change, treat it as a separately reviewed migration with data, index, and compatibility checks. See [fixes](./references/fixes.md).
6. Validate the effective EF model and generated parameter metadata, then rerun the representative query and compare actual plans/reads if database access exists. If access is unavailable, report static evidence and the exact checks needed to confirm impact.

## Output

Report property and code location, effective model mapping, column type/length/collation and index (or `unknown`), observed SQL/plan evidence (or `not measured`), proposed minimal fix, and validation result. Label findings **confirmed**, **candidate**, or **intentional exception**. Never silently change real Unicode columns or data to `varchar`.

For CI, first define accepted exceptions (Unicode, long text, unindexed strings); only promote a verified project policy to analyzer diagnostics. This skill works standalone or as the procedure behind an agent/CI check; it does not require Microsoft Agent Framework.