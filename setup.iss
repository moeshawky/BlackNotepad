; Inno Setup script for BlackNotepad
; Produces: BlackNotepad-Setup.exe

#define MyAppName "BlackNotepad"
#define MyAppVersion "1.1.5"
#define MyAppPublisher "moeshawky"
#define MyAppURL "https://github.com/moeshawky/BlackNotepad"
#define MyAppExeName "BlackNotepad.exe"

[Setup]
AppId={{13552780-24AC-414F-99FB-BD2E3C368446}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=.
OutputBaseFilename=BlackNotepad-Setup-{#MyAppVersion}
SetupIconFile=src\logo.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Files]
Source: "src\bin\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\bin\Release\*.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\bin\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\logo.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\logo.ico"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; IconFilename: "{app}\logo.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch BlackNotepad"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet472Installed: Boolean;
var
  Success: Boolean;
  Release: Cardinal;
begin
  Success := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release);
  Result := Success and (Release >= 461808);
end;

function InitializeSetup: Boolean;
begin
  if not IsDotNet472Installed then
  begin
    MsgBox('BlackNotepad requires .NET Framework 4.7.2 or later.'#13#13
           'Please install it from Microsoft before continuing.',
           mbCriticalError, MB_OK);
    Result := False;
  end
  else
    Result := True;
end;
