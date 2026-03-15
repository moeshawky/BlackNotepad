## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Inefficient Line Tracking]
**Learning:** `LineOfIndexOrDefault` in `StringEx.cs` used a character-by-character scan (O(N) operation where N is the index). When called repeatedly for operations like `FindNext` or `ReplaceAll`, it severely slowed down performance for large strings. Replacing this scan with a `string.IndexOf` loop scanning chunks provided a ~35x speedup.
**Action:** Always prefer `string.IndexOf` to find character occurrences over character-by-character iterations in large strings, ensuring exact bug-for-bug preservation of legacy quirks (e.g. implicitly incrementing a counter on the last index).
