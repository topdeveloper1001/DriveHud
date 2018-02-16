<# 
    .SYNOPSIS

        EditBin tools

        creation date: 02/16/2018

#>

Import-Module BuildRunner-Log

$ModuleName = 'EditBin'

function Use-EditBinApplyLargeAddress($session, $logfile)
{
	Write-LogInfo $ModuleName 'Running editbin...'

	$session.ExeToLargeAddressAware -split ',' | ForEach-Object {
		
		$file = Join-Path $session.Source $_
		
		$args = @(
			'/largeaddressaware',
			$file
		)
		
		Write-LogInfo $ModuleName "editbin $args"
		 
        &$session.EditBin $args | Out-File $logfile
		
		if($LastExitCode -ne 0)
		{
			Write-LogError $ModuleName "Failed to editbin $file. Please check the log. Exit code: $LastExitCode"
			throw ""
		}
		
		Write-LogInfo $ModuleName "Successfully editbin $file. Exit code: $LastExitCode"
   }     
}

Export-ModuleMember Use-EditBinApplyLargeAddress