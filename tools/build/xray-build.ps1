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

    [string] $Source = 'DriveHUD.PlayerXRay\bin',
	
    [string] $Solution = 'PlayerXRay.sln',                             
    
    [string] $Version = '1.0.0',

    [string] $ObfuscatorIncludeFilter = 'DriveHUD.PlayerXRay.dll',

    [string] $ObfuscatorStrongNamedAssemblies = '',

    [string] $ObfuscatorExcludeFilter = 'vshost',

    [string] $SigningIncludeFilter = 'DriveHUD.PlayerXRay.dll',
	
    [string] $SigningExcludeFilter = 'vshost',

    [string] $SigningCertificate = 'Certificates/APSCertificate.pfx',

    [string] $SigningPassword = 'backup',

    [string] $StrongNameKey = 'Certificates/lic-assemb-sign.pfx',

    [string] $StrongNamePassword = 'backup1',
    
    [ValidateSet('Debug', 'Info', 'Notice', 'Warning', 'Error')]
    [string] $LogLevel = 'Debug',

    [int] $StartRevision = 512,

    [string] $LicSolution = 'PlayerXRayReg.sln',

    [string] $LicSource = 'XRCReg\bin',

    [string] $LicObfuscatorIncludeFilter = 'XR*Reg.dll',        

    [string] $LicProjectsToUpdate = 'DriveHUD.PlayerXRay\DriveHUD.PlayerXRay.csproj',

    [string] $LicCSFileToUpdate = 'DriveHUD.PlayerXRay\PlayerXRayModule.cs',

    [string] $LicOutputPath = 'dependencies',

    [string] $HashToolSolution = 'tools\BuildFileHash\BuildFileHash.sln',

    [string] $HashToolPath = 'tools\BuildFileHash\BuildFileHash\bin',

    [string] $HashTool = 'BuildFileHash.exe',

    [bool] $UpdateOnlyLic = $false
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
  Candle = 'C:\Program Files (x86)\WiX Toolset v3.11\bin\candle.exe'
  Light = 'C:\Program Files (x86)\WiX Toolset v3.11\bin\Light.exe'
  Insignia = 'C:\Program Files (x86)\WiX Toolset v3.11\bin\insignia.exe'
  MSBuild = 'c:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe'
  Nuget = '.\.nuget\Nuget.exe'
  Git = 'c:\Program Files\Git\bin\git.exe'
  Mode = $Mode
  Solution = Join-Path $BaseDir $Solution  
  Version = $Version
  ObfuscatorIncludeFilter = $ObfuscatorIncludeFilter
  ObfuscatorExcludeFilter = $ObfuscatorExcludeFilter
  ObfuscatorStrongNamedAssemblies = $ObfuscatorStrongNamedAssemblies
  SigningIncludeFilter = $SigningIncludeFilter  
  SigningExcludeFilter = $SigningExcludeFilter 
  SigningCertificate = Join-Path $BaseDir $SigningCertificate
  SigningPassword= $SigningPassword
  StrongNameKey = Join-Path $BaseDir $StrongNameKey
  StrongNamePassword = $StrongNamePassword
  StartRevision = $StartRevision
  WixExtensions = 'dependencies\Wix\WixUIExtension.dll,dependencies\Wix\WixNetFxExtension.dll'
  LicSolution = Join-Path $BaseDir $LicSolution
  LicSource = Join-Path $BaseDir (Join-Path $LicSource $Mode)
  LicObfuscatorIncludeFilter = $LicObfuscatorIncludeFilter      
  LicProjectsToUpdate = Join-Path $BaseDir $LicProjectsToUpdate
  LicCSFileToUpdate = Join-Path $BaseDir $LicCSFileToUpdate
  LicOutputPath = Join-Path $BaseDir $LicOutputPath
  HashToolSolution = Join-Path $BaseDir $HashToolSolution  
  HashTool = Join-Path $BaseDir (Join-Path $HashToolPath (Join-Path $Mode $HashTool)) 
}

Import-Module BuildRunner-Log
Import-Module BuildRunner-Tools
Import-Module BuildRunner-Versioning
Import-Module BuildRunner-MSBuild
Import-Module BuildRunner-Nuget
Import-Module BuildRunner-Obfuscate
Import-Module BuildRunner-Sign
Import-Module BuildRunner-SignWixBundle
Import-Module BuildRunner-LicUpdater

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

   if(-Not (Test-Path($session.LicSolution)))
   {
       throw "Solution not found '$($session.LicSolution)'"
   } 

   if(-Not (Test-Path($session.HashToolSolution)))
   {
       throw "Solution not found '$($session.HashToolSolution)'"
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

   if(-Not (Test-Path($session.Nuget)))
   {
       throw "Nuget not found '$($session.Nuget)'"
   }

   if(Test-Path($session.Source))
   {
       Write-LogInfo 'SETUP' 'Clearing source directory'
       Remove-Item -Path $session.Source -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
   }
     
   # setup version
   Set-Version($session)  
       
   # nuget
   Use-Nuget $session $session.Solution 'nuget.log'   

   # build hash tools
   Use-MSBuild $session $session.HashToolSolution 'hashtool-msbuild.log'

   # build lic dlls
   Use-MSBuild $session $session.LicSolution 'lic-msbuild.log'

   # obfuscate lic dlls
   Use-Obfuscator $session $session.LicSource $session.LicObfuscatorIncludeFilter '' $session.LicObfuscatorIncludeFilter

   # sign lic dlls
   Use-Sign $session $session.LicSource $session.LicObfuscatorIncludeFilter ''

   # copy lic dlls
   &robocopy $session.LicSource $session.LicOutputPath $session.LicObfuscatorIncludeFilter /s | Out-Null

   # update license dll hashes and version in specified projects
   Use-LicUpdater($session)
   
   if($UpdateOnlyLic)
   {
        Write-LogInfo 'SETUP' 'Done.'
        exit(0)
   }

   # msbuild
   Use-MSBuild $session $session.Solution 'msbuild.log'

   # obfuscate
   Use-Obfuscator $session $session.Source $session.ObfuscatorIncludeFilter $session.ObfuscatorExcludeFilter $session.ObfuscatorStrongNamedAssemblies

   # sign
   Use-Sign $session $session.Source $session.SigningIncludeFilter $session.SigningExcludeFilter
        
   Write-LogInfo 'SETUP' 'Done.'
   
   Write-Host "Press any key to continue ..."

   $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")   
}
catch
{
    $ErrorMessage = $_.Exception.Message    
    Write-Host $ErrorMessage -ForegroundColor Red        
    
	Write-Host "Press any key to continue ..."

	$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	
    exit(1)
}
finally
{
    # remove all used modules (for debugging purpose)
    Remove-Module BuildRunner-Log           
    Remove-Module BuildRunner-Tools
    Remove-Module BuildRunner-Versioning 
    Remove-Module BuildRunner-MSBuild
    Remove-Module BuildRunner-Obfuscate
    Remove-Module BuildRunner-LicUpdater
    Remove-Module BuildRunner-Sign
	Remove-Module BuildRunner-SignWixBundle
}