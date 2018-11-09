$json = (Get-Content 'CodeGenerator\config.json') -join "`n" | ConvertFrom-Json
$outputDirectory = $json | Select -expand OutputDirectory
$destinationPath = $json | Select -expand DestinationDirectory
$connectionString = $json | Select -expand ConnectionString

$connectionStringLen = ($connectionString).Length
$serverParam = "Server="
$serverParamLen = ($serverParam).Length
$serverPosStart = $connectionString.IndexOf($serverParam) + $serverParamLen
$serverPosEnd = $connectionString.IndexOf(';')
$server = $connectionString.Substring($serverPosStart, $serverPosEnd - $serverPosStart)

$catalogParam = "Initial Catalog="
$catalogParamLen = ($catalogParam).Length
$catalogPosStart = $connectionString.IndexOf($catalogParam) + $catalogParamLen
$catalogPosEnd = $connectionString.Substring($catalogPosStart, $connectionStringLen - $catalogPosStart).IndexOf(';') + $catalogPosStart
$database = $connectionString.Substring($catalogPosStart, $catalogPosEnd - $catalogPosStart)

$path = "$outputDirectory\Business\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Data\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Models\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Tests\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Database\Drop"
$command = "CodeGenerator\Scripts\runSQL.ps1"
Invoke-Expression "$command -server $server -database $database -path $path"

$path = "$outputDirectory\Database\Functions"
 Invoke-Expression "$command -server $server -database $database -path $path"

$path = $outputDirectory + '\Database\Stored` Procedures'
 Invoke-Expression "$command -server $server -database $database -path $path"

Read-Host -Prompt "Press Enter to exit"