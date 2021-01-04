$basepath = (get-item $PSScriptRoot).parent.FullName
$targetpath = "$basepath\site\scripts\lib"
write-host $basepath
$path = "$basepath\node_modules"

$xs = @("requirejs\require.js", "requirejs-text\text.js")
foreach ($x in $xs) {
    $src = "$path\$x"
    $target = "$targetpath\$x"
    $null = New-Item -Path $target -Type File -Force
    Copy-Item -Path $src -Destination $target -Recurse -Force
}