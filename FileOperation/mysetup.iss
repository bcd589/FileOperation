;汉化:MonKeyDu 
;由 Inno Setup 脚本向导 生成的脚本,有关创建 INNO SETUP 脚本文件的详细信息，请参阅文档！!

#define MyAppName "文件类型批量管理工具"
#define MyAppVersion "1.5"
#define MyAppPublisher "安绍丰"
#define MyAppURL "https://gitee.com/tao5cai/FileOperation"
#define MyAppExeName "FileOperation.exe"

[Setup]
;注意:AppId 的值唯一标识此应用程序。请勿在安装程序中对其他应用程序使用相同的 AppId 值。
;（若要生成新的 GUID，请单击“工具”|”在 IDE 中生成 GUID）。
AppId={{B4C2F9D1-EBC9-49B5-8FBB-0FA22EB4E745}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\FileOperation
UninstallDisplayIcon={app}\{#MyAppExeName}
DisableWelcomePage=no
DisableReadypage=yes
;下行注释，指定安装程序无法运行，除 Arm 上的 x64 和 Windows 11 之外的任何平台上.
ArchitecturesAllowed=x64compatible
;WizardImageFile=侧图186x356.bmp
;WizardSmallImageFile=顶图165x54.bmp,
;WizardSmallImageFile=顶图54x54.bmp
;下行注释，强制安装程序在 64 位系统上，但不强制以 64 位模式运行.
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
LicenseFile=C:\Users\anshaofeng\WPSDrive\972721572\WPS云盘\code\Csharp\FileOperation\FileOperation\LICENSE
;取消下行前面 ; 符号，在非管理安装模式下运行（仅为当前用户安装）.
;PrivilegesRequired=lowest
SetupIconFile=C:\Users\anshaofeng\WPSDrive\972721572\WPS云盘\code\Csharp\FileOperation\FileOperation\FileOperation\AppIcon.ico
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "chs"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone

[Files]
Source: "C:\Users\anshaofeng\WPSDrive\972721572\WPS云盘\code\Csharp\FileOperation\FileOperation\FileOperation\bin\Release\net9.0-windows\publish\win-x64\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\anshaofeng\WPSDrive\972721572\WPS云盘\code\Csharp\FileOperation\FileOperation\FileOperation\bin\Release\net9.0-windows\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[code]
procedure InitializeWizard();
begin
WizardForm.LICENSEACCEPTEDRADIO.checked:= true;
end;

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

