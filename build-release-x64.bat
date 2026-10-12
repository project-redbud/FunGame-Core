@echo off
call dotnet clean "%~dp0src\FunGame.Core.csproj" -c Release
if errorlevel 1 exit /b %errorlevel%
call dotnet build "%~dp0src\FunGame.Core.csproj" -c Release
if errorlevel 1 exit /b %errorlevel%
pause
