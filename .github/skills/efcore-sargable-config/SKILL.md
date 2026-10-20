---
name: efcore-sargable-config
description: "Find why an indexed SQL Server lookup from EF Core scans instead of seeks: varchar/nvarchar and MaxLength mapping mismatches (CONVERT_IMPLICIT), and non-sargable predicates in LINQ or SQL (functions, arithmetic, conversions, leading wildcards, OR across columns, ISNULL/COALESCE on columns). Use for EF Core model reviews, query-plan investigations, and CI mapping guardrails. Not for general EF Core performance (N+1, tracking), collation changes, or non-SQL Server plans."
---

# EF Core Sargable Configuration

Rule of thumb: keep the indexed column bare on its side of the comparison, and make the model describe the real column. A missing annotation or a non-sargable shape is a **candidate** until an actual plan proves the scan. Not every scan is a bug.

## Inputs

- EF Core version, DbContext/model code, the slow query (LINQ and/or SQL), and the indexed column(s) involved.
- SQL Server schema (type, length, collation, index), server collation (`SELECT SERVERPROPERTY('Collation')`), and an actual plan when available. Continue with a qualified static report when they are not.

## Workflow

1. **Locate the effective mapping.** Find the DbContext, entity, Fluent configuration, attributes, and `ConfigureConventions`. Resolve the effective column type before judging (see [detection](./references/detection.md)).
2. **Mapping rules (ESC001–ESC004, ESC007).** Compare each relevant string property with the real column. Do not require both annotations when another configuration already supplies the correct mapping. See [rules](./references/rules.md).
3. **Predicate rules (ESC005, ESC006, ESC008–ESC012).** Read the LINQ and translated SQL (`ToQueryString()`) for column-side functions, arithmetic, conversions, leading wildcards, OR across columns, and ISNULL/COALESCE. These are **other-cause** findings: a mapping change will not fix them.
4. **Confirm.** With database access, check executed parameter metadata and an **actual** plan: `CONVERT_IMPLICIT`, `PlanAffectingConvert`, Seek vs Scan, Predicate vs Seek Predicates, logical reads vs rows returned. `ToQueryString()` is a preview, not plan proof. See [patterns](./references/patterns.md).
5. **Fix narrowly.** Change the owning mapping to match the **existing** column, or rewrite the predicate so the column is bare. Never flip real Unicode data to `varchar`. Treat any column type change as a reviewed migration. See [fixes](./references/fixes.md).
6. **Validate.** Re-check the effective model and generated SQL; re-run the query and compare plans/reads when possible. Otherwise list the exact checks still needed.

## Output (fixed format)

One block per finding, then a summary line:

```
[ESC0NN-rule-name] <file>:<line>  <Entity.Property or query>
Label: confirmed | candidate | other-cause | intentional-exception
Effective mapping: <type or n/a>   Column: <type/length/collation/index or unknown>
Evidence: <SQL/parameter/plan evidence or "not measured">
Fix: <minimal change or "none - exception">
Validate: <check to run>
Summary: <n> findings (confirmed x, candidate y, other-cause z, exception w)
```

- **confirmed**: actual plan or executed parameters prove the issue.
- **candidate**: static evidence only.
- **other-cause**: slow/scanning predicate not caused by string mapping (ESC005, ESC006, ESC008–ESC012).
- **intentional-exception**: deliberate (e.g. unindexed long Unicode text, ESC007 reverse case).

For CI, define accepted exceptions first; prefer a model-metadata test over an analyzer. Works standalone; no Microsoft Agent Framework required.