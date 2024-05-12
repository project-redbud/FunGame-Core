@echo off
call dotnet clean
call dotnet build -c Release
pause