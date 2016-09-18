param
(
    [ValidateSet('Debug','Release')]
    [string] $mode = 'Release',

    [string] $certificate = '../Certificates/APSCertificate.pfx',

    [string] $password = 'backup',

    [string] $path = '..\DriveHUD.Application\bin\',

    [string] $filters = 'DriveHUD.*.exe,DriveHUD.*dll,Model.dll,HandHistories.Parser.dll,HandHistories.Objects.dll,CapPipedB.dll,CapPipedI.dll',

    [string] $exclude = 'vshost'
)

$ErrorActionPreference = "Stop"

$signTool = 'c:\Program Files (x86)\Windows Kits\10\bin\x64\signtool.exe'

if(-Not (Test-Path($signTool)))
{
    throw "Sign tool $signTool not found"
}

if(-Not (Test-Path($certificate)))
{
    throw "Certificate $certificate not found"
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
        'sign',
        '/fd',
        'SHA256',
        '/a',
        '/f',
        $certificate,
        '/p',
        $password,
        '/t',
        'http://timestamp.globalsign.com/scripts/timstamp.dll',
        $assembly
    )

    #Write-Output "Executing: $signTool $args"

    &$signTool $args
}