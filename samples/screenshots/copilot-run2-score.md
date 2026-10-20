# Copilot CLI Session

> [!NOTE]
> - **Session ID:** `c90fc23e-a8f3-4792-93ff-34889b8ee584`  
> - **Started:** 20/10/2026, 6:58:52 pm  
> - **Duration:** 2m 7s  
> - **Exported:** 20/10/2026, 7:00:59 pm  

---

<sub>7s</sub>

### User

Score samples/screenshots/copilot-run1.log (findings from an efcore-sargable-config skill run) against samples/expected-findings.json. A true positive is a finding whose file and line match a should_flag=true entry with the same rule id (also check the label). A false negative is a should_flag=true entry with no matching finding. A false positive is a finding on a should_flag=false entry, except an intentional-exception label on that entry's rule (e.g. ESC007), which counts as correct; allow +/-1 line for multi-line statements. Print a table of each manifest entry with its result, then TP, FN, FP, label accuracy, precision and recall. Do not edit any files.

---

<sub>8s</sub>

### Info

Remote index: queued nabs-darrel-schreyer/efcore-sargable-config for indexing

---

<sub>12s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config

<details>
<summary>7 lines</summary>

```
.git
.github
.gitignore
EfSargable.slnx
README.md
samples
tests
```

</details>


---

<sub>16s</sub>

### `glob`

