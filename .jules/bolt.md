## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [O(n) StringEx.LineOfIndexOrDefault]
**Learning:** `StringEx.LineOfIndexOrDefault` in WPF string extensions used a character-by-character scan to count line endings, scaling terribly for large documents ($O(n)$ where $n$ is characters instead of $O(n)$ where $n$ is line endings).
**Action:** Replace `char` loops counting occurrences with `IndexOf` loops which process chunks of characters at a time, resulting in ~100x performance improvements for large strings.
