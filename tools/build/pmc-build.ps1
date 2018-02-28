<# 
    .SYNOPSIS

        Script is used to build DriveHUD.PMCatcher

        creation date: 09/20/2016      
#>

[CmdletBinding(SupportsShouldProcess)]
param
(   
    [string] $Version = '1.0.0',

    [string] $VersionIncludeFilter = '**DriveHUD.PMCatcher**,**PM*Reg**',

    [string] $VersionExlcudeFilter = '',
   
    [ValidateSet('Debug', 'Info', 'Notice', 'Warning', 'Error')]
    [string] $LogLevel = 'Debug',

    [int] $StartRevision = -1530  
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\include\Init.ps1"

[Environment]::CurrentDirectory = $script:BaseDir
Set-Location $script:BaseDir

$session = @{
  BaseDir = $BaseDir  
  Git = 'c:\Program Files\Git\bin\git.exe'  
  Version = $Version
  VersionIncludeFilter = $VersionIncludeFilter
  VersionExlcudeFilter = $VersionExlcudeFilter  
  StartRevision = $StartRevision 
}

Import-Module BuildRunner-Log
Import-Module BuildRunner-Tools
Import-Module BuildRunner-Versioning

# setup logging
if ($LogLevel)
{
    Set-LogLevel $LogLevel
}

try
{
   Write-LogInfo 'SETUP' 'Validating base tools...'
     
   if(-Not (Test-Path($session.Git)))
   {
       throw "Git not found '$($session.Git)'"
   }    
         
   # setup version
   Set-Version($session)   
   
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
    Remove-Module BuildRunner-Tools
    Remove-Module BuildRunner-Versioning    
}