<# 
    .SYNOPSIS

        Tools for signing

        creation date: 09/20/2016     
#>

Import-Module BuildRunner-Log

$ModuleName = 'SIGN'

function Use-Sign($session, $source, $signingIncludeFilter, $signingExcludeFilter)
{
    Write-LogInfo $ModuleName 'Signing assemblies...'   

    $assemblies = @()

    $signingIncludeFilter -split ',' | ForEach-Object {
        $filteredAssemblies = Get-ChildItem -Path $source -Filter $_ -Recurse | ForEach-Object {
            if(-Not $_.FullName.Contains($signingExcludeFilter))
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
            $session.SigningCertificate,
            '/p',
            $session.SigningPassword,
            '/t',
            'http://timestamp.globalsign.com/scripts/timstamp.dll',
            $assembly
        )        

        &$session.SignTool $args | Out-File 'sign.log'

        if($LastExitCode -ne 0)
        {
            Write-LogError $ModuleName "$assembly hasn't been signed. Exit code: $LastExitCode"
            throw ""
        }
       
        Write-LogInfo $ModuleName "$assembly has been signed."
    }
}

Export-ModuleMember Use-Sign