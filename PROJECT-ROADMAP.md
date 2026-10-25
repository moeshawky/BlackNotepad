# BlackNotepad — Repository Functional Surface Census & Maintenance Roadmap

**Repository:** `moeshawky/BlackNotepad` (Fork continuation of `savaged/BlackNotepad`)
**Target Framework:** .NET Framework 4.7.2 (C# WPF, MVVM Light)
**Session ID:** `c6ad1e04-dc4e-4eed-9b98-9d3a7ca2fb6f`
**Date:** 2026-10-25
**Maintainer:** Moe S.

---

## 1. Quantitative Implementation Scale

| Dimension | Measured Metric |
|-----------|----------------|
| Active C# Source Files | 86 (.cs files excluding obj/bin/node_modules) |
| Active XAML Files | 11 (.xaml files) |
| Classes | 59 |
| Interfaces | 19 |
| Enums | 3 (`ThemeMode`, `LineEndings`, `SaveReason`) |
| Structs | 0 |
| Methods | 229 |
| MainViewModel RelayCommands | 26 |
| Test Files | 11 active test fixtures (15 total files in `BlackNotepad.Test`) |
| Total Automated Tests | 60 tests (52 unit, 6 VM null-safety, 2 UI) |
| Build & Installer Assets | `setup.iss` (Inno Setup 6), `.github/workflows/build.yml` (GitHub Actions) |

---

## 2. Repository Functional Surface Census

| Census Category | Repository Implementation & Surface |
|---|---|
| **Executables & Entry Points** | `App.xaml` / `App.xaml.cs` (WPF Application entry point, CLI arguments for shell open). |
| **CLI Commands / Shell Assoc.** | File association command-line handling in `App.xaml.cs` (`e.Args[0]` file load on startup). |
| **Public APIs & Interfaces** | `IFileModelService`, `IFileHistoryService`, `IViewStateService`, `IDialogService`, `IThemeService`, `IFontZoomLookupService`, `IFontColourLookupService`, `IFontFamilyLookupService`, `IDialog`, `IActionDialogViewModel`, `IFindDialogViewModel`, `IGoToDialogViewModel`, `IReplaceDialogViewModel`, `IHistoryDialogViewModel`. |
| **Library Exports** | Standalone WPF executable, not published as NuGet package. |
| **Services & Daemons** | `DispatcherTimer` AutoSave background service (60s tick on dirty file). |
| **Workers & Background Jobs** | `Task.Run` async background disk I/O in `FileModelService.SaveAsync` and `FileHistoryService`. |
| **Agents & Roles** | Orchestration profiles defined in `AGENTS.md` and `.opencode/skills/`. |
| **Plugins / Extension Points** | Lookup services (`LookupServiceBase<T>`), dialog abstractions (`IDialogService`). |
| **Server Endpoints** | None (pure client desktop application). |
| **Scheduled Operations** | Auto-save timer tick (`OnAutoSaveTick` in `MainViewModel.cs`). |
| **Ingestion / Export Pipelines** | File text loader (`StreamReader` UTF-8 / auto-detect) and serializer (`File.WriteAllText` via temp file + atomic replace). |
| **Persistence / Storage** | JSON ViewState at `%APPDATA%\BlackNotepad\BlackNotepad.ViewState.json`; Document History store at `%APPDATA%\BlackNotepad\History\{sha256(path)}/`. |
| **Network Integrations** | None. Completely local and offline. |
| **External Service Adapters** | Windows Win32 MSDelta API (`msdelta.dll` via P/Invoke in `MsDeltaCodec.cs`). |
| **Auth / Security Surfaces** | Local file system ACLs; path validation on save/load/history paths; temp file isolation. |
| **Configuration Behaviors** | `ViewStateModel` (FontFamily, FontColour, FontZoom, ThemeMode, AutoSaveEnabled, RecentFiles, WordWrap, LineNumbers, Window bounds). |
| **Administrative Operations** | History purge / delete (`DeleteHistoryAsync` in `IFileHistoryService`). |
| **Migrations** | `ViewStateService.Open` backward compatibility fallback when fields or json are missing/corrupted. |
| **Deployment / Bootstrap** | Inno Setup installer script `setup.iss` generating release installer. |
| **Build / Package / Release** | GitHub Actions workflow `.github/workflows/build.yml` compiling `BlackNotepad.sln` via msbuild and building Inno Setup installer. |
| **Recovery / Rollback** | Atomic write (`.tmp` -> `.bak` -> destination) in `FileModelService`; Previous Versions delta restore in `FileHistoryService`. |
| **Maintenance Utilities** | Unit tests in `BlackNotepad.Test`, boundary validation docs (`CBP-BOUNDARY-MAP.md`). |
| **Internal Subsystems** | Search/Find engine, Regex/Replace engine, GoTo line calculator, WordWrap controller, JSON Prettifier (`PrettifyJsonText`), Line Numbering renderer (`LineNumbersControl`). |
| **User-Visible Workflows** | 26 RelayCommands: New, Open, Save, SaveAs, Previous Versions, Exit, Recent Files, Find, Find Next, Replace, GoTo, Time/Date, WordWrap, Zoom In/Out/Reset, Status Bar toggle, Line Numbers toggle, Theme toggle, Font styling, JSON prettify, Print. |
| **Machine-Consumed Workflows** | Windows Shell "Open With..." and double-click file associations. |
| **Indirect Automation** | Auto-save tick executing while user is idle or active. |
| **Compatibility Surfaces** | Support for CRLF (Windows), LF (Unix), and CR (Legacy Mac) line ending detection and display. |
| **Legacy Surfaces** | Old Mac CR line endings; legacy font zoom key fallback. |

---

## 3. Functional Surface Ledger

| Capability | Implementation Area | Dependencies | Tests | Current Condition | Maintenance Findings |
|---|---|---|---|---|---|
| **File Persistence & TOCTOU Guard** | `FileModelService.cs`, `IFileModelService.cs` | `System.IO`, `System.Threading` | 14 tests in `FileModelServiceTests.cs` | In-progress mutation | `FileSaveSnapshot` and `_saveLock` resolve TOCTOU. Needs verification that semaphore doesn't block UI. |
| **File History & Revisions** | `FileHistoryService.cs`, `MsDeltaCodec.cs`, `HistoryDialogViewModel.cs`, `HistoryDialog.xaml` | `msdelta.dll`, `System.Text.Json` / JSON serializer | 5 tests in `FileHistoryServiceTests.cs` | Newly added | Comprehensive tests for service, but missing tests for `HistoryDialogViewModel`. Missing XML docstrings on some public members. |
| **View State Persistence** | `ViewStateService.cs`, `ViewStateModel.cs` | `System.IO`, `Newtonsoft.Json` / JSON | 24 tests in `ViewStateServiceTests.cs` & `ViewStateModelTests.cs` | Solid | 100% resilient to nulls, corrupt JSON, missing files. |
| **Find & Replace Engine** | `FindDialogViewModel.cs`, `ReplaceDialogViewModel.cs`, `MainViewModel.cs` | `MvvmLight`, WPF | 6 tests in `FindTests.cs` & `ReplaceTests.cs` | Solid | Regex and case-sensitive paths tested. |
| **GoTo Line Navigation** | `GoToDialogViewModel.cs`, `MainViewModel.cs` | WPF | 1 test in `GoToTests.cs` | Solid | Out-of-bounds line numbers handled. |
| **Zoom & Font Rendering** | `FontZoomLookupService.cs`, `FontColourLookupService.cs`, `FontFamilyLookupService.cs` | WPF | 6 tests in `MainViewModelNullSafetyTests.cs` | Solid | Null state fallbacks verified. |
| **JSON Prettifier** | `StringEx.cs`, `MainViewModel.cs` | System.Text | **0 tests** | **UNTESTED** | JSON streaming optimization added in PR #96, but zero automated tests exist for invalid JSON, empty text, or whitespace. |
| **Line Ending Detection & Conversion** | `LineEndings.cs`, `LineEndingEnumToDisplayNameConverter.cs` | WPF Converters | 1 test in `LineEndingEnumToDisplayNameConverterTests.cs` | Solid | Previously identified label swap resolved. |
| **Coordinates Status Bar Display** | `CoordsToDisplayNameConverter.cs` | WPF Converters | 1 test in `CoordsToDisplayNameConverterTests.cs` | Solid | Validated. |
| **Line Numbers Control** | `LineNumbersControl.xaml.cs` | WPF | 0 direct tests | Solid | Optimized via StringBuilder in PR #105. |
| **CI Build & Verification** | `.github/workflows/build.yml` | GitHub Actions | None | **DEFECTIVE** | CI builds solution and runs Inno Setup, but **NEVER executes tests**. Tests are completely skipped in CI. |

---

## 4. Test Surface Census & Adversarial Audit

### Test Frameworks & Suites
- Framework: **MSTest** (`Microsoft.VisualStudio.QualityTools.UnitTestFramework`)
- UI Automation: **TestStack.White** (in `UITests/BasicTests.cs`)
- Total Fixtures: 11 active test classes
- Total Test Cases: 60 test methods

### Adversarial Audit Findings
1. **False Confidence in CI:** `.github/workflows/build.yml` contains zero test commands (`vstest.console.exe` or `dotnet test`). A broken test will not fail the CI build.
2. **Untested Functional Surfaces:**
   - `PrettifyJsonCmd`: Zero tests. If malformed JSON is passed, does it throw, swallow, or corrupt content?
   - `HistoryDialogViewModel`: Zero tests. UI interactions (previewing a version, restoring a version, closing dialog) are untested.
   - `AutoSaveTimer`: Race conditions between UI thread edits and background saving are partially tested in unit isolation, but end-to-end ViewModel timer integration is untested.
3. **Headless Environment Constraint:** `UITests/BasicTests.cs` launches `BlackNotepad.exe` using `TestStack.White`. This requires an interactive Windows GUI desktop session; executing this in headless GitHub Actions without desktop emulation will fail. Unit tests and ViewModel tests must run independently.

---

## 5. Maintenance Action Plan

1. **Phase 1: Code Audit Mindset (CAM)**
   - Audit the in-flight Previous Versions / File History implementation (`MsDeltaCodec`, `FileHistoryService`, `FileSaveSnapshot`, `HistoryDialogViewModel`, `HistoryDialog.xaml`).
   - Audit `FileModelService.cs` concurrency, TOCTOU safety, and semaphore acquisition.
   - Audit public API XML docstrings (DNA) and upstream GPLv2 license headers across all modified/untracked files.

2. **Phase 2: Compounded Bug Protocol (CBP)**
   - Re-sweep boundaries B1–B8 and examine newly introduced boundaries (B9: `FileModelService` -> `FileHistoryService`, B10: `MainViewModel` -> `HistoryDialogViewModel`).
   - Validate temporal and exception safety when `msdelta.dll` fails or encounters corrupt history deltas.

3. **Phase 3: Advanced Debugging (AD)**
   - Root-cause any defects identified in CAM/CBP.

4. **Phase 4: Advanced Patching (AP)**
   - Add unit tests for `PrettifyJsonText` in `BlackNotepad.Test`.
   - Add unit tests for `HistoryDialogViewModel`.
   - Ensure complete XML docstrings and GPLv2 headers.

5. **Phase 5: Independent Proof Gate & MAAT**
   - Verify all 6 obligations per patch: compilation, tests, boundary safety, XML docstrings, license preservation, and zero regression.

6. **Phase 6: CI & Release Reconnaissance**
   - Update `.github/workflows/build.yml` to include a `vstest.console.exe` step for unit and ViewModel test assemblies while isolating headless-incompatible UI tests.

---

## 6. Maintenance Execution Record (2026-10-25)

### Phase 1: CAM Reconnaissance
- Auditor subagent 821ddf8e-0374-4fd4-ac4c-b7567f60424a audited working tree changes.
- Identified 4 Critical Blockers:
  1. G-ERR-1: HistoryDialogViewModel restores error strings on preview load failure.
  2. G-EDGE-1: MainViewModel tab-switch TOCTOU corrupts wrong tab on dialog restore.
  3. G-SEM-1 & S-BROKEN-1: PrettifyJson truncates multi-token JSON and misplaced in MainViewModel.
  4. COMPLIANCE-1: Missing upstream GPLv2 headers across new files.

### Phase 2: CBP Boundary Analysis
- Traced newly added boundaries B9 (Persistence -> FileHistoryService), B10 (UI -> HistoryDialogViewModel), B11 (HistoryDialogViewModel -> History Engine), B12 (History Engine -> Win32 msdelta.dll).
- Verified immutability of FileSaveSnapshot and SemaphoreSlim serialization.

### Phase 3 & 4: Advanced Debugging & Advanced Patching (AP)
- Patch engineer 82ca397d-2c32-4443-a99b-4bfca7372d9b applied verified fixes:
  1. HistoryDialogViewModel: Added _previewLoadSuccess gating on IsActionEnabled and Restore().
  2. MainViewModel: Pinned 	argetItem stack local variable before asynchronous dialog initialization.
  3. StringEx.cs: Added PrettifyJson(this string json) with full token streaming and registered in StringExTests.cs.
  4. MsDeltaCodec.cs: Added 0-byte buffer checks and checked integer casts.
  5. FileHistoryService.cs: Deduplicated VersionIds and added circular delta chain detection.
  6. Restored GPLv2 license headers across all 15 new/untracked files.
  7. Added 100% typed XML docstrings to all touched public members.
  8. Created comprehensive tests in StringExTests.cs and HistoryDialogViewModelTests.cs (73 total automated tests).

### Phase 5: Independent Proof Gate & MAAT
- Independent verifier 3bb36abb-db80-41c1-9e9e-8362fb0221b0 confirmed all 8 claims passed with line-by-line evidence.
- Post-proof dialectic G-DIALECTIC-1 resolved.
- Maat order auditor 291381c-fa8b-4c44-9ebe-70e408ac9459 completed 42-confession audit across 6 domains, generating .maat/2026-10-25/ artifacts.
- Fixed CI testing gap in .github/workflows/build.yml (added stest.console.exe step).
- Added [TestCategory("UI")] to BasicTests.cs to decouple GUI automation from headless CI.
- Updated CHANGELOG.md under ## [Unreleased].
- Updated .gitignore to exclude .maat/ and .opencode/.

### Gate Status
- **G-CAM**: PASS
- **G-CBP**: PASS
- **G-AP**: PASS
- **G-PROOF**: PASS (8/8 claims verified independently)
- **G-DIALECTIC-1**: PASS
- **G-MAAT**: PASS (artifacts persisted to .maat/2026-10-25/)
- **G-DIALECTIC-2**: PASS
- **G-PUBLISH / G-COMMIT**: Awaiting explicit operator authorization.
