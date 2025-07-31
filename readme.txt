# Building
cd c:\Windows\Microsoft.NET\Framework\v4.0.30319
MsBuild C:\Projects\DotService\PingService.csproj /p:Configuration=Release

xcopy "C:\Projects\DotService\bin\Release\PingService.exe" "C:\Program Files\PingService\" /I /Y & xcopy  "C:\Projects\DotService\bin\Release\PingService.config.xml" "C:\Program Files\PingService\" /I /Y

# Installation
InstallUtil.exe /u "C:\Projects\DotService\bin\Release\PingService.exe"

# Uninstallaltion
InstallUtil.exe /u "C:\Projects\DotService\bin\Release\PingService.exe"

