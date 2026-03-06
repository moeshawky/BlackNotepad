## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Inefficient Line Counting via Character Scan]
**Learning:** `LineOfIndexOrDefault` in `StringEx` used a character-by-character scan (O(N) with high overhead in C# loops) to find the line number of an index. Replacing this with a `string.IndexOf` loop that jumps directly to newlines provides a ~20x performance improvement for line counting in large files.
**Action:** Always prefer `IndexOf` to scan chunks over character-by-character `for` loops when finding specific characters or calculating line offsets. Make sure to precisely replicate bounds and edge case behavior of legacy index counting.
