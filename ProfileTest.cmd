@echo off
SETLOCAL EnableDelayedExpansion
set OutDir=!ETWLocation!
mkdir !OutDir! 2> NUL
 
if "%1" EQU "" (
	echo ProfileTest.cmd TestcaseName YourApp.exe ...
	echo   This will start profiling, wait a bit, and then run your executable with the arguments passed to your application.
	echo   The contract is that the called executable returns the measured duration as milliseconds in its return value when it exits. 
	echo Usage:
	echo ProfileTest.cmd TestCase1 YourApp.exe -Run ....
	echo   This will generate in !OutDir! a file named TestCase1_ddddmsYourMachineName.etl where dddd is the return value of YourApp.exe.
	goto :EOF
)

REM By default use the latest one from the Windows SDK
set WPRLocation=C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit\wpr.exe
set ScriptLocation=%~dp0
set TestCase=%1

REM otherwise use the one supplied by the OS, which might have bugs
if NOT EXIST "!WPRLocation!" set WPRLocation=wpr.exe
echo Testcase !TestCase!
shift
REM cancel any previously started session
"!WPRLocation!" -cancel 2> NUL

REM Start CPU profiling (which is includes in the Default Profile) and add File Acccess and Context Switch (Thread Wait times) data
"!WPRLocation!" -start "!ScriptLocation!MultiProfile.wprp"^^!File -start "!ScriptLocation!MultiProfile.wprp"^^!CSwitch -start "!ScriptLocation!MultiProfile.wprp"^^!PMCLLC

REM When we measure performance we need to be able to differentiate a system state which did already exist before the performance test was executed
REM an easy way is to wait 10s before and after the test to also capture potential asynchronous pending work which is still executing
echo Wait 3s for system to settle down
call :Wait 3

echo Calling: %1 %2 %3 %4 %5 %6 %7 %8 %9 
%1 %2 %3 %4 %5 %6 %7 %8 %9 

REM By convention we get the measured performance number back as the return code. That keeps the script simple
set ElapsedTimeInMs=%ErrorLevel%
echo Elapsed time by Return Code: !ElapsedTimeInMs!

echo Wait 3s before profiling is stopped to capture potential async operations which are still running
call :Wait 3

Echo Stop Profiling 
set StopTime=%TIME::=_%
set StopTime=!StopTime:,=_!
set StopTime=!StopTime:.=_!

set OutputFileName=!OutDir!\!TestCase!_!ElapsedTimeInMs!ms!ComputerName!.etl
"!WPRLocation!" -stop !OutputFileName! -skipPdbGen
REM echo WPR Return code: %Errorlevel%

if "%ERRORLEVEL%" EQU "-2147417850" ( 
	echo Encountered WPR bug: Cannot change thread mode after it is set.
	echo Cancel profiling. You need to download a newer Windows Performance Toolkit version from the Windows 11 SDK and put it before the default one in the PATH environment variable
	wpr -cancel
)

goto :EOF

REM the timeout command fails if it is called with redirected stdin which is frequently the case in build integration environments 
REM instead we use ping
:Wait
ping 127.0.0.1 -n %1 > NUL
goto :EOF