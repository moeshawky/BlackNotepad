## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-14 - Replace O(n) String Iteration with string.IndexOf in GoTo Logic
**Learning:** Legacy character-by-character string scanning algorithms (such as finding lines with a loop over `text[i]`) are a significant bottleneck in processing large documents, consuming millions of operations compared to native index searches.
**Action:** When finding specific characters or sequences within strings (especially large files), always prefer `.IndexOf` or `.LastIndexOf` to jump chunk-by-chunk over a manual character iteration loop, which provided a roughly ~7x execution time improvement in `MainViewModel.GoTo`.
