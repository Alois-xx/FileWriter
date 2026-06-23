using FileWriter;
using System.Diagnostics;

namespace FileWriter
{
    public class Program
    {
        static int myFileCount = 5000;

        static void Help()
        {
            string Help = "FileWriter [-write/compile/execute] [-folder dir]  [-files dd] [-sizeKB n] [-threads n or all or all-1] [-extension .cmd] " + Environment.NewLine +
                          "  -write    Generate text files with given extension." + Environment.NewLine + 
                          "  -compile  Simulate compilation by writing exe files instead of text files." + Environment.NewLine +
                          "  -execute  Execute the generated files." + Environment.NewLine +
                          "Examples" + Environment.NewLine +
                          $"Write {myFileCount} files to c:\\temp\\Write with 1 thread." + Environment.NewLine +
                          "  FileWriter -write -folder c:\\temp\\Write -threads 1" + Environment.NewLine +
                         $"Simulate compile by writing executables to c:\\temp\\Compile." + Environment.NewLine +
                          "  FileWriter -compile -folder c:\\temp\\compile -threads 1" + Environment.NewLine +
                        $"Simulate execution of compiled executables by running all executables in folder c:\\temp\\Compile." + Environment.NewLine +
                         "  FileWriter -execute -folder c:\\temp\\compile -threads 1";


            Console.WriteLine(Help);
        }

        enum Mode
        {
            Generate,
            Compile,
            Execute,
        }


        public static int Main(string[] args)
        {
            int sizeKB = 200;

            string? outputDirectory = null;
            int? nThreads = null;
            string extension = ".cmd"; // Make Antivirus nervous
            Queue<string> qArgs = new Queue<string>(args);

            Mode mode = Mode.Generate;

            try
            {
                while (qArgs.Count > 0)
                {
                    string current = qArgs.Dequeue();

                    switch (current.ToLowerInvariant())
                    {
                        case "-folder":
                            outputDirectory = qArgs.Dequeue();
                            Directory.CreateDirectory(outputDirectory);
                            break;
                        case "-threads":
                            string threadCount = qArgs.Dequeue();
                            if (threadCount.ToLowerInvariant() == "all")
                            {
                                nThreads = Environment.ProcessorCount;
                            }
                            else if( threadCount.ToLowerInvariant() == "all-1")
                            {
                                nThreads = Environment.ProcessorCount - 1;
                            }
                            else
                            {
                                nThreads = int.Parse(threadCount);
                            }
                            break;
                        case "-extension":
                            extension = qArgs.Dequeue();
                            break;
                        case "-files":
                            myFileCount = int.Parse(qArgs.Dequeue());
                            break;
                        case "-sizekb":
                            sizeKB = int.Parse(qArgs.Dequeue());
                            break;
                        case "-compile":
                            mode = Mode.Compile;
                            break;
                        case "-execute":
                            mode = Mode.Execute;
                            break;
                        case "-write":
                            mode = Mode.Generate;
                            break;
                        default:
                            Help();
                            Console.WriteLine($"Error: Argument {current} is not valid");
                            return 0;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                Help();
                Console.WriteLine("Error: Required command argument missing.");
                return 0;
            }

            if( outputDirectory == null)
            {
                Help();
                return 0;
            }

            DataGenerator gen = new DataGenerator(outputDirectory, extension);
            var sw = Stopwatch.StartNew();
            switch(mode)
            {
                case Mode.Compile:
                    gen.SimulateCompile(myFileCount, sizeKB, nThreads ?? 1);
                    break;
                case Mode.Execute:
                    myFileCount = gen.SimulateExecute(nThreads ?? 1);   
                    break;
                default:
                    gen.CreateFilesParallel(myFileCount, sizeKB, nThreads);
                    break;
            }
            sw.Stop();
            Console.WriteLine($"Did {mode} {myFileCount} files of {sizeKB} KB size ({(sizeKB * myFileCount) / 1024:N0}) MB in {sw.Elapsed.TotalSeconds:F3}s");
            return (int)sw.Elapsed.TotalMilliseconds;
        }
    }

}