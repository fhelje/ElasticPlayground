@echo off
REM we need nuget to install tools locally
build\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

build\paket.exe %*