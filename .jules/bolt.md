## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Quirky Legacy String Indexing]
**Learning:** The `LineOfIndexOrDefault` string extension returned mixed 0-based and 1-based indexing returns based on the presence of line endings in the string (e.g. "A" returns 1 but "A\nB" returns 0 for index 0).
**Action:** Always test boundary edge cases completely on legacy functions and maintain legacy returns instead of "fixing" them if they are heavily relied upon.
