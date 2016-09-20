<# 
    .SYNOPSIS

        Tools for test runner

        creation date: 05/20/2016
        author: alexander.danilov@aurea.com 

#>

Import-Module BuildRunner-Log

$ModuleName = 'WIX'

function Use-Wix($wixInfo)
{
    Write-LogInfo $ModuleName "Creating installer using $wixFile..."    

    $extArgs = @()

    if($session.WixExtensions)
    {
        $session.WixExtensions -split ',' | ForEach-Object {
            $extPath = Join-Path $session.BaseDir $_

            if($_)
            {                           
                if(-Not (Test-Path($extPath)))
                {
                    Write-LogError $ModuleName "Wix Extension hans't been found at $extPath"
                    throw ""
                }

                $extArgs += "-ext $extPath"                
            }
        }
    }

    $extArgs = $extArgs -join ' '

    $args = @(            
            "-dConfiguration=$($session.Mode)",            
            '-arch',
            'x86',
            $extArgs, 
            $wixFile
        )   
   
   Write-LogInfo $ModuleName "Executing $($session.Candle) $args"

   &$session.Candle $args
}



Export-ModuleMember Use-Wix