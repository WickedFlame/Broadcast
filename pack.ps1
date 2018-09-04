$root = (split-path -parent $MyInvocation.MyCommand.Definition)

$version = [System.Reflection.Assembly]::LoadFile("$root\src\Broadcast\bin\Release\Broadcast.dll").GetName().Version

# Nuget Package for RC
$versionStr = "{0}.{1}.{2}-RC0{3}" -f ($version.Major, $version.Minor, $version.Build, $version.Revision)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\src\Broadcast.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\src\Broadcast.compiled.nuspec

& $root\build\NuGet.exe pack $root\src\Broadcast.compiled.nuspec

# Nuget Package for Release
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build, $version.Revision)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\src\Broadcast.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\src\Broadcast.compiled.nuspec

& $root\build\NuGet.exe pack $root\src\Broadcast.compiled.nuspec