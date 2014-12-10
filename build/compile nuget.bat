@echo off

echo Begin create nuget for PersistanceMap
nuget.exe pack ..\src\PersistanceMap.nuspec

echo Begin create nuget for PersistanceMap.Sqlite
nuget.exe pack ..\src\PersistanceMap.Sqlite.nuspec

echo End create nuget for PersistanceMap
pause