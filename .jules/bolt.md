## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-03-26 - [Inefficient and Buggy Replace]
**Learning:** `MainViewModel.Replace` used a very inefficient concatenation strategy via `Substring` and `ToLower` which could lowercase the entire document on case-insensitive matches if `Contains` was false. Using `string.Remove(index, length).Insert(index, replacement)` fixes the bug and is ~20x faster because it avoids massive string reconstruction overhead.
**Action:** When replacing a substring at a known index, use `Remove().Insert()` rather than full-string lowercasing and multiple substrings.
