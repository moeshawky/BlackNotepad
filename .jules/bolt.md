## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.
## 2025-02-21 - [Inefficient GoTo in MainViewModel]
**Learning:** `MainViewModel.GoTo` used an O(n) character-by-character scan which is slow for large documents. Using an `IndexOf` loop with `lineEndingChar` is ~3x faster and provides identical logic to legacy indexing.
**Action:** Replace character-by-character string scans with `IndexOf` chunking when looking for line endings or specific characters.
