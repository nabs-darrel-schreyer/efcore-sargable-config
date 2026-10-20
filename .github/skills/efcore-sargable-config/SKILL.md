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

## Output

1. **Write a Markdown report to a file** (default `sargable-report.md` in the repo root, or the path the user gives) using [report-template](./references/report-template.md): executive summary, findings table, one section per finding with before/after code and how to verify, exceptions, checklist, next steps.
2. **Summarise in chat**: counts by label and severity, any `correctness` findings, the top 3 fixes, and the report path.
3. **Keep the machine-readable block** for every finding (in the report appendix and/or chat), unchanged in shape so runs can be scored:

```
[ESC0NN-rule-name] <file>:<line>  <Entity.Property or query>
Label: confirmed | candidate | other-cause | intentional-exception
Severity: high | medium | low | info   Correctness: yes | no
Effective mapping: <type or n/a>   Column: <type/length/collation/index or unknown>
Evidence: <SQL/parameter/plan evidence or "not measured">
Fix: <minimal change or "none - exception">
Validate: <check to run>
Summary: <n> findings (confirmed x, candidate y, other-cause z, exception w; correctness c)
```

### Labels

- **confirmed**: actual plan or executed parameters prove the issue.
- **candidate**: static evidence only.
- **other-cause**: slow/scanning predicate not caused by string mapping (ESC005, ESC006, ESC008-ESC012).
- **intentional-exception**: deliberate (e.g. unindexed long Unicode text, ESC007 reverse case).

### Severity and correctness

- **Correctness: yes** when the code can return wrong results or fail, regardless of speed: ESC004 (Unicode text sent as varchar becomes `?`, wrong rows), ESC012 (closed end date misses rows after midnight), ESC003 when the model length is shorter than the column (truncation error), ESC009 when the conversion can fail on non-numeric rows. List these first; never label them as only a performance issue.
- **high**: any correctness finding, or a confirmed scan on a selective, hot, indexed lookup.
- **medium**: candidate mapping mismatch (ESC001/002) or non-sargable predicate on an indexed column, not yet measured.
- **low**: length-only mismatch with no correctness risk, unindexed column, or a pattern the optimiser may already handle (e.g. ESC010 index union).
- **info**: intentional exceptions and checks to run.

For CI, define accepted exceptions first; prefer a model-metadata test over an analyzer. Works standalone; no Microsoft Agent Framework required.