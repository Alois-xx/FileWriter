@echo off
where /Q ETWAnalyzer
if %ERRORLEVEL% GTR 0 (
	echo Install latest ETWAnalyzer from https://github.com/Siemens-Healthineers/ETWAnalyzer/releases/ and put it in your path.
	goto :EOF
)
REM Extract all CPU data (-allcpu) which can make file size ca. 50% larger but then you see everything what WPA also sees
REM skip already exctracted files 
ETWAnalyzer -extract all -fd c:\temp\*Create*.etl -symserver ms -nooverwrite -allcpu
IF %ERRORLEVEL% LEQ 0 (
	echo If during extraction files could not be extracted due to OutOfMemoryException you can execute the ExtractETW script again to extract the missing files. 
	echo You can also add to the ETWAnalyzer call -nthreads 1 to force single threaded extraction.
