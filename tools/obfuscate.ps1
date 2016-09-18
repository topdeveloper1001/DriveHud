param
(
    [ValidateSet('Debug','Release')]
    [string] $mode = 'Release',    

    [string] $password = 'backup',

    [string] $path = '..\DriveHUD.Application\bin\',

    [string] $filters = 'DriveHUD.*.exe,DriveHUD.*dll,Model.dll',

    [string] $exclude = 'vshost'
)

Set-Location 'd:\AcePokerSolutions\DriveHUD\trunk\src\tools\'

$ErrorActionPreference = "Stop"

$reactor = 'c:\Program Files (x86)\Eziriz\.NET Reactor\dotNET_Reactor.exe'

if(-Not (Test-Path($reactor)))
{
    throw ".Net reactor $reactor not found"
}

$path = Join-Path -Path $path $mode

$assemblies = @()

$filters -split ',' | ForEach-Object {
    $filteredAssemblies = Get-ChildItem -Path $path -Filter $_ -Recurse | ForEach-Object {
        if(-Not $_.FullName.Contains($exclude))
        {
           $assemblies += $_.FullName
        }        
    }
}

$assembliesArg = $assemblies -join ' '

foreach($assembly in $assemblies)
{

    $args = @(
        '-file',
        $assembly,
        '-targetfile',
        $assembly,
        '-suppressildasm',
        '1',
        '-stringencryption',
        '1',
        '-obfuscation',
        '1'
    )

    #Write-Output "Executing: $reactor $args"

    &$reactor $args
}