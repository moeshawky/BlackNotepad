## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Optimize LineOfIndexOrDefault in StringEx]
**Learning:** `LineOfIndexOrDefault` mixed 0-based and 1-based line counting logic, returning 1 for the first line of a single-line string (no line break), but returning 0 for the first line of a multi-line string before the line break. This unique behavior needed to be exactly preserved to ensure bug-for-bug compatibility with legacy features like Go To.
**Action:** When converting O(n) character loops into `IndexOf` loops, verify initial edge conditions (like `linesCounted == 0`) carefully and check if the method unexpectedly deviates from expected baseline variables (e.g. initial `value = 1` vs `linesCounted = 0`).
