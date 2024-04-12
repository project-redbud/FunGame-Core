@echo off
call dotnet clean
call dotnet build -c Release -r linux-x64
pause