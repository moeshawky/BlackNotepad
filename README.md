# BlackNotepad Community Fork 📝

> A modern, dark-themed text editor for Windows with file association handling and UTF-8 encoding.

---

**Fork Notice:** This is a community-maintained fork of [savaged/BlackNotepad](https://github.com/savaged/BlackNotepad) (upstream archived since 2024). Issues and releases are handled here.

[![License: GPL v2](https://img.shields.io/badge/License-GPLv2-blue.svg)](LICENSE)
[![.NET Framework](https://img.shields.io/badge/.NET-4.7.2-yellow.svg)](https://dotnet.microsoft.com/download/dotnet-framework/net472)
[![Status](https://img.shields.io/badge/status-active-brightgreen)]()

---

## 🎯 What You Get

A clean, fast text editor that remembers your preferences:

```
┌─────────────────────────────────────────────────────┐
│  BlackNotepad                              ─ □ x    │
├─────────────────────────────────────────────────────┤
│  File  Edit  Format  View  Help                     │
│  ┌───────────────────────────────────────────────┐  │
│  │                                               │  │
│  │   Your text here...                           │  │
│  │                                               │  │
│  └───────────────────────────────────────────────┘  │
│  Status: Ln 1, Col 1     UTF-8     100%           │
└─────────────────────────────────────────────────────┘
```

### Key Features

- ✅ **File Association Fix** - Double-clicked .txt files open correctly
- ✅ **Dark Theme** - Clean, dark-themed interface
- ✅ **Window Persistence** - Remembers font settings, zoom, and word wrap
- ✅ **UTF-8 Encoding** - Standard text file support
- ✅ **Extended Formats** - Standard text files — .txt and all file types
- ✅ **Custom Colors** - Font color selection
- ✅ **Zoom Levels** - Multiple zoom levels from 8pt to 600%
- ✅ **Print Support** - Print documents directly (Ctrl+P)
- ✅ **Line Numbers** - Toggleable gutter synced with scrolling
- ✅ **Faster Startup** - Deferred font loading, low-overhead typing

---

## ⚡ Quick Start

### Download & Run

1. Download `BlackNotepad-Setup-*.exe` from [GitHub Releases](../../releases)
2. Run the installer (requires .NET Framework 4.7.2 or later)
3. Launch BlackNotepad from the Start menu

### Set as Default

```
Right-click any .txt file → Open With → Choose another app
→ Check "Always use this app to open .txt files"
→ Select BlackNotepad
```

---

## 🏗️ Build from Source

### Prerequisites

| Requirement | Version | Notes |
|:------------|:--------|:------|
| Windows | 10 or 11 | Required for WPF |
| .NET Framework | 4.7.2 or later | [Download](https://dotnet.microsoft.com/download/dotnet-framework/net472) |
| Visual Studio Build Tools | 2022 | Includes MSBuild |

### Build Commands

```powershell
# Navigate to project directory
cd BlackNotepad-master

# Build Release
msbuild src\BlackNotepad.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.7.2

# Output: src\bin\Release\BlackNotepad.exe
```

---

## 📦 Create Release Packages

Releases are built by CI from version tags
(see `.github/workflows/build.yml`). To build the installer
locally with Inno Setup installed:

```powershell
iscc setup.iss

# Output: BlackNotepad-Setup-<version>.exe
```

---

## 📝 Usage Guide

### File Menu

| Shortcut | Action |
|:---------|:-------|
| Ctrl+N | New file |
| Ctrl+O | Open file |
| Ctrl+S | Save file |
| Ctrl+Shift+S | Save as |
| Ctrl+P | Print |

### Edit Menu

| Shortcut | Action |
|:---------|:-------|
| Ctrl+F | Find |
| Ctrl+H | Replace |
| Ctrl+G | Go to line |

### Format Menu

- Word wrap toggle
- Font color selection
- Font family selection
- Font zoom (Ctrl++ / Ctrl+- / Ctrl+0)

### View Menu

- Status bar toggle

---

## 🔧 Configuration

### Settings Location

```
%LOCALAPPDATA%\BlackNotepad.ViewState.json
```

---

## 🤝 Contributing

Contributions welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

### Reporting Issues

- [Bug Report](../../issues/new?template=01_BUG_REPORT.md)
- [Feature Request](../../issues/new?template=02_FEATURE_REQUEST.md)

---

## 📄 License

This project is licensed under **GNU GPL v2** - see [LICENSE](LICENSE) for details.

Based on [savaged/BlackNotepad](https://github.com/savaged/BlackNotepad) (archived).

### Third-Party Libraries

| Library | License | Purpose |
|---------|---------|---------|
| MvvmLight | MIT | MVVM framework |
| Newtonsoft.Json | MIT | JSON processing |
| CommonServiceLocator | MS-PL | Service locator |

---

## 🙏 Acknowledgments

- Original [savaged/BlackNotepad](https://github.com/savaged/BlackNotepad) project
- [OpenCode](https://github.com/anomalyco/opencode) - AI-powered development environment
- [Minimax](https://www.minimaxi.com/) - Creators of the M2.1 AI model used in development

### AI Modernization

This fork has been modernized using AI assistance. Notable improvements include:
- File association fix (double-clicked files now open correctly)
- Find function bug fixes (position 0 and last occurrence handling)
- Multiple code quality improvements
- Enhanced documentation

AI Models used: Minimax M2.1

---

<div align="center">


[Website](https://github.com/moeshawky) · [Issues](../../issues) · [Releases](../../releases)

</div>
