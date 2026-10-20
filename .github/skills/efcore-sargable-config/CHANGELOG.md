# Changelog

## 0.2.0 - 2026-10-20

- Add stable rule ids ESC001-ESC012 (`references/rules.md`) covering both articles: varchar/nvarchar mismatches and non-sargable predicates.
- Add fixed output format and the `other-cause` label for slow queries not caused by string mapping.
- Document EF default `nvarchar(450)` for keys/indexed columns, collation behaviour, reverse case, convention-overrides-annotations, plan-cache query, logging tip.
- Record SQL Server 2022 plan findings: parameterised LIKE full-range seek, optimiser index union for OR, ESC004 wrong results, no CE warning on Windows collation.
- Narrow description to the real scope; add evals 4-6 and new triggers.

## 0.1.0

- Add a schema-first EF Core SQL Server string-mapping investigation workflow, reference examples, and evaluation prompts.