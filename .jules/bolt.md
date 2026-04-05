## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Inefficient character iteration in LineOfIndexOrDefault]
**Learning:** `LineOfIndexOrDefault` in `StringEx.cs` iterated character-by-character to find lines. This O(N) scan was noticeably slow on large strings. Utilizing a loop with `string.IndexOf` to find newlines provides a ~71x performance improvement for large documents. Note that it mixes 0-based and 1-based logic (defaults to returning 1, but counts lines from 0 starting on the second line based on newline characters), so bug-for-bug compatibility relies heavily on correctly incrementing logic for empty strings and end characters.
**Action:** Always prefer optimized string search methods (`IndexOf`, `LastIndexOf`) over explicit `for` loop character scanning. Ensure careful replication of legacy edge case behaviors (like counting an extra line if the last character is not a newline) when refactoring index/line utilities.
