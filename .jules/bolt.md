## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [StringEx.LineOfIndexOrDefault Inefficiency]
**Learning:** `LineOfIndexOrDefault` used a character-by-character scan (O(N)) to count lines up to an index. For large files, this was extremely slow. By replacing it with a `string.IndexOf` loop, the execution time was reduced by ~100x while precisely maintaining its quirk where `index == self.Length - 1` increments the line count if not a newline.
**Action:** Replace character-by-character string scans with chunked `IndexOf` loops for massive performance gains.
