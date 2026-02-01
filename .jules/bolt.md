## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Legacy File Loading Quirks]
**Learning:** `FileModelService.ReadFile` has two critical legacy behaviors:
1. Empty files must load as a string containing `\uFFFF` (not empty string).
2. Line ending detection is "CRLF anywhere" vs "LF/CR only at end of file".
**Action:** Always replicate `\uFFFF` for empty content and verify line ending logic against `PerformanceTest` harness when refactoring.
