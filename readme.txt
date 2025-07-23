# Building
cd c:\Windows\Microsoft.NET\Framework\v4.0.30319
MsBuild C:\Projects\DotService\PingService.csproj /p:Configuration=Release


# Installation
InstallUtil.exe /u "C:\Projects\DotService\bin\Release\PingService.exe"

# Uninstallaltion
InstallUtil.exe /u "C:\Projects\DotService\bin\Release\PingService.exe"
