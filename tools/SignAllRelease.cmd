@echo off
PowerShell -NoProfile -ExecutionPolicy remotesigned  -Command "./sign_assemblies.ps1 -mode Release"
pause