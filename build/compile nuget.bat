@echo off

nuget.exe pack ..\src\PersistanceMap.nuspec

nuget.exe pack ..\src\PersistanceMap.Sqlite.nuspec

pause