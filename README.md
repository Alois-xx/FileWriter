**FileWriter**

See [https://aloiskraus.wordpress.com/2022/07/25/pdd-profiler-driven-development/](https://aloiskraus.wordpress.com/2022/07/25/pdd-profiler-driven-development/) for full details.

It is a test application to show how one can integrate performance regression tests with ETW profiling and automated data extraction with ETWAnalyzer.

Compile the application with VS 2022 and execute from an administrator prompt *RunPerformanceTests.cmd*.

For automated ETW data analysis you need to download ETWAnalyzer from https://github.com/Siemens-Healthineers/ETWAnalyzer/releases
and put the ETWAnalyzer.exe into your path.

The used *MultiProfile.wprp* ETW profile is an example of most ETW features. If you want to author
your own recording profile it can serve as base for your own expeditions into uncharted territories of Windows.

FileWriter writes 5000 files into a folder where each file is 100 KB in size. 
It has 3 strategies implemented:
- Single Threaded
- Multi Threaded with Parallel.For
- Multi Threaded with TPL Tasks Task.Run

**Ask Your Data Questions**

- What is the fastest approach and what the most economical?
- Is something affecting our performance? 

What can we do to make things faster?

***Sample Output***
```
D:\Source\FileWriter\bin\Release\net6.0>RunPerformanceTests.cmd

Testcase CreateSerial
Wait 10s for system to settle down
Calling: FileWriter.exe -generate "C:\temp\Test1" serial
Did create 5000 files of 100 KB size (488) MB in 12,043s
Elapsed time by Return Code: 12042
Wait 10s before profiling is stopped to capture potential async operations which are still running
Stop Profiling
Press Ctrl+C to cancel the stop operation.
100%  [>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>]
The trace was successfully saved.

Testcase CreateParallel
Wait 10s for system to settle down
Calling: FileWriter.exe -generate "C:\temp\Test1" parallel
Did create 5000 files of 100 KB size (488) MB in 11,529s
Elapsed time by Return Code: 11528
Wait 10s before profiling is stopped to capture potential async operations which are still running
Stop Profiling
Press Ctrl+C to cancel the stop operation.
100%  [>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>]
The trace was successfully saved.

Testcase CreateTaskParallel
Wait 10s for system to settle down
Calling: FileWriter.exe -generate "C:\temp\Test1" taskparallel
Did create 5000 files of 100 KB size (488) MB in 11,355s
Elapsed time by Return Code: 11354
Wait 10s before profiling is stopped to capture potential async operations which are still running
Stop Profiling
Press Ctrl+C to cancel the stop operation.
100%  [>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>]
The trace was successfully saved.

15 - files found to extract.
Skipping file c:\temp\CreateSerial_12443msSKYRMION.etl 1/15 because extract already exists
Skipping file c:\temp\CreateTaskParallel_12166msSKYRMION.etl 2/15 because extract already exists
Skipping file c:\temp\CreateSerial_7732msSKYRMION.etl 5/15 because extract already exists
Skipping file c:\temp\CreateSerial_7066msSKYRMION.etl 4/15 because extract already exists
Skipping file c:\temp\CreateParallel_12506msSKYRMION.etl 3/15 because extract already exists
Skipping file c:\temp\CreateSerial_12012msSKYRMION.etl 6/15 because extract already exists
Skipping file c:\temp\CreateParallel_4721msSKYRMION.etl 7/15 because extract already exists
Skipping file c:\temp\CreateParallel_4538msSKYRMION.etl 8/15 because extract already exists
Skipping file c:\temp\CreateParallel_12019msSKYRMION.etl 9/15 because extract already exists
Skipping file c:\temp\CreateTaskParallel_4069msSKYRMION.etl 10/15 because extract already exists
Skipping file c:\temp\CreateTaskParallel_11150msSKYRMION.etl 11/15 because extract already exists
Skipping file c:\temp\CreateTaskParallel_6234msSKYRMION.etl 12/15 because extract already exists
Success Extraction of c:\temp\Extract\CreateSerial_12042msSKYRMION.json
Extracted 13/15 - Failed 0 files.
Success Extraction of c:\temp\Extract\CreateTaskParallel_11354msSKYRMION.json
Extracted 14/15 - Failed 0 files.
Success Extraction of c:\temp\Extract\CreateParallel_11528msSKYRMION.json
Extracted 15/15 - Failed 0 files.
Extracted: 15 files in 00 00:06:14, Failed Files 0
```
