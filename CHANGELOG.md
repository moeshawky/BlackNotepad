# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

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

