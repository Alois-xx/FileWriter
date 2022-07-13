using FileWriter;
using System.Diagnostics;

namespace FileWriter
{
    public class Program
    {
        static int myFileCount = 5000;

        enum GenerationMode
        {
            Serial,
            Parallel,
            TaskParallel,
        }

        static void Help()
        {
            string Help = "FileWriter [-generate dir] [serial,parallel [-threads n],taskparallel]  [-files dd] [-extension .cmd]" + Environment.NewLine +
                          "Examples" + Environment.NewLine +
                          $"Write {myFileCount} files to c:\\temp with 4 threads." + Environment.NewLine +
                          "  FileWriter -generate c:\\temp parallel -threads 4";
            Console.WriteLine(Help);
        }

        public static int Main(string[] args)
        {
            int sizeKB = 100;

            string? outputDirectory = null;
            int? nThreads = null;
            GenerationMode mode = GenerationMode.Serial;
            string extension = ".cmd"; // Make Antivirus nervous
            Queue<string> qArgs = new Queue<string>(args);

            try
            {
                while (qArgs.Count > 0)
                {
                    string current = qArgs.Dequeue();

                    switch (current.ToLowerInvariant())
                    {
                        case "-generate":
                            outputDirectory = qArgs.Dequeue();
                            if (qArgs.Count > 0)
                            {
                                if (qArgs.TryPeek(out string? strMode))
                                {
                                    if (Enum.TryParse<GenerationMode>(strMode, true, out mode))
                                    {
                                        string tmp = qArgs.Dequeue();
                                    }
                                }
                            }
                            break;
                        case "-threads":
                            nThreads = int.Parse(qArgs.Dequeue());
                            break;
                        case "-extension":
                            extension = qArgs.Dequeue();
                            break;
                        case "-files":
                            myFileCount = int.Parse(qArgs.Dequeue());
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
            switch (mode)
            {
                case GenerationMode.Parallel:
                    gen.CreateFilesParallel(myFileCount, sizeKB, nThreads);
                    break;
                case GenerationMode.TaskParallel:
                    gen.CreateFilesTask(myFileCount, sizeKB);
                    break;
                case GenerationMode.Serial:
                default:
                    gen.CreateFiles(myFileCount, sizeKB);
                    break;
            }
            sw.Stop();
            Console.WriteLine($"Did create {myFileCount} files of {sizeKB} KB size ({(sizeKB * myFileCount) / 1024:N0}) MB in {sw.Elapsed.TotalSeconds:F3}s");
            return (int)sw.Elapsed.TotalMilliseconds;
        }
    }

}