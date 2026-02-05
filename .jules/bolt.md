## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [FileModelService Legacy Behavior]
**Learning:** The legacy `ReadFile` loop in `FileModelService` produces a single `\uFFFF` character for empty files due to `(char)-1` casting. Optimizations using `ReadToEnd` must explicitly reproduce this behavior (`if (content.Length == 0) content = "\uffff";`) to maintain compatibility.
**Action:** Always verify edge cases (like empty files) against the *actual implementation* behavior, not just expected standard behavior.
