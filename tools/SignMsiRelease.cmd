@echo off
PowerShell -NoProfile -ExecutionPolicy remotesigned  -Command "./sign_assemblies.ps1 -mode Release" -path %~dp0/../DriveHUD.Setup/bin" -filters DriveHUD.msi

pause