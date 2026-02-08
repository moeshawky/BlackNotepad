## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.
## 2024-05-22 - [Legacy Loop Side Effects]
**Learning:** Legacy loops reading char-by-char can have side effects (like casting -1 to \uffff on empty files) that are hard to spot.
**Action:** Always test edge cases (empty file, single char) when replacing loops with bulk operations.
## 2026-02-08 - [CI/CD]
**Learning:** GitHub CI workflows (like pr-labels) can fail if required labels are missing.
**Action:** Implement an auto-labeler workflow using  to automatically apply labels based on file paths, ensuring CI passes without manual intervention.
## 2026-02-08 - [CI/CD]
**Learning:** GitHub CI workflows (like pr-labels) can fail if required labels are missing.
**Action:** Implement an auto-labeler workflow to automatically apply labels based on file paths.
