$path = (get-item $PSScriptRoot).parent.FullName
write-host $path
$tag = 'chesswebsite:latest'
docker build -t $tag $path