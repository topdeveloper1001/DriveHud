<# 
    .SYNOPSIS

        Obfuscate tool for build runner

        creation date: 05/20/2016    
#>

Import-Module BuildRunner-Log

$ModuleName = 'OBFUSCATE'

function Use-Obfuscator($session, $source, $includeFilter, $excludeFilter, $strongNameAssemblies)
{    
   Write-LogInfo $ModuleName "Obfuscating $source..."
   Write-LogInfo $ModuleName "Include filter: $includeFilter"
   Write-LogInfo $ModuleName "Exclude filter: $excludeFilter"

    $assemblies = @()

    $includeFilter -split ',' | ForEach-Object {
        $filteredAssemblies = Get-ChildItem -Path $source -Filter $_ -Recurse | ForEach-Object {
            if(-Not $_.FullName.Contains($excludeFilter))
            {
               $assemblies += $_
            }        
        }
    }

    $assembliesArg = $assemblies -join ' '
    
    foreach($assembly in $assemblies)
    {        
        $args = @(
            '-file',
            $assembly.FullName,
            '-targetfile',
            $assembly.FullName,
            '-suppressildasm',
            '1',
            '-stringencryption',
            '1',
            '-obfuscation',
            '1'                             
        )

        if($strongNameAssemblies.Contains($assembly.Name))
        {
            $args += '-snkeypair' 
            $args += $session.StrongNameKey
            $args += '-snpassword'
            $args += $session.StrongNamePassword
        }

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