## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.
## 2025-03-03 - [Inefficient and Buggy Replace]
**Learning:** `MainViewModel.Replace` used `ToLower()` on the entire document for case-insensitive replacements, resulting in the whole document being incorrectly lowercased. It also allocated large strings via `Substring()` and interpolation.
**Action:** Use `Remove(IndexOfCaret, length).Insert(IndexOfCaret, replacement)` to safely and efficiently update the string at the caret position without full-document case modifications.