**samples/****

"samples/**"

<details>
<summary>47 matches</summary>

```
[glob: 42 path(s) under ./samples; grouped by directory]

./samples/EfSargable.Sample/ (37 path(s))
  Bad/BadConventionDbContext.cs
  Bad/BadDbContext.cs
  Bad/BadPredicates.cs
  Bad/BadQueries.cs
  EfSargable.Sample.csproj
  Good/GoodDbContext.cs
  Good/GoodPredicates.cs
  Good/GoodQueries.cs
  Good/PredicateDbContext.cs
  Shared/ContextFactory.cs
  Shared/Entities.cs
  Shared/MappingGuardrail.cs
  Shared/PredicateEntities.cs
  bin/Debug/net10.0/EfSargable.Sample.deps.json
  bin/Debug/net10.0/EfSargable.Sample.dll
  bin/Debug/net10.0/EfSargable.Sample.pdb
  bin/Debug/net10.0/schema.json
  obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs
  obj/Debug/net10.0/EfSargable.Sample.AssemblyInfo.cs
  obj/Debug/net10.0/EfSargable.Sample.AssemblyInfoInputs.cache
  obj/Debug/net10.0/EfSargable.Sample.GeneratedMSBuildEditorConfig.editorconfig
  obj/Debug/net10.0/EfSargable.Sample.GlobalUsings.g.cs
  obj/Debug/net10.0/EfSargable.Sample.assets.cache
  obj/Debug/net10.0/EfSargable.Sample.csproj.AssemblyReference.cache
  obj/Debug/net10.0/EfSargable.Sample.csproj.CoreCompileInputs.cache
  obj/Debug/net10.0/EfSargable.Sample.csproj.FileListAbsolute.txt
  obj/Debug/net10.0/EfSargable.Sample.dll
  obj/Debug/net10.0/EfSargable.Sample.pdb
  obj/Debug/net10.0/EfSargable.Sample.sourcelink.json
  obj/Debug/net10.0/ref/EfSargable.Sample.dll
  obj/Debug/net10.0/refint/EfSargable.Sample.dll
  obj/EfSargable.Sample.csproj.nuget.dgspec.json
  obj/EfSargable.Sample.csproj.nuget.g.props
  obj/EfSargable.Sample.csproj.nuget.g.targets
  obj/project.assets.json
  obj/project.nuget.cache
  schema.json

./samples/screenshots/ (3 path(s))
  copilot-run1.log
  copilot-run1.md
  copilot-run2-score.log

./samples/ (2 path(s))
  README.md
  expected-findings.json

[extensions: .cs=15, .json=7, .cache=5, .dll=4, .md=2, .log=2, .pdb=2, .csproj=1]
```

</details>


---

<sub>23s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\expected-findings.json.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\expected-findings.json

<details>
<summary>342 lines</summary>

```
{
    "_comment":  "Generated from // BAD / // GOOD / // NEGATIVE markers. Score: TP = flag on should_flag=true line with matching rule id (and label); FN = missed; FP = any flag on should_flag=false. Rule catalogue: .github/skills/efcore-sargable-config/references/rules.md",
    "schema":  "samples/EfSargable.Sample/schema.json",
    "findings":  [
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadConventionDbContext.cs",
                         "line":  13,
                         "rule":  "ESC004-blanket-convention",
                         "should_flag":  true,
                         "label":  "candidate",
                         "note":  "(forces real Unicode Text nvarchar(400) to varchar(256))"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadConventionDbContext.cs",
                         "line":  21,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "convention gives varchar(256) which matches schema - must NOT be flagged"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadDbContext.cs",
                         "line":  18,
                         "rule":  "ESC001-unicode-mismatch",
                         "should_flag":  true,
                         "label":  "candidate",
                         "note":  "(no config + index -\u003e nvarchar(450); column is varchar(256) indexed)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadDbContext.cs",
                         "line":  20,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "intentional-exception (nvarchar(max) matches schema, unindexed long text)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadDbContext.cs",
                         "line":  26,
                         "rule":  "ESC002-maxlength-without-unicode",
                         "should_flag":  true,
                         "label":  "candidate",
                         "note":  "(HasMaxLength alone -\u003e nvarchar(32); column varchar(32))"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadDbContext.cs",
                         "line":  28,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "store type alone supplies varchar(16) - must NOT be flagged"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadDbContext.cs",
                         "line":  35,
                         "rule":  "ESC003-length-mismatch",
                         "should_flag":  true,
                         "label":  "candidate",
                         "note":  "(varchar(max) vs varchar(20); NOT a Unicode conversion)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  11,
                         "rule":  "ESC005-function-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(DATEPART(year, OrderDate) = 2025)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  14,
                         "rule":  "ESC005-function-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(UPPER(LastName); pointless on a CI collation)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  17,
                         "rule":  "ESC006-leading-wildcard",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(LIKE \u0027%son\u0027)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  20,
                         "rule":  "ESC006-leading-wildcard",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(LIKE \u0027%son%\u0027)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  23,
                         "rule":  "ESC008-arithmetic-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(DATEADD(day, 30, CreatedAt))"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  26,
                         "rule":  "ESC008-arithmetic-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(Price * 1.1 \u003e 110)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  29,
                         "rule":  "ESC009-implicit-conversion",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(varchar column converted to int)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  32,
                         "rule":  "ESC010-or-across-columns",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  ""
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  35,
                         "rule":  "ESC011-isnull-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(COALESCE(Status, \u0027New\u0027))"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadPredicates.cs",
                         "line":  38,
                         "rule":  "ESC012-closed-date-range",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(misses rows after midnight 31 Dec)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadQueries.cs",
                         "line":  11,
                         "rule":  "ESC005-function-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(LOWER([c].[Email]) - mapping fix cannot help)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadQueries.cs",
                         "line":  14,
                         "rule":  "ESC005-function-on-column",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(SUBSTRING on column)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Bad/BadQueries.cs",
                         "line":  17,
                         "rule":  "ESC006-leading-wildcard",
                         "should_flag":  true,
                         "label":  "other-cause",
                         "note":  "(LIKE \u0027%term\u0027 - mapping fix cannot make it a seek)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  19,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC001"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  21,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "intentional-exception"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  27,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC002"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  29,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "store type alone is sufficient"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  36,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC003"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  43,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "scoped mapping instead of ESC004 blanket convention"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodDbContext.cs",
                         "line":  45,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "real Unicode nvarchar(400) - must stay Unicode"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  11,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC005 and ESC012"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  14,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC005 (CI collation already ignores case)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  17,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC006 (LIKE \u0027nos%\u0027 on indexed REVERSE computed column)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  20,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "LIKE \u0027Smi%\u0027 is sargable"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  23,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC008"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  26,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC008"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  29,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC009"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  32,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC010"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  35,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC011"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  38,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "CAST(dt AS date) can still seek (optimiser rewrite) - mention, don\u0027t flag"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodPredicates.cs",
                         "line":  42,
                         "rule":  "ESC007-reverse-unicode",
                         "should_flag":  false,
                         "kind":  "ESC007-reverse-unicode",
                         "note":  null
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodQueries.cs",
                         "line":  8,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC005 (normalise the parameter; rely on CI collation)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodQueries.cs",
                         "line":  11,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC005 (LIKE \u0027abc%\u0027 is seekable)"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/GoodQueries.cs",
                         "line":  14,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "NEGATIVE",
                         "note":  "ESC006 has no mapping fix; suffix search needs a reversed computed column / full-text"
                     },
                     {
                         "file":  "samples/EfSargable.Sample/Good/PredicateDbContext.cs",
                         "line":  21,
                         "rule":  "none",
                         "should_flag":  false,
                         "kind":  "GOOD",
                         "note":  "fixes ESC006 (indexed REVERSE computed column)"
                     }
                 ]
}
```

</details>


---

<sub>21s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log

```
File too large to read at once (30.0 KB). Consider using the grep tool to search within the file, or view with view_range to read specific sections.
```


---

<sub>27s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log (lines 1-400)

<details>
<summary>182 lines</summary>

```
��� � �   s k i l l ( e f c o r e - s a r g a b l e - c o n f i g )  
  
 � � �   L i s t   d i r e c t o r y   s a m p l e s \ E f S a r g a b l e . S a m p l e  
     � � �   7   f i l e s   f o u n d  
  
 � � �   R e a d   p a t t e r n s . m d  
     � � �   . g i t h u b \ s k i l l s \ e f c o r e - s a r g a b l e - c o n f i g \ r e f e r e n c e s \ p a t t e r n s . m d  
     � � �   2 1   l i n e s   r e a d  
  
 � � �   R e a d   d e t e c t i o n . m d                                                                                                                                                               5 s  
     � � �   . g i t h u b \ s k i l l s \ e f c o r e - s a r g a b l e - c o n f i g \ r e f e r e n c e s \ d e t e c t i o n . m d  
     � � �   5 9   l i n e s   r e a d  
  
 � � �   R e a d   r u l e s . m d                                                                                                                                                                       7 s  
     � � �   . g i t h u b \ s k i l l s \ e f c o r e - s a r g a b l e - c o n f i g \ r e f e r e n c e s \ r u l e s . m d  
     � � �   2 9   l i n e s   r e a d  
  
 � � �   R e a d   f i x e s . m d                                                                                                                                                                       8 s  
     � � �   . g i t h u b \ s k i l l s \ e f c o r e - s a r g a b l e - c o n f i g \ r e f e r e n c e s \ f i x e s . m d  
     � � �   4 9   l i n e s   r e a d  
  
 � � �   L i s t   d i r e c t o r y   s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d  
     � � �   4   f i l e s   f o u n d  
  
 � � �   L i s t   d i r e c t o r y   s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d  
     � � �   4   f i l e s   f o u n d  
  
 � � �   R e a d   s c h e m a . j s o n                                                                                                                                                                 5 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ s c h e m a . j s o n  
     � � �   1 3   l i n e s   r e a d  
  
 � � �   L i s t   d i r e c t o r y   s a m p l e s \ E f S a r g a b l e . S a m p l e \ S h a r e d                                                                                                   7 s  
     � � �   4   f i l e s   f o u n d  
  
 N o w   r e a d i n g   a l l   t h e   r e l e v a n t   C #   s o u r c e   f i l e s   i n   p a r a l l e l .  
  
 � � �   R e a d   B a d C o n v e n t i o n D b C o n t e x t . c s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d C o n v e n t i o n D b C o n t e x t . c s  
     � � �   2 6   l i n e s   r e a d  
  
 � � �   R e a d   B a d D b C o n t e x t . c s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d D b C o n t e x t . c s  
     � � �   3 8   l i n e s   r e a d  
  
 � � �   R e a d   B a d Q u e r i e s . c s                                                                                                                                                             6 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d Q u e r i e s . c s  
     � � �   1 8   l i n e s   r e a d  
  
 � � �   R e a d   G o o d P r e d i c a t e s . c s                                                                                                                                                     8 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d P r e d i c a t e s . c s  
     � � �   4 3   l i n e s   r e a d  
  
 � � �   R e a d   B a d P r e d i c a t e s . c s                                                                                                                                                     1 0 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d P r e d i c a t e s . c s  
     � � �   3 9   l i n e s   r e a d  
  
 � � �   R e a d   E n t i t i e s . c s                                                                                                                                                               1 1 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ S h a r e d \ E n t i t i e s . c s  
     � � �   7   l i n e s   r e a d  
  
 � � �   R e a d   G o o d D b C o n t e x t . c s                                                                                                                                                     1 3 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d D b C o n t e x t . c s  
     � � �   4 9   l i n e s   r e a d  
  
 � � �   R e a d   G o o d Q u e r i e s . c s                                                                                                                                                         1 6 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d Q u e r i e s . c s  
     � � �   1 5   l i n e s   r e a d  
  
 � � �   R e a d   P r e d i c a t e D b C o n t e x t . c s                                                                                                                                           1 9 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ P r e d i c a t e D b C o n t e x t . c s  
     � � �   4 5   l i n e s   r e a d  
  
 � � �   R e a d   M a p p i n g G u a r d r a i l . c s                                                                                                                                               2 1 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ S h a r e d \ M a p p i n g G u a r d r a i l . c s  
     � � �   5 3   l i n e s   r e a d  
  
 � � �   R e a d   P r e d i c a t e E n t i t i e s . c s                                                                                                                                             2 3 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ S h a r e d \ P r e d i c a t e E n t i t i e s . c s  
     � � �   6   l i n e s   r e a d  
  
 � � �   R e a d   C o n t e x t F a c t o r y . c s                                                                                                                                                   2 5 s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ S h a r e d \ C o n t e x t F a c t o r y . c s  
     � � �   1 2   l i n e s   r e a d  
  
 � � �   R e a d   B a d D b C o n t e x t . c s  
     � � �   s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d D b C o n t e x t . c s  
     � � �   L 1 : 1 0   ( 1 0   l i n e s   r e a d )  
  
 � � �   S e a r c h   ( g r e p )  
     � � �   " . "   ( [ " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d D b C o n  
     � � �   t e x t . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d C o  
     � � �   n v e n t i o n D b C o n t e x t . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e . S a m  
     � � �   p l e \ B a d \ B a d P r e d i c a t e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e  
     � � �   . S a m p l e \ B a d \ B a d Q u e r i e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l  
     � � �   e . S a m p l e \ G o o d \ G o o d D b C o n t e x t . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a  
     � � �   r g a b l e . S a m p l e \ G o o d \ G o o d P r e d i c a t e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e  
     � � �   s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d Q u e r i e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m  
     � � �   p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ P r e d i c a t e D b C o n t e x t . c s " ] )  
     � � �   S e a r c h   p a t h s   d o   n o t   e x i s t :   [ " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b  
         l e . S a m p l e \ B a d \ B a d D b C o n t e x t . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r  
         g a b l e . S a m p l e \ B a d \ B a d C o n v e n t i o n D b C o n t e x t . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \  
         s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d P r e d i c a t e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n  
         f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ B a d \ B a d Q u e r i e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o  
         n f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d D b C o n t e x t . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b  
         l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d P r e d i c a t e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e -  
         s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ G o o d Q u e r i e s . c s " , " C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o  
         r e - s a r g a b l e - c o n f i g \ s a m p l e s \ E f S a r g a b l e . S a m p l e \ G o o d \ P r e d i c a t e D b C o n t e x t . c s " ]  
  
 /   S e a r c h   ( g r e p )  
     � � �   " . "   ( C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d D b C o n t e  
     � � �   x t . c s ,   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d C o n v e  
     � � �   n t i o n D b C o n t e x t . c s ,   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e /  
     � � �   B a d / B a d P r e d i c a t e s . c s ,  
     � � �   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d Q u e r i e s . c s ,  
     � � �   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / G o o d / G o o d D b C o n t e x t .  
     � � �   c s ,   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / G o o d / G o o d P r e d i c  
     � � �   a t e s . c s ,   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / G o o d / G o o d Q  
     � � �   u e r i e s . c s ,   C : / D e v / n a b s - d a r r e l - s c h r e y e r / e f c o r e - s a r g a b l e - c o n f i g / s a m p l e s / E f S a r g a b l e . S a m p l e / G o o d / P r e  
     � � �   d i c a t e D b C o n t e x t . c s )  
     � � �   2 4 1   l i n e s   f o u n d  
  
 F i n d i n g s   f r o m   s t a t i c   r e v i e w   ( n o   D B   a c c e s s ;   L I N Q / a t t r i b u t e s   c o m p a r e d   a g a i n s t   ` s c h e m a . j s o n ` ) .  
  
 ` ` `  
 [ E S C 0 0 1 - u n i c o d e - m i s m a t c h ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d D b C o n t e x t . c s : 1 8     C u s t o m e r . E m a i l  
 L a b e l :   c a n d i d a t e  
 E f f e c t i v e   m a p p i n g :   n v a r c h a r ( 4 5 0 )   ( n o   U n i c o d e / M a x L e n g t h   s e t ,   p r o p e r t y   i s   i n d e x e d )       C o l u m n :   v a r c h a r ( 2 5 6 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   C O N V E R T _ I M P L I C I T ( n v a r c h a r ( 4 5 0 ) ,   [ c ] . [ E m a i l ] ,   0 )   a g a i n s t   t h e   v a r c h a r   c o l u m n   ( d a t a   t y p e   p r e c e d e n c e )  
 F i x :   . I s U n i c o d e ( f a l s e ) . H a s M a x L e n g t h ( 2 5 6 )  
 V a l i d a t e :   r e - c h e c k   G e t C o l u m n T y p e ( ) / I s U n i c o d e ( )   m e t a d a t a ;   c o n f i r m   C O N V E R T _ I M P L I C I T   d i s a p p e a r s   f r o m   t h e   a c t u a l   p l a n  
  
 [ E S C 0 0 2 - m a x l e n g t h - w i t h o u t - u n i c o d e ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d D b C o n t e x t . c s : 2 6     P r o d u c t . S k u  
 L a b e l :   c a n d i d a t e  
 E f f e c t i v e   m a p p i n g :   n v a r c h a r ( 3 2 )   ( H a s M a x L e n g t h   s e t ,   U n i c o d e   d e f a u l t s   t r u e )       C o l u m n :   v a r c h a r ( 3 2 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   H a s M a x L e n g t h   a l o n e   d o e s   n o t   s e t   I s U n i c o d e ( f a l s e )  
 F i x :   a d d   . I s U n i c o d e ( f a l s e )   ( k e e p   H a s M a x L e n g t h ( 3 2 ) )  
 V a l i d a t e :   m e t a d a t a   c h e c k ;   c o n f i r m   S e e k   o n   S k u   i n   a n   a c t u a l   p l a n  
  
 [ E S C 0 0 3 - l e n g t h - m i s m a t c h ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d D b C o n t e x t . c s : 3 5     O r d e r . R e f e r e n c e  
 L a b e l :   c a n d i d a t e  
 E f f e c t i v e   m a p p i n g :   v a r c h a r ( m a x )   ( I s U n i c o d e ( f a l s e )   s e t ,   n o   M a x L e n g t h )       C o l u m n :   v a r c h a r ( 2 0 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   s a m e   t y p e   f a m i l y   ( v a r c h a r ) ,   s o   n o   c o l u m n - s i d e   c o n v e r s i o n   e x p e c t e d   � � �   r i s k   i s   o v e r s i z e d   m e m o r y   g r a n t   /   c o l u m n   u n u s a b l e   a s   a n   i n d e x   k e y   a t   m a x  
 F i x :   a d d   . H a s M a x L e n g t h ( 2 0 )  
 V a l i d a t e :   m e t a d a t a   c h e c k   f o r   G e t M a x L e n g t h ( ) = = 2 0 ;   c o n f i r m   n o   v a l u e s   a r e   b e i n g   s i l e n t l y   a f f e c t e d   b y   t h e   l e n g t h   c h a n g e  
  
 [ E S C 0 0 4 - b l a n k e t - c o n v e n t i o n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d C o n v e n t i o n D b C o n t e x t . c s : 1 3     T r a n s l a t i o n . T e x t  
 L a b e l :   c a n d i d a t e  
 E f f e c t i v e   m a p p i n g :   v a r c h a r ( 2 5 6 )   ( C o n f i g u r e C o n v e n t i o n s   b l a n k e t   r u l e ,   n o   p e r - p r o p e r t y   o v e r r i d e   f o r   T e x t )       C o l u m n :   n v a r c h a r ( 4 0 0 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   c o n v e n t i o n   r u n s   p r e - m o d e l - b u i l d i n g   a n d   s i l e n t l y   o v e r r i d e s   t h e   r e a l   U n i c o d e   c o l u m n  
 F i x :   s c o p e   t h e   c o n v e n t i o n ,   o r   a d d   e . P r o p e r t y ( t   = >   t . T e x t ) . H a s M a x L e n g t h ( 4 0 0 ) ;   ( l e a v e   U n i c o d e   t r u e )   i n   O n M o d e l C r e a t i n g  
 V a l i d a t e :   m e t a d a t a   c h e c k   f o r   I s U n i c o d e ( ) = = t r u e ,   G e t M a x L e n g t h ( ) = = 4 0 0 ;   a u d i t   a l l   o t h e r   s t r i n g   p r o p e r t i e s   u n d e r   t h i s   c o n v e n t i o n  
  
 [ E S C 0 0 5 - f u n c t i o n - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 1 1     S a l e . O r d e r D a t e   ( Y e a r O n C o l u m n )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n   ( S a l e s   n o t   i n   s c h e m a . j s o n )  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   Y E A R ( [ s ] . [ O r d e r D a t e ] )   =   2 0 2 5  
 F i x :   s . O r d e r D a t e   > =   n e w   D a t e T i m e ( 2 0 2 5 , 1 , 1 )   & &   s . O r d e r D a t e   <   n e w   D a t e T i m e ( 2 0 2 6 , 1 , 1 )  
 V a l i d a t e :   c o m p a r e   T o Q u e r y S t r i n g ( )   b e f o r e / a f t e r ;   a c t u a l   p l a n   s h o w s   S e e k   P r e d i c a t e s ,   n o t   r e s i d u a l   P r e d i c a t e  
  
 [ E S C 0 0 5 - f u n c t i o n - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 1 4     P e r s o n . L a s t N a m e   ( U p p e r O n C o l u m n )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n   ( P e o p l e   n o t   i n   s c h e m a . j s o n )  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   U P P E R ( [ p ] . [ L a s t N a m e ] )   =   @ n a m e  
 F i x :   d r o p   . T o U p p e r ( ) ,   c o m p a r e   p . L a s t N a m e   = =   n a m e   d i r e c t l y   ( r e l i e s   o n   C I   c o l l a t i o n   � � �   c o n f i r m )  
 V a l i d a t e :   c o n f i r m   s e r v e r / c o l u m n   c o l l a t i o n   i s   c a s e - i n s e n s i t i v e ;   a c t u a l   p l a n   f o r   S e e k  
  
 [ E S C 0 0 6 - l e a d i n g - w i l d c a r d ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 1 7     P e r s o n . L a s t N a m e   ( S u f f i x S e a r c h )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   L I K E   ' % s o n '  
 F i x :   p r e f i x   s e a r c h ,   f u l l - t e x t ,   o r   i n d e x e d   R E V E R S E ( L a s t N a m e )   c o m p u t e d   c o l u m n   s e a r c h e d   w i t h   S t a r t s W i t h ( " n o s " )  
 V a l i d a t e :   a c t u a l   p l a n   c o n f i r m s   S e e k   o n   t h e   c o m p u t e d   c o l u m n   i n d e x  
  
 [ E S C 0 0 6 - l e a d i n g - w i l d c a r d ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 2 0     P e r s o n . L a s t N a m e   ( C o n t a i n s S e a r c h )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   L I K E   ' % s o n % '  
 F i x :   s a m e   a s   a b o v e   ( r e v e r s e d   c o m p u t e d   c o l u m n   /   f u l l - t e x t )  
 V a l i d a t e :   s a m e  
  
 [ E S C 0 

[Output truncated. Use view_range=[182, ...] to continue reading. In your next response, you may batch this with other view calls. File has at least 269 lines.]
```

</details>


---

<sub>31s</sub>

### `powershell` — Failed

**Convert log file encoding and count lines**

<details>
<summary>Arguments</summary>

```json
{
  "command": "Get-Content -Raw \"samples\\screenshots\\copilot-run1.log\" -Encoding Unicode | Out-File -FilePath \"$env:TEMP\\run1_utf8.txt\" -Encoding utf8; (Get-Content \"$env:TEMP\\run1_utf8.txt\" | Measure-Object -Line).Lines",
  "description": "Convert log file encoding and count lines"
}
```

</details>

```
Permission to run this tool was denied due to the following rules: `shell`
```


---

<sub>33s</sub>

### `powershell` — Failed

**Inspect log file raw bytes and encoding**

<details>
<summary>Arguments</summary>

```json
{
  "command": "python -c \"\ndata = open('samples/screenshots/copilot-run1.log', encoding='utf-8').read()\nprint(len(data))\nprint(repr(data[:200]))\n\"",
  "description": "Inspect log file raw bytes and encoding"
}
```

</details>

```
Permission to run this tool was denied due to the following rules: `shell`
```


---

<sub>36s</sub>

### `powershell` — Failed

**Test basic powershell access**

<details>
<summary>Arguments</summary>

```json
{
  "command": "Write-Output \"test\"",
  "description": "Test basic powershell access"
}
```

</details>

```
Permission to run this tool was denied due to the following rules: `shell`
```


---

<sub>40s</sub>

### `list_powershell`

<details>
<summary>Arguments</summary>

```json
{}
```

</details>

```
<no active shell sessions>
```


---

<sub>47s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log (lines 182-400)

<details>
<summary>88 lines</summary>

```
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   D A T E A D D ( d a y ,   3 0 ,   [ s ] . [ C r e a t e d A t ] )   >   @ n o w  
 F i x :   s . C r e a t e d A t   >   n o w . A d d D a y s ( - 3 0 )  
 V a l i d a t e :   a c t u a l   p l a n   S e e k ;   v e r i f y   d a t e - m a t h   e q u i v a l e n c e   a t   e d g e s  
  
 [ E S C 0 0 8 - a r i t h m e t i c - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 2 6     S a l e . P r i c e   ( M a t h s O n C o l u m n )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   [ s ] . [ P r i c e ]   *   1 . 1   >   1 1 0  
 F i x :   s . P r i c e   >   1 0 0 m  
 V a l i d a t e :   v e r i f y   r o u n d i n g   e q u i v a l e n c e ;   a c t u a l   p l a n   S e e k  
  
 [ E S C 0 0 9 - i m p l i c i t - c o n v e r s i o n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 2 9     A c c o u n t . A c c o u n t N u m b e r   ( N u m b e r V s V a r c h a r )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   v a r c h a r ( 2 0 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   C A S T ( [ a ] . [ A c c o u n t N u m b e r ]   A S   i n t )   =   1 2 3 4 5 ;   a l s o   f a i l s   o n   n o n - n u m e r i c   v a l u e s  
 F i x :   a . A c c o u n t N u m b e r   = =   " 1 2 3 4 5 "  
 V a l i d a t e :   a c t u a l   p l a n   S e e k ;   c o n f i r m   n o   n o n - n u m e r i c   A c c o u n t N u m b e r   r o w s   d e p e n d   o n   t h e   o l d   c a s t   b e h a v i o r  
  
 [ E S C 0 1 0 - o r - a c r o s s - c o l u m n s ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 3 2     P e r s o n . F i r s t N a m e   /   P e r s o n . L a s t N a m e   ( O r A c r o s s C o l u m n s )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   [ p ] . [ F i r s t N a m e ]   =   @ n a m e   O R   [ p ] . [ L a s t N a m e ]   =   @ n a m e ,   d e f e a t i n g   s i n g l e - i n d e x   s e e k  
 F i x :   d b . P e o p l e . W h e r e ( p   = >   p . F i r s t N a m e   = =   n a m e ) . U n i o n ( d b . P e o p l e . W h e r e ( p   = >   p . L a s t N a m e   = =   n a m e ) )  
 V a l i d a t e :   c o n f i r m   U N I O N   ( n o t   U N I O N   A L L )   n e e d e d   i f   a   r o w   c a n   m a t c h   b o t h   p r e d i c a t e s ;   a c t u a l   p l a n   s h o w s   t w o   S e e k s  
  
 [ E S C 0 1 1 - i s n u l l - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 3 5     S a l e . S t a t u s   ( C o a l e s c e O n C o l u m n )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   C O A L E S C E ( [ s ] . [ S t a t u s ] ,   ' N e w ' )   =   ' N e w '  
 F i x :   s . S t a t u s   = =   " N e w "   | |   s . S t a t u s   = =   n u l l  
 V a l i d a t e :   a c t u a l   p l a n   S e e k ;   c o n f i r m   w h e t h e r   S t a t u s   s h o u l d   s i m p l y   d i s a l l o w   N U L L  
  
 [ E S C 0 1 2 - c l o s e d - d a t e - r a n g e ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 3 8     S a l e . O r d e r D a t e   ( C l o s e d E n d D a t e )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   c l o s e d   u p p e r   b o u n d   < =   ' 2 0 2 5 - 1 2 - 3 1 '   s i l e n t l y   e x c l u d e s   r o w s   w i t h   a   t i m e   p a r t   a f t e r   m i d n i g h t   � � �   c o r r e c t n e s s ,   n o t   s p e e d  
 F i x :   s . O r d e r D a t e   <   n e w   D a t e T i m e ( 2 0 2 6 , 1 , 1 )  
 V a l i d a t e :   d i f f   r o w   c o u n t s   b e t w e e n   c l o s e d   a n d   h a l f - o p e n   r a n g e   o n   r e p r e s e n t a t i v e   d a t a  
  
 [ E S C 0 0 5 - f u n c t i o n - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d Q u e r i e s . c s : 1 1     C u s t o m e r . E m a i l   ( L o w e r O n C o l u m n )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   v a r c h a r ( 2 5 6 )   ( G o o d D b C o n t e x t   m a p p i n g   a l r e a d y   m a t c h e s   s c h e m a )       C o l u m n :   v a r c h a r ( 2 5 6 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   L O W E R ( [ c ] . [ E m a i l ] )   =   @ e m a i l   � � �   m a p p i n g   f i x   c a n n o t   h e l p   h e r e  
 F i x :   d r o p   . T o L o w e r ( ) ,   c o m p a r e   c . E m a i l   = =   e m a i l   d i r e c t l y  
 V a l i d a t e :   c o n f i r m   C I   c o l l a t i o n ;   a c t u a l   p l a n   S e e k  
  
 [ E S C 0 0 5 - f u n c t i o n - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d Q u e r i e s . c s : 1 4     P r o d u c t . S k u   ( P r e f i x V i a S u b s t r i n g )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   v a r c h a r ( 3 2 )   ( G o o d D b C o n t e x t   m a p p i n g   a l r e a d y   m a t c h e s   s c h e m a )       C o l u m n :   v a r c h a r ( 3 2 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   S U B S T R I N G ( [ p ] . [ S k u ] ,   1 ,   3 )   =   @ p r e f i x  
 F i x :   p . S k u . S t a r t s W i t h ( p r e f i x )  
 V a l i d a t e :   a c t u a l   p l a n   S e e k  
  
 [ E S C 0 0 6 - l e a d i n g - w i l d c a r d ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d Q u e r i e s . c s : 1 7     C u s t o m e r . E m a i l   ( L e a d i n g W i l d c a r d )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   v a r c h a r ( 2 5 6 )   ( G o o d D b C o n t e x t   m a p p i n g   a l r e a d y   m a t c h e s   s c h e m a )       C o l u m n :   v a r c h a r ( 2 5 6 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   L I K E   ' % '   +   @ t e r m   � � �   m a p p i n g   f i x   c a n n o t   m a k e   t h i s   a   s e e k  
 F i x :   n o n e   v i a   m a p p i n g ;   n e e d s   r e v e r s e d   c o m p u t e d   c o l u m n   /   f u l l - t e x t   s e a r c h  
 V a l i d a t e :   a c t u a l   p l a n   c o n f i r m s   r e s i d u a l   P r e d i c a t e / S c a n   r e m a i n s   e v e n   a f t e r   a n y   m a p p i n g   c h a n g e  
  
 [ E S C 0 0 7 - r e v e r s e - u n i c o d e ]   s a m p l e s / E f S a r g a b l e . S a m p l e / G o o d / G o o d P r e d i c a t e s . c s : 4 2     T r a n s l a t i o n . T e x t   ( V a r c h a r P a r a m V s N v a r c h a r C o l u m n )  
 L a b e l :   i n t e n t i o n a l - e x c e p t i o n  
 E f f e c t i v e   m a p p i n g :   n v a r c h a r ( 4 0 0 )   ( P r e d i c a t e D b C o n t e x t :   H a s M a x L e n g t h ( 4 0 0 ) ,   U n i c o d e   l e f t   d e f a u l t   t r u e )       C o l u m n :   n v a r c h a r ( 4 0 0 ) ,   i n d e x e d  
 E v i d e n c e :   n o t   m e a s u r e d ;   S q l P a r a m e t e r   d e c l a r e d   S q l D b T y p e . V a r C h a r ( 4 0 0 )   a g a i n s t   a n   n v a r c h a r   c o l u m n   � � �   t h e   p a r a m e t e r ,   n o t   t h e   c o l u m n ,   g e t s   c o n v e r t e d   ( h a r m l e s s   b y   d a t a   t y p e   p r e c e d e n c e )  
 F i x :   n o n e   -   e x c e p t i o n  
 V a l i d a t e :   c o n f i r m   v i a   a c t u a l   p l a n   t h a t   o n l y   t h e   p a r a m e t e r   i s   c o n v e r t e d ,   n o t   T e x t  
  
 S u m m a r y :   1 8   f i n d i n g s   ( c o n f i r m e d   0 ,   c a n d i d a t e   4 ,   o t h e r - c a u s e   1 3 ,   e x c e p t i o n   1 )  
 ` ` `  
  
 N o t e   ( n o t   a   f i n d i n g ) :   ` G o o d / G o o d P r e d i c a t e s . c s : 3 8 `   ( ` s . O r d e r D a t e . D a t e   = =   d ` )   a n d   ` G o o d / G o o d D b C o n t e x t . c s ` / ` G o o d / G o o d Q u e r i e s . c s `   s h o w e d   n o   m i s m a t c h e s   � � �   a l l   m a p p i n g s   m a t c h   ` s c h e m a . j s o n `   a n d   a l l   p r e d i c a t e s   k e e p   t h e   i n d e x e d   c o l u m n   b a r e .  
  
 c o p i l o t   :   S e s s i o n   e x p o r t e d   t o :   C : \ D e v \ n a b s - d a r r e l - s c h r e y e r \ e f c o r e - s a r g a b l e - c o n f i g \ s a m p l e s \ s c r e e n s h o t s \ c o p i l o t - r u n 1 . m d  
 A t   C : \ U s e r s \ d w s c h _ x p 1 8 6 1 r \ A p p D a t a \ L o c a l \ T e m p \ p s - s c r i p t - d 7 a 8 9 7 7 0 - c f 4 1 - 4 4 2 b - a b 6 8 - 0 f d 9 6 4 7 3 d e c 6 . p s 1 : 9 5   c h a r : 1  
 +   c o p i l o t   - p   $ p   - - a l l o w - a l l - t o o l s   - - d e n y - t o o l = s h e l l   - - d e n y - t o o l = w r i t e   -   . . .  
 +   ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~  
         +   C a t e g o r y I n f o                     :   N o t S p e c i f i e d :   ( S e s s i o n   e x p o r t e . . . c o p i l o t - r u n 1 . m d : S t r i n g )   [ ] ,   R e m o t e E x c e p t i o n  
         +   F u l l y Q u a l i f i e d E r r o r I d   :   N a t i v e C o m m a n d E r r o r  
    
  
  
 C h a n g e s         + 0   - 0  
 A I   C r e d i t s   6 5 . 2 9   ( 5 m   1 5 s )  
 T o k e n s           � � �   8 0 0 . 7 k   ( 6 8 6 . 0 k   c a c h e d ,   1 1 4 . 7 k   w r i t t e n )   � � �   � � �   2 2 . 9 k   ( 1 5 . 9 k   r e a s o n i n g )  
 R e s u m e           c o p i l o t   - - r e s u m e = 9 d 8 f 8 7 2 9 - 2 6 c d - 4 5 6 8 - b 7 9 7 - d 9 a 6 5 4 5 a 2 d d 7  
 
```

</details>


---

<sub>57s</sub>

### `view`

**view the file at C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log.**

C:\Dev\nabs-darrel-schreyer\efcore-sargable-config\samples\screenshots\copilot-run1.log (lines 175-185)

<details>
<summary>11 lines</summary>

```
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   L I K E   ' % s o n % '  
 F i x :   s a m e   a s   a b o v e   ( r e v e r s e d   c o m p u t e d   c o l u m n   /   f u l l - t e x t )  
 V a l i d a t e :   s a m e  
  
 [ E S C 0 0 8 - a r i t h m e t i c - o n - c o l u m n ]   s a m p l e s / E f S a r g a b l e . S a m p l e / B a d / B a d P r e d i c a t e s . c s : 2 3     S a l e . C r e a t e d A t   ( D a t e A d d O n C o l u m n )  
 L a b e l :   o t h e r - c a u s e  
 E f f e c t i v e   m a p p i n g :   n / a       C o l u m n :   u n k n o w n  
 E v i d e n c e :   n o t   m e a s u r e d ;   e x p e c t e d   D A T E A D D ( d a y ,   3 0 ,   [ s ] . [ C r e a t e d A t ] )   >   @ n o w  
 F i x :   s . C r e a t e d A t   >   n o w . A d d D a y s ( - 3 0 )  
```

</details>


---

<sub>2m 7s</sub>

### Copilot

## Scoring: copilot-run1.log vs expected-findings.json

**Findings extracted from log (18 total):** ESC001@BadDbContext.cs:18, ESC002@26, ESC003@35, ESC004@BadConventionDbContext.cs:13, ESC005@BadPredicates.cs:11, ESC005@14, ESC006@17, ESC006@20, ESC008@23, ESC008@26, ESC009@29, ESC010@32, ESC011@35, ESC012@38, ESC005@BadQueries.cs:11, ESC005@14, ESC006@17, ESC007@GoodPredicates.cs:42

### Manifest scoring table

| # | File | Line | Rule | should_flag | Expected label/kind | Result |
|---|------|------|------|---|---|---|
| 1 | BadConventionDbContext.cs | 13 | ESC004-blanket-convention | true | candidate | ✅ TP (label match) |
| 2 | BadConventionDbContext.cs | 21 | none | false | NEGATIVE | ✅ TN (not flagged) |
| 3 | BadDbContext.cs | 18 | ESC001-unicode-mismatch | true | candidate | ✅ TP (label match) |
| 4 | BadDbContext.cs | 20 | none | false | NEGATIVE | ✅ TN (not flagged) |
| 5 | BadDbContext.cs | 26 | ESC002-maxlength-without-unicode | true | candidate | ✅ TP (label match) |
| 6 | BadDbContext.cs | 28 | none | false | NEGATIVE | ✅ TN (not flagged) |
| 7 | BadDbContext.cs | 35 | ESC003-length-mismatch | true | candidate | ✅ TP (label match) |
| 8 | BadPredicates.cs | 11 | ESC005-function-on-column | true | other-cause | ✅ TP (label match) |
| 9 | BadPredicates.cs | 14 | ESC005-function-on-column | true | other-cause | ✅ TP (label match) |
| 10 | BadPredicates.cs | 17 | ESC006-leading-wildcard | true | other-cause | ✅ TP (label match) |
| 11 | BadPredicates.cs | 20 | ESC006-leading-wildcard | true | other-cause | ✅ TP (label match) |
| 12 | BadPredicates.cs | 23 | ESC008-arithmetic-on-column | true | other-cause | ✅ TP (label match) |
| 13 | BadPredicates.cs | 26 | ESC008-arithmetic-on-column | true | other-cause | ✅ TP (label match) |
| 14 | BadPredicates.cs | 29 | ESC009-implicit-conversion | true | other-cause | ✅ TP (label match) |
| 15 | BadPredicates.cs | 32 | ESC010-or-across-columns | true | other-cause | ✅ TP (label match) |
| 16 | BadPredicates.cs | 35 | ESC011-isnull-on-column | true | other-cause | ✅ TP (label match) |
| 17 | BadPredicates.cs | 38 | ESC012-closed-date-range | true | other-cause | ✅ TP (label match) |
| 18 | BadQueries.cs | 11 | ESC005-function-on-column | true | other-cause | ✅ TP (label match) |
| 19 | BadQueries.cs | 14 | ESC005-function-on-column | true | other-cause | ✅ TP (label match) |
| 20 | BadQueries.cs | 17 | ESC006-leading-wildcard | true | other-cause | ✅ TP (label match) |
| 21 | GoodDbContext.cs | 19 | none | false | GOOD | ✅ TN |
| 22 | GoodDbContext.cs | 21 | none | false | NEGATIVE | ✅ TN |
| 23 | GoodDbContext.cs | 27 | none | false | GOOD | ✅ TN |
| 24 | GoodDbContext.cs | 29 | none | false | NEGATIVE | ✅ TN |
| 25 | GoodDbContext.cs | 36 | none | false | GOOD | ✅ TN |
| 26 | GoodDbContext.cs | 43 | none | false | GOOD | ✅ TN |
| 27 | GoodDbContext.cs | 45 | none | false | NEGATIVE | ✅ TN |
| 28 | GoodPredicates.cs | 11 | none | false | GOOD | ✅ TN |
| 29 | GoodPredicates.cs | 14 | none | false | GOOD | ✅ TN |
| 30 | GoodPredicates.cs | 17 | none | false | GOOD | ✅ TN |
| 31 | GoodPredicates.cs | 20 | none | false | NEGATIVE | ✅ TN |
| 32 | GoodPredicates.cs | 23 | none | false | GOOD | ✅ TN |
| 33 | GoodPredicates.cs | 26 | none | false | GOOD | ✅ TN |
| 34 | GoodPredicates.cs | 29 | none | false | GOOD | ✅ TN |
| 35 | GoodPredicates.cs | 32 | none | false | GOOD | ✅ TN |
| 36 | GoodPredicates.cs | 35 | none | false | GOOD | ✅ TN |
| 37 | GoodPredicates.cs | 38 | none | false | NEGATIVE | ✅ TN |
| 38 | GoodPredicates.cs | 42 | ESC007-reverse-unicode | false | ESC007-reverse-unicode (intentional-exception) | ✅ Flagged ESC007/intentional-exception — allowed, not a FP |
| 39 | GoodQueries.cs | 8 | none | false | GOOD | ✅ TN |
| 40 | GoodQueries.cs | 11 | none | false | GOOD | ✅ TN |
| 41 | GoodQueries.cs | 14 | none | false | NEGATIVE | ✅ TN |
| 42 | PredicateDbContext.cs | 21 | none | false | GOOD | ✅ TN |

### Summary

| Metric | Value |
|---|---|
| **TP** | 17 |
| **FN** | 0 |
| **FP** | 0 |
| **Label accuracy** | 17/17 = 100% |
| **Precision** | 17/17 = 100% |
| **Recall** | 17/17 = 100% |

Every should_flag=true entry was caught with the correct rule id and label; the one finding on a should_flag=false line (GoodPredicates.cs:42) matched its own rule id (ESC007-reverse-unicode), which is the designated intentional-exception, so it does not count as a false positive. No false positives, no false negatives, no label mismatches.

---

<sub>Generated by [GitHub Copilot CLI](https://github.com/features/copilot/cli)</sub>
