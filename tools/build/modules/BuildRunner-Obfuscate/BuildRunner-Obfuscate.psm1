<# 
    .SYNOPSIS

        Obfuscate tool for build runner

        creation date: 05/20/2016    
#>

Import-Module BuildRunner-Log

$ModuleName = 'OBFUSCATE'

function Use-Obfuscator($session)
{
   Write-LogInfo $ModuleName "Obfuscating $($session.Source)..."
   Write-LogInfo $ModuleName "Include filter: $($session.ObfuscatorIncludeFilter)"
   Write-LogInfo $ModuleName "Exclude filter: $($session.ObfuscatorExcludeFilter)"

    $assemblies = @()

    $session.ObfuscatorIncludeFilter -split ',' | ForEach-Object {
        $filteredAssemblies = Get-ChildItem -Path $session.Source -Filter $_ -Recurse | ForEach-Object {
            if(-Not $_.FullName.Contains($session.ObfuscatorExcludeFilter))
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

        Write-LogInfo $ModuleName "Executing: $($session.Obfuscator) $args"

        &$session.Obfuscator $args | Out-File 'obfuscator.log'

        if($LastExitCode -ne 0)
        {
            Write-LogError $ModuleName "$assembly wasn't obfuscated. Exit code: $LastExitCode"
            throw ""
        }
       
        Write-LogInfo $ModuleName "$assembly was obfuscated."
    }
}

Export-ModuleMember Use-Obfuscator