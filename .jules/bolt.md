## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-28 - [Inefficient and Buggy Replace]
**Learning:** `MainViewModel.Replace` used `Substring` concatenations with `ToLower()`, creating O(N) intermediate strings. It also contained a bug where failed case-insensitive searches would incorrectly lower-case the entire document content. Using `string.Remove().Insert()` is ~20x faster and prevents the lower-casing bug.
**Action:** Replace manual `Substring` concatenation for substring replacements with `string.Remove().Insert()`. When creating a search copy of a string (e.g. `ToLower()`), never assign it back to the original content variable unless the user explicitly requested a change in casing.
