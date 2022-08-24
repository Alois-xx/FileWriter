@echo off
REM with powershell you can add a Defender exclusion directory with Add-MpPreference -ExclusionPath "C:\temp\Test"
REM to check AV overhead with and without AV interference
set OutDir=C:\temp\Testdata

call :CleanFiles
call ProfileTest.cmd CreateSerial       FileWriter.exe -generate "%OutDir%" serial
call :CleanFiles
call ProfileTest.cmd CreateParallel     FileWriter.exe -generate "%OutDir%" parallel
call :CleanFiles
call ProfileTest.cmd CreateParallel2    FileWriter.exe -generate "%OutDir%" parallel -threads 2
call :CleanFiles
call ProfileTest.cmd CreateParallel3    FileWriter.exe -generate "%OutDir%" parallel -threads 3
call :CleanFiles
call ProfileTest.cmd CreateParallel4    FileWriter.exe -generate "%OutDir%" parallel -threads 4
call :CleanFiles
call ProfileTest.cmd CreateParallel5    FileWriter.exe -generate "%OutDir%" parallel -threads 5
call :CleanFiles
call ProfileTest.cmd CreateTaskParallel FileWriter.exe -generate "%OutDir%" taskparallel
call :CleanFiles

call ExtractETW.cmd
goto :EOF

:CleanFiles
for %%i in (%OutDir%\*.cmd) do del %%i
goto :EOF