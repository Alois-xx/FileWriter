@echo off

REM with powershell you can add a Defender exclusion directory with Add-MpPreference -ExclusionPath "C:\temp\Test"
REM to check AV overhead with and without AV interference
set OutDir=C:\temp\Testdata

if "%1" NEQ "" set OutDir=%1

call :CleanFiles
call ProfileTest.cmd CreateSerial      FileWriter.exe -write   -folder "%OutDir%" -threads 1 -files 1000
call :CleanFiles
call ProfileTest.cmd CreateParallel    FileWriter.exe -write   -folder "%OutDir%" -threads all-1 -files 1000
call :CleanFiles
call ProfileTest.cmd CompileSerial     FileWriter.exe -compile -folder "%OutDir%" -threads 1 -files 500
call ProfileTest.cmd ExecSerial        FileWriter.exe -execute -folder "%OutDir%" -threads 1 
call :CleanFiles
call ProfileTest.cmd CompileParallel   FileWriter.exe -compile -folder "%OutDir%" -threads all-1 -files 500
call ProfileTest.cmd ExecParallel      FileWriter.exe -execute -folder "%OutDir%" -threads all-1
call :CleanFiles


call ExtractETW.cmd Create
call ExtractETW.cmd Compile
call ExtractETW.cmd Exec
goto :EOF

:CleanFiles
for %%i in (%OutDir%\*.cmd) do del %%i
for %%i in (%OutDir%\*.exe) do del %%i
goto :EOF