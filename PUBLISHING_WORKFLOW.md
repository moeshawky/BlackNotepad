# PUBLISHING_WORKFLOW.md

> **Documentarian protocol:** This file describes the actual release pipeline for BlackNotepad as implemented in CI and local workflows. Measure first. Write second. Verify always.

## Overview

BlackNotepad uses a **tag-triggered release** pipeline:

```
Local build → Commit → Tag v* → Push → CI builds + packages → Release created
```

No local .NET SDK is required. All builds run on GitHub Actions (`windows-latest`).

---

## Release Pipeline Architecture

```
┌─────────────────────────────────────────────────────────┐
│  Developer (local)                                      │
│  1. Make changes                                        │
│  2. Commit + push to master                             │
│  3. Tag v1.x.y                                          │
│  4. Push tag                                            │
└──────────────────────┬──────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────┐
│  GitHub Actions: .github/workflows/build.yml            │
│                                                         │
│  BUILD job (runs on every push/PR to master):           │
│    checkout → MSBuild → NuGet restore → Release build   │
│    → Inno Setup (choco install innosetup) → iscc        │
│    → Upload BlackNotepad-Setup-*.exe as artifact        │
│                                                         │
│  RELEASE job (runs only on v* tags):                    │
│    Download artifact → softprops/action-gh-release@v2   │
│    → Creates GitHub Release with .exe attached          │
│    → Auto-generates release notes                       │
└─────────────────────────────────────────────────────────┘
```

---

## Step-by-Step Release Procedure

### 1. Pre-flight Checks

Before tagging, verify:

```bash
# Check CI is green on master (build.yml must pass)
# Check CHANGELOG.md has a [version] section for this release
# Check version numbers are consistent:
```

| File | Location | Value |
|------|----------|-------|
| `src/Properties/AssemblyInfo.cs` | `AssemblyVersion` + `AssemblyFileVersion` | `1.x.y.0` |
| `src/BlackNotepad.csproj` | `<AssemblyVersion>` + `<FileVersion>` | `1.x.y.0` |
| `src/BlackNotepad.csproj` | `<AssemblyOriginatorKeyFileVersion>` | `1.x.y.0` |
| `setup.iss` | `#define MyAppVersion` | `"1.x.y"` |
| `CHANGELOG.md` | Top entry | `[1.x.y]` section |

### 2. Version Bumping

Update all five locations atomically in one commit:

```bash
# Example: bumping from 1.1.0 to 1.2.0
# Edit src/Properties/AssemblyInfo.cs — Assembly + File version
# Edit src/BlackNotepad.csproj — Assembly, File, OriginatorKey versions
# Edit setup.iss — MyAppVersion define
# Edit CHANGELOG.md — Add new section at top, keep [Unreleased] placeholder
```

### 3. Commit the Version Bump

```bash
git add -A
git commit -m "Bump version to 1.2.0 for release"
```

### 4. Tag and Push

```bash
git tag v1.2.0
git push origin master --tags
```

### 5. Monitor CI

- **Build job** starts immediately on push
- **Release job** starts after build succeeds, only if tag matches `v*`
- Release appears at: `https://github.com/moeshawky/BlackNotepad/releases/tag/v1.2.0`

---

## CI Workflow: `.github/workflows/build.yml`

### Triggers

```yaml
on:
  push:
    branches: [master]    # Build on every push to master
    tags: ['v*']          # Build + release on version tags
  pull_request:
    branches: [master]    # Build on PRs (no release)
```

### BUILD Job

| Step | Action | Purpose |
|------|--------|---------|
| 1 | `actions/checkout@v4` | Clone repo |
| 2 | `microsoft/setup-msbuild@v1.1` | Install MSBuild |
| 3 | `nuget/setup-nuget@v2` | Install NuGet |
| 4 | `nuget restore BlackNotepad.sln` | Restore packages (MvvmLight, Newtonsoft.Json, etc.) |
| 5 | `msbuild BlackNotepad.sln /p:Configuration=Release /p:Platform="Any CPU" /p:SignManifests=false` | Build Release |
| 6 | `choco install innosetup --no-progress -y` | Install Inno Setup via Chocolatey |
| 7 | `iscc setup.iss` | Compile installer |
| 8 | `actions/upload-artifact@v4` | Upload `BlackNotepad-Setup-*.exe` |

