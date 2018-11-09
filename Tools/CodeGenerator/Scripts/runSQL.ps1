param([string]$server, [string]$database, [string]$path)

#only currently works with integrated security
#feel free to update this

foreach ($f in Get-ChildItem -path $path -Filter *.sql){ 
    try {
        invoke-sqlcmd -Server $server -Database $database -InputFile $f.fullname 
    } catch {
        Write-Host "SQL Exception";
        $Error | format-list -force;
    }
}