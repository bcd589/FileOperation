@echo off
echo 文件类型批量管理工具 - 自动打包脚本
echo ====================================
echo.

:: 检查Inno Setup是否安装
set INNO_COMPILER="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if not exist %INNO_COMPILER% (
    echo 错误: 未找到Inno Setup编译器。
    echo 请安装Inno Setup 6，然后再运行此脚本。
    echo 下载地址: https://jrsoftware.org/isdl.php
    pause
    exit /b 1
)

:: 检查图标文件
if not exist "FileOperation\AppIcon.ico" (
    echo 警告: 未找到AppIcon.ico文件。
    echo 请先将AppIcon.svg转换为AppIcon.ico，并放置在FileOperation目录下。
    echo 可以使用在线转换工具如https://convertio.co/svg-ico/
    echo.
    set /p CONTINUE=是否继续打包过程? (Y/N): 
    if /i "%CONTINUE%" neq "Y" exit /b 1
)

:: 创建输出目录
if not exist Installer mkdir Installer

:: 发布应用程序
echo 正在发布应用程序...
dotnet publish FileOperation\FileOperation.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
if %ERRORLEVEL% neq 0 (
    echo 发布应用程序失败。
    pause
    exit /b 1
)

:: 编译安装程序
echo 正在编译安装程序...
%INNO_COMPILER% Setup.iss
if %ERRORLEVEL% neq 0 (
    echo 编译安装程序失败。
    pause
    exit /b 1
)

echo.
echo 打包完成！安装程序已生成在Installer目录中。
echo.

if exist "Installer\FileOperationSetup.exe" (
    echo 安装程序路径: %CD%\Installer\FileOperationSetup.exe
    set /p RUN_INSTALLER=是否运行安装程序? (Y/N): 
    if /i "%RUN_INSTALLER%"=="Y" start "" "Installer\FileOperationSetup.exe"
)

pause