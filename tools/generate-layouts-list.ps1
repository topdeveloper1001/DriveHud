$folder = 'd:\Git\DriveHUD\DriveHUD.Common.Resources\Layouts\New\'
$prefix = 'DriveHUD.Common.Resources.Layouts'

$result = ""
 
Get-ChildItem -Path $folder -Filter *.xml | ForEach-Object {
    $result += """$prefix.$($_.Name)"","
}

Write-Output $result