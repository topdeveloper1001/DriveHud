@echo off
PowerShell -NoProfile -ExecutionPolicy remotesigned  -Command "%~dp0/sign_assemblies.ps1 -mode Debug -certificate %~dp0/../Certificates/APSCertificate.pfx -path %~dp0/../DriveHUD.Application\bin"
pause