# Maat Report — BlackNotepad

**Codebase:** BlackNotepad (C# WPF/.NET4.8)
**Weighed:** 2026-06-13
**Scope:** `src/**/*.cs`, `WAP/Package.appxmanifest`, `README.md`, `CHANGELOG.md`
**Threshold:** 30/100 (operator-configured)
**Status:** DEVOUR (41.8/100 > 30/100)

---

## Domain Scores

| Domain | Score | Confessions | Weight |
|--------|-------|-------------|--------|
| TRUTH | 4/7 (57%) | 3 GUILTY | 37.20 |
| VISIBILITY | 0/3 (0%) | 3 GUILTY | 10.00 |
| COHERENCE | 3/3 (100%) | 0 GUILTY | 0.00 |
| STRUCTURE | 3/5 (60%) | 2 GUILTY | 10.00 |
| VITALITY | 1/4 (25%) | 3 GUILTY | 10.00 |
| CONTRACT | 5/7 (71%) | 2 GUILTY (weak) | 4.40 |

**Total Weight: 41.80/100**

---

## Findings

### TRUTH Domain (Confessions 1-7)

```yaml
- id: 1
  declaration: "I have not allowed configuration to have two masters"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "All ViewState.json I/O consolidated in ViewStateService. Single config gateway confirmed."
  weight: 0
  locations: ["src/Services/ViewStateService.cs:42-71"]

- id: 3
  declaration: "I have not allowed hardcoded defaults to masquerade as configuration"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: medium
  evidence_for: "Field initializers are deserialization safety nets, not config substitutes. Actual defaults come from lookup services."
  weight: 0
  locations: ["src/Models/ViewStateModel.cs:13,379"]

- id: 4
  declaration: "I have not allowed version numbers to diverge across the codebase"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "AssemblyInfo.cs:58-59 declares 1.0.10.0. Package.appxmanifest:3 declares 1.9.0.0. Two authoritative versions disagree."
  evidence_against: "Package.appxmanifest version is for MSIX packaging; AssemblyInfo is for runtime. May be intentional."
  benign_explanation: "Store manifest version lags behind assembly version — release-process gap."
  needed_to_confirm: "Verify MSIX build pipeline reads AssemblyInfo or manually maintains manifest."
  weight: 15.68
  locations: ["src/Properties/AssemblyInfo.cs:58-59", "WAP/Package.appxmanifest:3"]

- id: 5
  declaration: "I have not allowed stale comments to remain after the bug they describe is fixed"
  finding: GUILTY
  evidence_classification: medium
  evidence_for: "MainViewModel.cs:575-580 contains commented-out code referencing FindDialogViewModel.ResetFilters() which does NOT exist."
  evidence_against: "Comment may be intentional historical context for removed feature."
  benign_explanation: "ResetFilters feature removed during refactoring; comment documents what was removed."
  needed_to_confirm: "Check git history for when ResetFilters was removed."
  weight: 13.52
  locations: ["src/ViewModels/MainViewModel.cs:575-580"]

- id: 6
  declaration: "I have not allowed dead imports to survive"
  finding: GUILTY
  evidence_classification: medium
  evidence_for: "9 unused imports across 3 files: LineEndingEnumToDisplayNameConverter.cs, CoordsToDisplayNameConverter.cs, FileModel.cs"
  evidence_against: "May be Visual Studio auto-generated imports never cleaned up."
  benign_explanation: "Default Visual Studio template adds common namespaces."
  needed_to_confirm: "Verify via IDE unused-import warnings."
  weight: 8.00
  locations: ["src/Converters/LineEndingEnumToDisplayNameConverter.cs:3-7", "src/Converters/CoordsToDisplayNameConverter.cs:2-6", "src/Models/FileModel.cs:4"]

- id: 7
  declaration: "I have not allowed configuration to bypass the central loading path"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "ViewStateService is the sole gateway for config I/O. No bypasses detected."
  weight: 0
  locations: ["src/Services/ViewStateService.cs:42-71"]
```

### VISIBILITY Domain (Confessions 8-14)

```yaml
- id: 8
  declaration: "I have not allowed bare except clauses to swallow errors"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "MainViewModel.cs:1116 — catch { } in OnPrettifyJson(). No re-throw, no logging, no user message. Finally clears busy spinner. User sees nothing."
  evidence_against: "Single instance in production code. Non-critical formatting operation."
  benign_explanation: "Developer intended 'if prettify fails, leave content as-is' — valid UX but implemented without signal."
  needed_to_confirm: "Replace with catch (JsonException) with status bar message."
  weight: 5.5
  locations: ["src/ViewModels/MainViewModel.cs:1116"]

- id: 9
  declaration: "I have not allowed silent None/empty/default returns on failure"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "FileModelService.cs:39-43 returns silently on null Location. MainViewModel.cs:205-218 Open() has no try/catch. MainViewModel.cs:1156-1163 OnAutoSaveTick (async void) calls SaveAsync — failure silently vanishes."
  evidence_against: "WordCount returning 0 for null is valid domain semantics."
  benign_explanation: "Property getters returning defaults for 'no document' state is standard MVVM pattern."
  needed_to_confirm: "Add try/catch to file operations. Surface errors via status bar."
  weight: 6.7
  locations: ["src/Services/FileModelService.cs:39-43", "src/ViewModels/MainViewModel.cs:205-218,1156-1163"]

- id: 10
  declaration: "I have not allowed warning-worthy conditions to pass without a log"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "No logging framework (Serilog, NLog, log4net). No System.Diagnostics.Trace. Zero diagnostic output. 7 async void handlers without try/catch."
  evidence_against: "Lightweight desktop apps sometimes skip logging."
  benign_explanation: "Developer may rely on debugger/IDE for diagnostics during development."
  needed_to_confirm: "Add at minimum Debug.WriteLine for error paths. Consider System.Diagnostics.Trace for production."
  weight: 5.7
  locations: ["src/ViewModels/MainViewModel.cs:470-573"]
```

### COHERENCE Domain (Confessions 15-21)

```yaml
- id: 15
  declaration: "I have not allowed caches to diverge from their source of truth"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "No structural cache exists. ViewStateModel is session-scoped settings, not a cache."
  weight: 0
  locations: []

- id: 16
  declaration: "I have not allowed state to be read at import time instead of at point of use"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "Constructor reads initial state from persistence. All mutable state re-derived at point of use."
  weight: 0
  locations: []

- id: 21
  declaration: "I have not allowed session state to survive beyond the session's lifetime"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "Timers properly stopped in OnClosing. Events properly unsubscribed. Singleton lifecycle bounds all objects to process."
  weight: 0
  locations: []
```

### STRUCTURE Domain (Confessions 22-28)

```yaml
- id: 22
  declaration: "I have not allowed patches to accumulate over unfixed root causes"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: weak
  evidence_for: "Commit 6a4d6db addresses root causes (deserializer bypass, async font race). Null guards are contract enforcement, not patches."
  weight: 0
  locations: []

- id: 23
  declaration: "I have not allowed modules to exceed structural-review threshold without review"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "MainViewModel.cs:1188 lines — exceeds 500-line threshold by 2.1x. Contains 15+ unrelated concerns: file ops, find/replace, goto, font zoom, dialog management, auto-save, recent files, printing, JSON prettify."
  evidence_against: "WPF MVVM pattern tends to produce larger ViewModels. Clear internal section boundaries."
  benign_explanation: "File serves as Application Controller in WPF's constrained MVVM pattern."
  needed_to_confirm: "Extract file operations, find/replace, goto, font management to dedicated services."
  weight: 7.0
  locations: ["src/ViewModels/MainViewModel.cs:1-1188"]

- id: 24
  declaration: "I have not allowed defensive conversions to multiply when one contract would suffice"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: weak
  evidence_for: "66 null-check matches, but serve different architectural concerns (constructor guards, runtime validation, WPF converters, model validity)."
  weight: 0
  locations: []

- id: 25
  declaration: "I have not allowed redundant subsystem spawning"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: none
  evidence_for: "No subprocess/process spawning. Uses only WPF built-in mechanisms."
  weight: 0
  locations: []

- id: 28
  declaration: "I have not allowed the same concern to be handled by multiple mechanisms"
  finding: GUILTY (minor)
  evidence_classification: medium
  evidence_for: "DialogService uses TWO mechanisms: IoC resolution (line 51-52) and reflection-based resolution (line 155-171). Same concern, two independent mechanisms."
  evidence_against: "IoC + reflection is DELIBERATE architectural choice: IoC for ViewModels, reflection for Views."
  benign_explanation: "Standard WPF approach: IoC for business logic, convention-based for UI."
  needed_to_confirm: "Check if single unified resolution would simplify architecture."
  weight: 3.0
  locations: ["src/Services/DialogService.cs:48-59,155-171"]
```

### VITALITY Domain (Confessions 29-35)

```yaml
- id: 29
  declaration: "I have not allowed dead imports to survive"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "9 orphaned imports across 3 files. Each verified unused."
  evidence_against: "Classic Visual Studio 'Add using' habit."
  benign_explanation: "Template-generated files or speculative imports never cleaned up."
  needed_to_confirm: "Enable CS0105/CS8019 warnings."
  weight: 8.0
  locations: ["src/Converters/LineEndingEnumToDisplayNameConverter.cs:3-7", "src/Converters/CoordsToDisplayNameConverter.cs:2-6", "src/Models/FileModel.cs:4"]

- id: 31
  declaration: "I have not allowed deprecated interfaces to remain active"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: N/A
  evidence_for: "Zero [Obsolete] attributes found."
  weight: 0
  locations: []

- id: 32
  declaration: "I have not allowed stale documentation to remain after the code it describes has changed"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "README claims: Theme Support (no theme code), Window Persistence (no ViewState fields), UTF-8 Encoding (no BOM handling), Extended Formats (filter is *.txt only), Custom Colors (2 colors, not 10), 24 zoom levels (actual: 17), Settings at %APPDATA% (actual: %LOCALAPPDATA%)."
  evidence_against: "CHANGELOG.md is accurate. Build instructions appear correct."
  benign_explanation: "README written aspirationally for planned features never fully implemented."
  needed_to_confirm: "Git history to determine if features were removed or never implemented."
  weight: 9.0
  locations: ["README.md"]

- id: 34
  declaration: "I have not allowed dead code to survive"
  finding: GUILTY
  evidence_classification: strong
  evidence_for: "CanExecuteSave (line 398) — defined but never referenced. CanExecuteGoTo (line 416) — defined but never referenced. FileModel.Position (line 78) — public property never used. OnDialogDone (lines 575-579) — entirely commented out but handler still subscribed."
  evidence_against: "Properties may be intended for future XAML bindings."
  benign_explanation: "Defined for consistency or planned UI binding never completed."
  needed_to_confirm: "Static analysis tool dead code analysis."
  weight: 7.0
  locations: ["src/ViewModels/MainViewModel.cs:398,416,575-579", "src/Models/FileModel.cs:78"]
```

### CONTRACT Domain (Confessions 36-42)

```yaml
- id: 36
  declaration: "I have not allowed schema to drift between producers and consumers"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "Schema simple, stable, symmetric. Both producer and consumer use same Newtonsoft.Json defaults."
  weight: 0
  locations: []

- id: 37
  declaration: "I have not allowed file paths passed as wrong type where contract demands specific type"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "File paths consistently string throughout call chain. No type conversions."
  weight: 0
  locations: []

- id: 38
  declaration: "I have not allowed error messages to lie about what went wrong"
  finding: GUILTY (weak)
  evidence_classification: medium
  evidence_for: "MainViewModel.cs:1116 — empty catch { } silently swallows JSON parse failures. No error message shown. Lying by omission."
  evidence_against: "Single instance. Non-critical cosmetic feature."
  benign_explanation: "UX choice: don't show error if prettify fails, leave text unchanged."
  needed_to_confirm: "Verify silent failure is intentional UX."
  weight: 2.2
  locations: ["src/ViewModels/MainViewModel.cs:1116"]

- id: 39
  declaration: "I have not allowed version mismatches between components"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "All component versions consistent. NuGet package versions match assembly references."
  weight: 0
  locations: []

- id: 40
  declaration: "I have not allowed API contracts to be violated"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "Every interface has complete implementation. No missing methods."
  weight: 0
  locations: []

- id: 41
  declaration: "I have not allowed response values to violate their contract"
  finding: GUILTY (weak)
  evidence_classification: medium
  evidence_for: "FontColourLookupService.GetDefault() and FontZoomLookupService.GetDefault() can return null (FirstOrDefault). MainViewModel.cs:116 dereferences without null check."
  evidence_against: "Hardcoded keys make null unreachable in practice. FontFamilyLookupService has null coalescing fallback."
  benign_explanation: "Defensive coding concern, not practical contract violation."
  needed_to_confirm: "Add null coalescing to all GetDefault() implementations."
  weight: 2.2
  locations: ["src/Services/FontColourLookupService.cs:19", "src/Services/FontZoomLookupService.cs:34", "src/ViewModels/MainViewModel.cs:116"]

- id: 42
  declaration: "I have not allowed data to cross boundaries in wrong types"
  finding: NO_EVIDENCE_UNDER_SCOPE
  evidence_classification: strong
  evidence_for: "All boundary crossings use explicit, guarded type conversions. WPF converters check types."
  weight: 0
  locations: []
```

---

## Structural Diagnosis

### Primary Wound: `async void` Epidemic

The codebase's most dangerous structural property is **7 `async void` handlers in MainViewModel.cs without try/catch**:

```csharp
private async void OnNew() { ... }
private async void OnOpen() { ... }
private async void OnOpenRecent(string path) { ... }
private async void OnSave() { ... }
private async void OnSaveAs() { ... }
private async void OnExit() { ... }
private async void OnAutoSaveTick(object sender, EventArgs e) { ... }
```

**Why this is dangerous:** In C#, `async void` methods post exceptions to the `SynchronizationContext`. Without try/catch, any exception from the awaited task **crashes the application silently** with no diagnostic trace. This is the single highest-risk pattern in the codebase.

**Causal chain:**
1. `async void` + no try/catch → exceptions vanish into SynchronizationContext
2. No logging framework → no diagnostic output when exceptions occur
3. No global exception handler → no fallback when exceptions crash the app
4. Auto-save timer → `OnAutoSaveTick` is `async void` → file save failure crashes app silently

### Secondary Wound: MainViewModel as Application Controller

`MainViewModel.cs` (1188 lines) functions as an **Application Controller** while claiming the ViewModel role. This is a `false_authority` violation — the file's name implies single-screen concern, but it orchestrates the entire application:

- File operations (Open, Save, SaveAs, New, Exit)
- Find/Replace logic
- GoTo line logic
- Font zoom management
- Font colour/family application
- Dialog management setup
- Auto-save timer
- Recent files management
- Print preview
- JSON prettification
- Word/line counting
- Version display

**Heatmap density:** `src/ViewModels/MainViewModel.cs:1-1188` — HIGH (confessions 8, 9, 10, 23, 28, 34)

### Tertiary Wound: Aspirational Documentation

README describes features that don't exist:
- Theme Support → no theme code
- Window Persistence → no ViewState fields
- UTF-8 Encoding → no BOM handling
- Extended Formats → filter is *.txt only
- Custom Colors → 2 colors, not 10
- 24 zoom levels → actual: 17

**Heatmap density:** `README.md` — MEDIUM (confession 32)

---

## Heatmap

| Domain | Density | Files |
|--------|---------|-------|
| TRUTH | LOW | AssemblyInfo.cs, Package.appxmanifest, MainViewModel.cs, Converters/*.cs |
| VISIBILITY | HIGH | MainViewModel.cs:470-573, FileModelService.cs:39-49 |
| COHERENCE | NONE | — |
| STRUCTURE | HIGH | MainViewModel.cs:1-1188, DialogService.cs:48-171 |
| VITALITY | MEDIUM | Converters/*.cs, MainViewModel.cs:398,416,575-579, FileModel.cs:78, README.md |
| CONTRACT | LOW | FontColourLookupService.cs:19, FontZoomLookupService.cs:34, MainViewModel.cs:116 |

---

## Unknown

1. **Runtime reflection:** Static analysis cannot detect dynamic invocation of config loading. A runtime probe would be needed to confirm no reflection-based bypass exists.

2. **Build-time version sync:** Whether MSIX packaging reads AssemblyInfo or manually maintains Package.appxmanifest is unclear from source alone.

3. **Temporal accumulation rate:** Without commit-by-commit line count history, we cannot confirm whether MainViewModel.cs is growing, stable, or shrinking.

4. **Natural split boundaries:** A full dependency graph analysis would identify whether extracting services would create circular dependencies or require exposing internal state.

5. **Test coverage impact:** Whether extracting from MainViewModel would break existing tests.

---

## Next Instruments

1. **CAM (code-audit-mindset)** on MainViewModel.cs — identify specific G-* violations within the oversized file
2. **CBP (compounded-bug-protocol)** on the async void epidemic — trace the exception propagation chain
3. **Repo-maintenance-workflow** — if operator plans to split MainViewModel, run full chain to verify no regressions

---

*"You are the scale. Weigh truth against appearance. Report what is knowable and what remains hidden."*
