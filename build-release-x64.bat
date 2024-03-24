@echo off
call cd src
call dotnet clean
call dotnet build -c Release -r win-x64
pause