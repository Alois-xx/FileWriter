@echo off
SETLOCAL EnableDelayedExpansion
set ScriptLocation=%~dp0
if EXIST "!ScriptLocation!ETWAnalyzer\ETWAnalyzer.exe" (
	set ETWAnalyzer=!ScriptLocation!ETWAnalyzer\ETWAnalyzer.exe
) ELSE (
	where /Q ETWAnalyzer
	if %ERRORLEVEL% GTR 0 (
		echo Install latest ETWAnalyzer from https://github.com/Siemens-Healthineers/ETWAnalyzer/releases/ and put it in your path, or deploy ETWAnalyzer in a subfolder of the script named ETWAnalzyer.
		goto :EOF
	)
	set ETWAnalyzer=ETWAnalyzer
)

set ETWLocation=%1
set TestCase=%2

REM Extract all CPU data (-allcpu) which can make file size ca. 50% larger but then you see everything what WPA also sees
REM skip already exctracted files 

set ThreadArgs=
if "!ThreadCount!" NEQ "" set ThreadArgs=-nthreads !ThreadCount!

"!ETWAnalyzer!" -extract all -fd !ETWLocation!\*%TestCase%*.etl -symserver ms -nooverwrite !ThreadArgs!
IF %ERRORLEVEL% LSS 0 (
	echo If during extraction files could not be extracted due to OutOfMemoryException you can execute the RunPerformanceTests.cmd  
	echo with RunPerformanceTests c:\temp\ETW 1 to limit extraction to one parallel instance during extraction. The first argument is the output folder where 
	echo the first argument is the ETW data folder and second argument is the number of parallel extractor instances. 
)

goto :EOF