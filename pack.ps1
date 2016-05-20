$root = (split-path -parent $MyInvocation.MyCommand.Definition)

$version = [System.Reflection.Assembly]::LoadFile("$root\src\PersistenceMap\bin\Release\PersistenceMap.dll").GetName().Version
$versionStr = "{0}.{1}.{2}-RC0{3}" -f ($version.Major, $version.Minor, $version.Build, $version.Revision)

Write-Host "Setting PersistenceMap.nuspec version tag to $versionStr"
$content = (Get-Content $root\src\PersistenceMap.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\src\PersistenceMap.compiled.nuspec

& $root\build\NuGet.exe pack $root\src\PersistenceMap.compiled.nuspec

Write-Host "Setting PersistenceMap.Sqlite.nuspec version tag to $versionStr"
$content = (Get-Content $root\src\PersistenceMap.Sqlite.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\src\PersistenceMap.Sqlite.compiled.nuspec

& $root\build\NuGet.exe pack $root\src\PersistenceMap.Sqlite.compiled.nuspec