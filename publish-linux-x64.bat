@echo off
call cd src
call dotnet clean
call dotnet publish -c Release -r linux-x64
pause