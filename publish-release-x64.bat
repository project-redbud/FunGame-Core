@echo off
call dotnet clean
call dotnet publish -c Release
pause