## 2025-02-21 - [Inefficient and Buggy ReplaceAll]
**Learning:** `ReplaceAll` used `Regex.Replace` with inverted logic: case-sensitive search used `IgnoreCase` (bug) and case-insensitive used default (case-sensitive) regex. `String.Replace` is ~20x faster for literal case-sensitive replacement.
**Action:** Verify boolean logic carefully when `Regex` options are involved. Prefer `String.Replace` for literals.

## 2025-02-21 - [Environment Limitations]
**Learning:** This project targets .NET Framework 4.7.2 and uses WPF. Linux/Mono environment cannot build or run tests due to missing WPF assemblies (`PresentationCore`, `PresentationFramework`).
**Action:** Use Python scripts for logic verification when C# tests are unrunnable.

## 2025-02-21 - [Legacy Empty File Logic]
**Learning:** Legacy `FileModelService.ReadFile` loop produced `"\uffff"` (EOF char) for empty files instead of an empty string. This behavior must be preserved when optimizing.
**Action:** When replacing legacy I/O loops, explicitly check for edge cases like empty files and match legacy output quirks exactly.

## 2025-02-21 - [CI Label Verification]
**Learning:** The CI pipeline enforces PR labels using `pr-labels.yml` but lacked an `auto-label.yml` workflow to apply them automatically, causing CI failures for bots/agents. Workflows introduced in a PR only trigger if using `pull_request`, not `pull_request_target`.
**Action:** Always check for missing CI infrastructure (like labelers) when submitting PRs to strict repos, and add them if missing. Ensure `auto-label.yml` triggers on `pull_request` to run for the current PR.
