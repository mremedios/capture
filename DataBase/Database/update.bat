:GETOPTS
 if /I %~1 == --env set ENV=%~2& shift
 if /I %~1 == --password set PASS=%~2& shift
 shift
if not (%1)==() goto GETOPTS

@echo off
if "%OS%" == "Windows_NT" setlocal

setlocal enabledelayedexpansion

rem %~dp0 is expanded pathname of the current script under NT
rem %~p0 is the directory of the current script

if exist "%~dp0\liquibase\liquibase.bat" SET LIQUIBASE_HOME=%~dp0\liquibase

if "%LIQUIBASE_HOME%"=="" (
   FOR /F "tokens=* USEBACKQ" %%g IN (`where liquibase.bat`) do (SET "LIQUIBASE_HOME=%%~dpg")
)

if "%LIQUIBASE_HOME%"=="" (
     echo "Must set LIQUIBASE_HOME environment variable, or have liquibase.bat in your PATH"
     exit /B 1
)

"%LIQUIBASE_HOME%\liquibase.bat"  --logLevel=Info --defaultsFile=%ENV%.properties --password=%PASS% update
