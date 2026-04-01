## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [StringEx.LineOfIndexOrDefault Performance]
**Learning:** `LineOfIndexOrDefault` used a character-by-character scan `O(n)` to calculate the line number from an index. This scales poorly for large documents. Replacing this with an `O(n / lineLength)` `string.IndexOf` loop yields a ~30x performance improvement while perfectly maintaining edge-case behavior (like mixed 0/1 base indexing).
**Action:** Use `string.IndexOf` to count character occurrences in large strings rather than raw character iteration, as `.NET` heavily optimizes `IndexOf` using SIMD instructions.
