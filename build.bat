@echo off

:: Change to the directory that this batch file is in
:: NB: it must be invoked with a full path!
for /f %%i in ("%0") do set curpath=%%~dpi
cd /d %curpath%

:: Fetch input parameters
set target=%1
set build.config=%2

:: If nothing is specified, run the default target
if "%target%"=="" set target=default

:: If no config is specified, run a debug build
if "%build.config%"=="" set build.config=release

:: Execute the boo script with params - accessible with e.g. env("build.config")
Libraries\phantom\phantom.exe -f:build.boo %target% -a:build.config=%build.config%