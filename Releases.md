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
â”œâ”€â”€ BlackNotepad.exe       # Main application
â”œâ”€â”€ BlackNotepad.exe.config
â”œâ”€â”€ GalaSoft.MvvmLight.*   # MVVM framework
â”œâ”€â”€ Newtonsoft.Json.*      # JSON library
â”œâ”€â”€ CommonServiceLocator.dll
â”œâ”€â”€ System.Windows.Interactivity.dll
â”œâ”€â”€ logo.ico / logo.png
â””â”€â”€ README-PORTABLE.md
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


## Version History

See [CHANGELOG.md](CHANGELOG.md) for a detailed list of changes.
