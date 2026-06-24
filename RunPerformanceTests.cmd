@echo off
SETLOCAL EnableDelayedExpansion

REM with powershell you can add a Defender exclusion directory with Add-MpPreference -ExclusionPath "C:\temp\Test"
REM to check AV overhead with and without AV interference
set OutDir=C:\temp\Testdata
set ETWLocation=C:\temp\PerformanceETW

if "%1" NEQ "" set OutDir=%1
if "%2" NEQ "" set ETWLocation=%2

echo Writing data to !OutDir! and saving ETW data to !ETWLocation!. You can change both by passing the output folder as first and the ETW folder as second argument to this script.
mkdir !ETWLocation! 2> NUL

call :CleanFiles
call ProfileTest.cmd !ETWLocation! CreateSerial      FileWriter.exe -write   -folder "%OutDir%" -threads 1 -files 1000
call :CleanFiles
call ProfileTest.cmd !ETWLocation! CreateParallel    FileWriter.exe -write   -folder "%OutDir%" -threads all-1 -files 1000
call :CleanFiles
call ProfileTest.cmd !ETWLocation! CompileSerial     FileWriter.exe -compile -folder "%OutDir%" -threads 1 -files 500
call ProfileTest.cmd !ETWLocation! ExecSerial        FileWriter.exe -execute -folder "%OutDir%" -threads 1 
call :CleanFiles
call ProfileTest.cmd !ETWLocation! CompileParallel   FileWriter.exe -compile -folder "%OutDir%" -threads all-1 -files 500
call ProfileTest.cmd !ETWLocation! ExecParallel      FileWriter.exe -execute -folder "%OutDir%" -threads all-1
call :CleanFiles


call ExtractETW.cmd !ETWLocation! Create
call ExtractETW.cmd !ETWLocation! Compile
call ExtractETW.cmd !ETWLocation! Exec
goto :EOF

:CleanFiles
for %%i in (%OutDir%\*.cmd) do del %%i
for %%i in (%OutDir%\*.exe) do del %%i
goto :EOF