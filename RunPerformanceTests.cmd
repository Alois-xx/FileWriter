@echo off
setlocal enabledelayedexpansion

REM set TestCase=NugetDownload
REM set OutDir=%userprofile%\.nuget\packages\FileWriterTestFolder

set TestCase=CreateParallel
set OutDir=%temp%\FileCreationTest

set ETWLocation=C:\Temp

setlocal enabledelayedexpansion
set ScriptLocation=%~dp0


REM First argument is directory where ETW files are written to. Default is C:\temp
REM Second argument is the number of concurrent ETW extractions. Default is up to 5 depending on core count. That can lead to extraction errors due to not enough 
REM memory. In that case add as second argument to the script 1 to try a slower but less memory hungry extraction.

if "%1" NEQ "" set ETWLocation=%1
if "%2" NEQ "" set ThreadCount=%2
echo Saving ETW Files to !ETWLocation!, Parallel Extractors: !ThreadCount!

REM Simulate Nuget Download to user local folder 

REM set Extension=.dll
REM call :CleanFiles
REM call !ScriptLocation!ProfileTest.cmd %TestCase%-%Extension:~1%  "!ScriptLocation!FileWriter.exe" -generate "%OutDir%" serial -files 500 -extension %Extension%
REM call :CleanFiles
REM 
REM set Extension=.exe
REM call !ScriptLocation!ProfileTest.cmd %TestCase%-%Extension:~1%  "!ScriptLocation!FileWriter.exe" -generate "%OutDir%" serial -files 500 -extension %Extension%
REM call :CleanFiles
REM 
REM set Extension=.ps1
REM call !ScriptLocation!ProfileTest.cmd %TestCase%-%Extension:~1%  "!ScriptLocation!FileWriter.exe" -generate "%OutDir%" serial -files 500 -extension %Extension%
REM call :CleanFiles

REM Simulate file writing of .cmd files to a folder which might be excluded by AV or not
set Extension=.cmd
call :CleanFiles
call ProfileTest.cmd CreateParallel     FileWriter.exe -generate "%OutDir%" parallel -extension %Extension%
call :CleanFiles
call ProfileTest.cmd CreateParallel2    FileWriter.exe -generate "%OutDir%" parallel -threads 2 -extension %Extension%
call :CleanFiles
call ProfileTest.cmd CreateParallel3    FileWriter.exe -generate "%OutDir%" parallel -threads 3 -extension %Extension%
call :CleanFiles
call ProfileTest.cmd CreateParallel4    FileWriter.exe -generate "%OutDir%" parallel -threads 4 -extension %Extension%
call :CleanFiles
call ProfileTest.cmd CreateParallel5    FileWriter.exe -generate "%OutDir%" parallel -threads 5 -extension %Extension%
call :CleanFiles

REM delete everything in output folder
if "%OutDir%" NEQ "" (
	rd /q /s "%OutDir%"
)

call !ScriptLocation!ExtractETW.cmd %TestCase%
REM ETWAnalyzer -dump CPU -pn FileWriter -stacktags *Virus* -methods RtlUserThreadStart -fd !ETWLocation!\Extract\*%TestCase%*.json7z
goto :EOF

:CleanFiles
for %%i in ("%OutDir%\*!Extension!") do del %%i
goto :EOF