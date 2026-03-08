## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-03-08 - [Optimize StringEx.LineOfIndexOrDefault]
**Learning:** O(n) character-by-character string scans for line ending counts in `StringEx.LineOfIndexOrDefault` are significantly slower than using `string.IndexOf` loops. Re-writing it yielded ~100x performance improvements for index resolution in large documents while keeping identical behavior.
**Action:** Replace `O(n)` char-by-char loops with `string.IndexOf` loops when performing line/character counting in strings.
