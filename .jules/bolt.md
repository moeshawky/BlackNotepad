## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Optimized FindNext Logic]
**Learning:** Legacy `FindNext` logic allocated a lower-case copy of the entire document for every search, which is inefficient for large files. It also used `Substring` + `LastIndexOf` for Up search, which allocates substring copies.
**Action:** Optimized using `IndexOf`/`LastIndexOf` with `StringComparison` and calculated start indices to avoid all string allocations.
**Correction:** When simulating `LastIndexOf` logic, I found that `Substring(0, caret).LastIndexOf` enforces "match fully within substring", whereas `LastIndexOf(..., caret-1)` allows matches to overlap the caret if not careful. The correct start index for "Up search ending before caret" is `caret - len(sub)`.
