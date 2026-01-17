# Release Packages

Release packages are available as GitHub Releases: https://github.com/moeshawky/BlackNotepad/releases

## Available Downloads

| File | Description |
|------|-------------|
| `BlackNotepad-Portable.zip` | Extract and run - no installation required |
| `BlackNotepad-Setup.exe` | Windows installer |

## Local Files

The `Distribution/Portable/` folder contains all files needed to run BlackNotepad without installation.

### Portable Distribution Contents

```
Distribution/Portable/
├── BlackNotepad.exe       # Main application
├── BlackNotepad.exe.config
├── GalaSoft.MvvmLight.*   # MVVM framework
├── Newtonsoft.Json.*      # JSON library
├── CommonServiceLocator.dll
├── System.Windows.Interactivity.dll
├── logo.ico / logo.png
└── README-PORTABLE.md
```

## Building from Source

### Prerequisites

- Windows 10/11
- .NET Framework 4.8+
- Visual Studio Build Tools 2022

### Build Commands

```batch
cd BlackNotepad-master
msbuild src\BlackNotepad.csproj /p:Configuration=Release /p:TargetFrameworkVersion=v4.8
```

Output will be in: `src\bin\Release\BlackNotepad.exe`
