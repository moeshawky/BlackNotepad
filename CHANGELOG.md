# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [1.1.2] - 2026-06-14

### Fixed
- Silent crash on launch: removed `Icon` from XAML (XamlParseException if ICO malformed/not found), now set in code-behind with try-catch fallback
- Added `DispatcherUnhandledException` handler so any unhandled exception shows an error dialog instead of silently killing the process

## [1.1.1] - 2026-06-14

### Fixed
- Taskbar icon now shows BlackNotepad icon (Window.Icon was never set)
- Theme switching broken: CommandParameter passed string but RelayCommand expected ThemeMode enum, silently failing
- Installer version synced to 1.1.0 (was stuck at 1.0.10)

## [1.1.0] - 2026-06-14

### Added
- Line numbers toggle in View menu
- Unit tests: ViewStateService validation, MainViewModel null-safety, ViewStateModel contracts
- GitHub Actions CI workflow
- Inno Setup installer script
- Auto-save failure indicator in status bar (IsAutoSaveFailed property + orange TextBlock)
- Emergency recovery documentation on ViewStateService.Open()
- XML docstrings on all public service and ViewModel members

### Fixed
- Theme switching now modifies MergedDictionaries[0] instead of root Application resources
- Zoom null-fallthrough NullReferenceException in OnZoomIn/OnZoomOut (added return after RestoreDefaultZoom)
- ViewStateService.Open() validates deserialized state with try-catch + null validation + enum validation
- FileModelService.SaveFile now atomic (write to temp, File.Replace with backup, cleanup on error)
- 8 user-initiated catch blocks now show error dialogs via DialogService.ShowDialog
- DialogService.GetDialog/GetDialogViewModel throw InvalidOperationException on unregistered types
- Null guards added at OnFind, OnReplace, OnGoTo usage sites for dialog ViewModels
- AssemblyInfo version bumped to 1.0.11.0 (was stale at 1.0.10.0)
- .csproj ClickOnce versions synced to 1.0.11.x
- TestBase and MainViewModelNullSafetyTests use correct field names and mock setups

### Removed
- Dead IModalDialog interface (GoToDialog freed from unused contract)

### Fixed (early cleanup)
- Removed empty OnDialogDone method (dead code)
- Removed LineEndingDisplay property duplication (now uses converter directly)
- Updated README.md to remove false theme support claims
- Added AGENTS.md to .gitignore (agentic artifact exclusion)

## [1.0.10] - 2026-01-24

### Performance
- Optimized ReplaceAll functionality:
  - Switched to string.Replace for case-sensitive replacements (~20x faster).
  - Reduced overhead for literal string replacements.
- Optimized File Loading:
  - Replaced character-by-character reading with StreamReader.ReadToEnd for significant speed improvements on large files.
- Optimized Date/Time insertion:
  - Switched to AppendText to avoid costly string concatenation operations.

### Bug Fixes
- Fixed critical logic inversion in "Match Case" for Replace operations.
- Fixed application crashes when using special characters (regex metacharacters) in the Replace field.
- Fixed issue where empty files were loaded with a single \uFFFF character.

### Development
- Added IFileModelService mocks to improved unit testing infrastructure.
- Added reproduction tests for ReplaceAll bugs to prevent regressions.

