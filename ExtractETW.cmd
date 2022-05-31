@echo off
REM Extract all CPU data (-allcpu) which can make file size ca. 50% larger but then you see everything what WPA also sees
REM skip already exctracted files 
ETWAnalyzer -extract all -fd c:\temp\*Create*.etl -symserver ms -nooverwrite -allcpu
