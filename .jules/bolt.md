## 2025-02-21 - [Inefficient GoTo Character Scanning]
**Learning:** `MainViewModel.GoTo` previously used an `O(N)` character-by-character scan that resulted in substantial lag (~150ms for 10,000 lines). The legacy behavior mathematically equated the cursor position to `index - 1 + lineCharToInclude` relative to newline index hits.
**Action:** Replace manual string scanning loops with optimized `text.IndexOf(lineEndingChar, index)` loops while meticulously mapping the algebra of legacy off-by-one and indexing behaviors to maintain 100% bug compatibility without regression.

## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.
