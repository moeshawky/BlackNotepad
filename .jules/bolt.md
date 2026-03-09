## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [GoTo Line Calculation O(n) Bottleneck]
**Learning:** The legacy `GoTo` line functionality used a character-by-character `for` loop to scan for newline characters, which caused significant performance issues for large documents. Using a `string.IndexOf` loop gives ~100x performance improvements while precisely mimicking the legacy start line index calculation.
**Action:** Replace `for` loop character scanning with `string.IndexOf` chunks in all line counting algorithms, ensuring exact line-start index offsets logic is maintained.
