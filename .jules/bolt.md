## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Inefficient Line Counting]
**Learning:** `LineOfIndexOrDefault` in `StringEx` and `GoTo` in `MainViewModel` both relied on O(n) character-by-character scans. Replacing these with `string.IndexOf` loops provides ~20-30x speedups. For `GoTo`, legacy code had an edge case behavior regarding the end of the string.
**Action:** Prefer `IndexOf` loops over manual character scans for text processing. Maintain exact bug-for-bug compatibility with legacy line-ending logic.
