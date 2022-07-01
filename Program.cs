using FileWriter;
using System.Diagnostics;

namespace FileWriter
{
    public class Program
    {
        enum GenerationMode
        {
            Serial,
            Parallel,
            TaskParallel,
        }

        static void Help()
        {
            string Help = "FileWriter [-generate dir] [serial,parallel [-threads n],taskparallel]  [-files dd]" + Environment.NewLine +
                          "Examples" + Environment.NewLine +
                          "Write files to c:\\temp with 4 threads" + Environment.NewLine +
                          "  FileWriter -generate c:\\temp parallel -threads 4";
            Console.WriteLine(Help);
        }

        public static int Main(string[] args)
        {
            int n = 5000;
            int sizeKB = 100;

            string? outputDirectory = null;
            int? nThreads = null;
            GenerationMode mode = GenerationMode.Serial;

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
                        case "-files":
                            n = int.Parse(qArgs.Dequeue());
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

            DataGenerator gen = new DataGenerator(outputDirectory);
            var sw = Stopwatch.StartNew();
            switch (mode)
            {
                case GenerationMode.Parallel:
                    gen.CreateFilesParallel(n, sizeKB, nThreads);
                    break;
                case GenerationMode.TaskParallel:
                    gen.CreateFilesTask(n, sizeKB);
                    break;
                case GenerationMode.Serial:
                default:
                    gen.CreateFiles(n, sizeKB);
                    break;
            }
            sw.Stop();
            Console.WriteLine($"Did create {n} files of {sizeKB} KB size ({(sizeKB * n) / 1024:N0}) MB in {sw.Elapsed.TotalSeconds:F3}s");
            return (int)sw.Elapsed.TotalMilliseconds;
        }
    }

}