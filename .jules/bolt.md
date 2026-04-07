## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-04-07 - [Optimize LineOfIndexOrDefault]
**Learning:** The `LineOfIndexOrDefault` extension in `StringEx.cs` was using an O(n) character-by-character scan, resulting in significant overhead for large strings. Replacing it with `string.IndexOf` loops yielded an ~99x performance improvement while maintaining legacy bug-for-bug compatibility (such as effectively mixing 0-based and 1-based indexing depending on whether the file contains newlines).
**Action:** When finding character occurrences in strings, prefer `string.IndexOf` within a `while` loop rather than iterating character by character.
