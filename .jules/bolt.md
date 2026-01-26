## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.
## 2025-02-21 - [File Reading Optimization]
**Learning:** Legacy  loop was inefficient (O(N) syscalls/overhead). Replaced with  (O(1) overhead) + specialized line ending detection. Key was replicating the exact legacy line ending logic: CRLF anywhere > LF at end > CR at end. Also, strict legacy requirement: empty files must contain `\uFFFF`.
**Action:** Always verify legacy constraints (like empty file sentinels) before optimizing I/O.
## 2025-02-21 - [File Reading Optimization]
**Learning:** Legacy StreamReader.Read() loop was inefficient. Replaced with ReadToEnd(). strict legacy requirement: empty files must contain \uFFFF.
**Action:** Always verify legacy constraints before optimizing I/O.
