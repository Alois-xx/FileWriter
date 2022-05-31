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
            string Help = "FileWriter [-generate dir] [serial,parallel,taskparallel]" + Environment.NewLine;
            Console.WriteLine(Help);
        }

        public static int Main(string[] args)
        {
            int n = 5000;
            int sizeKB = 100;

            if (args.Length > 1)
            {
                if (args[0].ToLowerInvariant() == "-generate")
                {
                    if (args.Length > 2)
                    {
                        DataGenerator gen = new DataGenerator(args[1]);
                        GenerationMode mode = (GenerationMode)Enum.Parse(typeof(GenerationMode), args[2], true);

                        var sw = Stopwatch.StartNew();
                        switch (mode)
                        {
                            case GenerationMode.Parallel:
                                gen.CreateFilesParallel(n, sizeKB);
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
                    else
                    {
                        Help();
                    }
                }
                else
                {
                    Help();
                }
            }
            else
            {
                Help();
            }

            return 0;
        }
    }

}