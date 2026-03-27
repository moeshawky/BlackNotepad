## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [O(n) Character Scans vs IndexOf Chunking]
**Learning:** Legacy implementations in this project (like `StringEx.LineOfIndexOrDefault` and `MainViewModel.GoTo`) use O(n) character-by-character scans inside `for` loops to identify line endings. This is extremely slow for large documents (~40ms for 100k lines). Replacing these with `string.IndexOf` loops that process chunks between line endings provides a massive speedup (~40x faster) while maintaining exact legacy behavior (including zero-based/one-based bugs and end-of-string edge cases).
**Action:** When refactoring string processing loops, prioritize replacing character-by-character iterations with `string.IndexOf` block processing, but thoroughly simulate edge cases (e.g., using isolated C# console apps) to preserve the exact bug-for-bug legacy behavior before replacing.
