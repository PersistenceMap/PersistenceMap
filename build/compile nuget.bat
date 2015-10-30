@echo off

echo Begin create nuget for PersistenceMap
nuget.exe pack ..\src\PersistenceMap.nuspec

echo Begin create nuget for PersistenceMap.Sqlite
nuget.exe pack ..\src\PersistenceMap.Sqlite.nuspec

echo End create nuget for PersistenceMap
pause