### RELEASE Job

| Step | Action | Purpose |
|------|--------|---------|
| 1 | `actions/download-artifact@v4` | Download the built installer |
| 2 | `softprops/action-gh-release@v2` | Create GitHub Release with .exe attached |

**Gate:** `if: startsWith(github.ref, 'refs/tags/v')` — only runs on version tags.

---

## Inno Setup: `setup.iss`

Produces a single `BlackNotepad-Setup-{version}.exe` installer.

### Installer Features

| Feature | Implementation |
|---------|----------------|
| .NET 4.7.2 prerequisite | Registry check (`HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\Release >= 461808`) |
| Install location | `%ProgramFiles%\BlackNotepad` |
| Start Menu shortcut | `{autoprograms}\BlackNotepad` |
| Desktop shortcut | Optional (user selects during install) |
| Post-install launch | Launches BlackNotepad automatically |
| Icon | `src\logo.ico` |
| Compression | LZMA solid |
| Architecture | x64-compatible mode |

### Installer File Manifest

```
{app}\BlackNotepad.exe       — Main executable
{app}\*.dll                  — Dependencies (MvvmLightLibs, Newtonsoft.Json, etc.)
{app}\*.xml                  — XML doc files
{app}\*.config               — App.config
{app}\logo.ico               — Application icon
```

---

## Common Release Issues and Fixes

### CI build fails: CS8511 (`is null` on open generic)

**Cause:** C# 7.3 does not allow `is null` on open generic type parameters.

**Fix:** Replace `is null` with `== null` in the offending file.

**File:** `src/Services/DialogService.cs` — lines 59, 78

**Commit message:** `"Fix CS8511: use == null for C# 7.3 compatibility"`

### CI build fails: missing `using` in test files

**Cause:** Test files added without required namespace imports.

**Fix:** Add missing `using` directives:
- `System` — for `Attribute`, `ObsoleteAttribute`
- `Microsoft.Win32` — for `Registry`, `RegistryKey`

### Release job not triggering

**Cause:** Tag doesn't match `v*` pattern, or build job failed.

**Check:** Tag must be pushed (not just committed), must start with `v`, and build job must succeed first.

### Installer output path changed

**Cause:** `setup.iss` `OutputDir=.` puts the .exe in repo root.

**Fix:** CI uploads from root — ensure `BlackNotepad-Setup-*.exe` glob matches.

---

## Version Numbering

| Type | Format | Example |
|------|--------|---------|
| Assembly | `Major.Minor.Patch.0` | `1.2.0.0` |
| ClickOnce | `Major.Minor.Patch.x` | `1.2.0.x` |
| Installer | `Major.Minor.Patch` | `"1.2.0"` |
| Git tag | `vMajor.Minor.Patch` | `v1.2.0` |

All four must be updated together for a release.

---

## Rollback Procedure

If a release has a critical bug:

1. Fix the issue on `master`
2. Bump the patch version (e.g., `1.2.0` → `1.2.1`)
3. Tag and push `v1.2.1`
4. Previous release remains on GitHub; users download the new one

**Do not delete published releases.** GitHub releases are immutable public artifacts.

---

## Verification Checklist

Before tagging a release:

- [ ] `CHANGELOG.md` has a `[version]` section with all changes listed
- [ ] `setup.iss` `MyAppVersion` matches the version being released
- [ ] All three `AssemblyInfo` versions are updated (Assembly, File, Info)
- [ ] `.csproj` versions match `AssemblyInfo.cs`
- [ ] CI build passes on `master` (check Actions tab)
- [ ] All Maat findings are cured (run `python3 maat.py` if available locally)
- [ ] No secrets, keys, or `.pfx` files in the commit
- [ ] `.gitignore` excludes build artifacts and IDE files

---

*This document describes the pipeline as implemented. If the pipeline changes, update this file in the same commit.*
