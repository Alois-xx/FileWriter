@echo off
REM with powershell you can add a Defender exclusion directory with Add-MpPreference -ExclusionPath "C:\temp\Test"
REM to check AV overhead with and without AV interference
set OutDir=C:\temp\Test1
call ProfileTest.cmd CreateSerial       FileWriter.exe -generate "%OutDir%" serial
call ProfileTest.cmd CreateParallel     FileWriter.exe -generate "%OutDir%" parallel
call ProfileTest.cmd CreateTaskParallel FileWriter.exe -generate "%OutDir%" taskparallel

call ExtractETW.cmd