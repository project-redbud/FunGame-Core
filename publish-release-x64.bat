@echo off
call dotnet clean
call dotnet publish -c Release -r win-x64
pause