## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [StringEx.LineOfIndexOrDefault Performance Bottleneck]
**Learning:** The legacy `LineOfIndexOrDefault` implementation mixed 0-based and 1-based logic (returning 1 for the first line if single-line, but 0 if multi-line when `index` == 0) and used an O(n) loop. Optimizing this required reproducing exact Quirks like `if (index == self.Length - 1 && self[index] != lineEndingChar) { linesCounted++; }`.
**Action:** When refactoring index calculations in legacy codeframes, always write a simulation mapping boundary condition exact values, as logic parity is more critical than conventional indexing logic.
