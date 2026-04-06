## 2025-04-06 - [MainViewModel Replace Allocation Optimization]
**Learning:** `MainViewModel.Replace` used an O(N) `Contains` check and a full document `ToLower()` allocation when checking for case-insensitive matches before replacing, introducing a huge performance hit and a massive bug where the saved document lost all casing. Validation of matched text can be done without allocation or O(N) scans by using `string.Compare(allText, index, sought, 0, sought.Length, comparison)`.
**Action:** Use `string.Compare` with string indices instead of `Substring` or full-document `.Contains()` and `.ToLower()` to validate matches, preventing huge memory allocations and O(N) performance hits.

## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.
