<# 
    .SYNOPSIS

        Script is used to build DriveHUD

        creation date: 09/20/2016      
#>

[CmdletBinding(SupportsShouldProcess)]
param
(
    [ValidateSet('Debug','Release')]
    [string] $Mode = 'Release',

    [string] $Source = 'DriveHUD.Application\bin',

    [string] $Solution = 'DriveHUD.sln',       
    
    [string] $InstallerWix = 'DriveHUD.Bootstrapper\DriveHUD.Bootstrapper.wixproj',    
    
    [string] $Version = '1.0.3',

    [string] $ObfuscatorIncludeFilter = 'DriveHUD.*.exe,DriveHUD.*dll,Model.dll',

    [string] $ObfuscatorExcludeFilter = 'vshost',

    [string] $SigningIncludeFilter = 'DriveHUD.*.exe,DriveHUD.*dll,Model.dll,HandHistories.Parser.dll,HandHistories.Objects.dll,CapPipedB.dll,CapPipedI.dll',

    [string] $SigningExcludeFilter = 'vshost',

    [string] $SigningCertificate = 'Certificates/APSCertificate.pfx',

    [string] $SigningPassword = 'backup',
    
    [ValidateSet('Debug', 'Info', 'Notice', 'Warning', 'Error')]
    [string] $LogLevel = 'Debug',

    [int] $StartRevision = 512
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\include\Init.ps1"

[Environment]::CurrentDirectory = $script:BaseDir
Set-Location $script:BaseDir

$session = @{
  BaseDir = $BaseDir
  Source = Join-Path $BaseDir (Join-Path $Source $Mode)
  Obfuscator = 'c:\Program Files (x86)\Eziriz\.NET Reactor\dotNET_Reactor.Console.exe'
  SignTool = 'c:\Program Files (x86)\Windows Kits\10\bin\x64\signtool.exe'
  Candle = 'C:\Program Files (x86)\WiX Toolset v3.10\bin\candle.exe'
  Light = 'C:\Program Files (x86)\WiX Toolset v3.10\bin\Light.exe'
  MSBuild = 'c:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe'
  Git = 'c:\Program Files\Git\bin\git.exe'
  Mode = $Mode
  Solution = Join-Path $BaseDir $Solution  
  InstallerWix = Join-Path $BaseDir $InstallerWix
  Version = $Version
  ObfuscatorIncludeFilter = $ObfuscatorIncludeFilter
  ObfuscatorExcludeFilter = $ObfuscatorExcludeFilter
  SigningIncludeFilter = $SigningIncludeFilter
  SigningExcludeFilter = $SigningExcludeFilter 
  SigningCertificate = Join-Path $BaseDir $SigningCertificate
  SigningPassword= $SigningPassword
  StartRevision = $StartRevision
  WixExtensions = 'dependencies\Wix\WixUIExtension.dll,dependencies\Wix\WixNetFxExtension.dll'
}

Import-Module BuildRunner-Log
Import-Module BuildRunner-Versioning
Import-Module BuildRunner-MSBuild
Import-Module BuildRunner-Obfuscate
Import-Module BuildRunner-Sign

# setup logging
if ($LogLevel)
{
    Set-LogLevel $LogLevel
}

try
{
   Write-LogInfo 'SETUP' 'Validating base tools...'
   
   if(-Not (Test-Path($session.Solution)))
   {
       throw "Solution not found '$($session.Solution)'"
   } 

   if(-Not (Test-Path($session.Obfuscator)))
   {
       throw "Obfuscator not found '$($session.Obfuscator)'"
   } 

   if(-Not (Test-Path($session.SignTool)))
   {
       throw "SignTool not found '$($session.SignTool)'"
   } 

   if(-Not (Test-Path($session.Candle)))
   {
       throw "Candle not found '$($session.Candle)'"
   }

   if(-Not (Test-Path($session.Light)))
   {
       throw "Light not found '$($session.Light)'"
   }

   if(-Not (Test-Path($session.MSBuild)))
   {
       throw "MSBuild not found '$($session.MSBuild)'"
   }

   if((-Not $session.SigningCertificate) -Or (-Not (Test-Path($session.SigningCertificate))))
   {
       throw "SigningCertificate not found '$($session.SigningCertificate)'"
   }

   if(-Not (Test-Path($session.Git)))
   {
       throw "Git not found '$($session.Git)'"
   }
  
   if(-Not (Test-Path($session.InstallerWix)))
   {
       throw "Installer wix not found '$($session.InstallerWix)'"
   }

   if(Test-Path($session.Source))
   {
       Write-LogInfo 'SETUP' 'Clearing source directory'
       Remove-Item -Path $session.Source -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
   }

   # setup version
   Set-Version($session)  
   
   # msbuild
   Use-MSBuild $session $session.Solution 'msbuild.log'

   # obfuscate
   Use-Obfuscator($session)

   # sign
   Use-Sign($session)

   # build wix installer   
   Use-MSBuild $session $session.InstallerWix 'msbuild_installer.log'      

   Write-LogInfo 'SETUP' 'Done.'
}
catch
{
    $ErrorMessage = $_.Exception.Message    
    Write-Host $ErrorMessage -ForegroundColor Red        
    
    exit(1)
}
finally
{
    # remove all used modules (for debugging purpose)
    Remove-Module BuildRunner-Log           
    Remove-Module BuildRunner-Versioning 
    Remove-Module BuildRunner-MSBuild
    Remove-Module BuildRunner-Obfuscate
    Remove-Module BuildRunner-Sign
}