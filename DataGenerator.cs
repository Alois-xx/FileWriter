using System.Diagnostics;

namespace FileWriter
{
    internal class DataGenerator
    {
        string myDirectory;
        const int KB = 1024;

        /// <summary>
        /// Make Virus scanner nervous
        /// </summary>
        string myFileExtension = ".cmd";

        byte[]? myExeTemplateBuffer;
        readonly object myTemplateLock = new object();

        public DataGenerator(string dir, string extension)
        {
            myFileExtension = extension;
            if( !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                // delete old test data
                foreach(var file in Directory.GetFiles(dir, "*" + myFileExtension))
                {
                    File.Delete(file);
                }
            }

            
            myDirectory = dir;
        }

        public void SimulateCompile(int n, int sizeKB, int? nThreads)
        {
            ParallelOptions options = new ParallelOptions();
            if (nThreads != null)
            {
                options.MaxDegreeOfParallelism = nThreads.Value;
            }
            Parallel.For(0, n, options, i => CreateExe(i, sizeKB));
        }

        private byte[] GetExeTemplate()
        {
            if (myExeTemplateBuffer != null)
            {
                return myExeTemplateBuffer;
            }

            lock (myTemplateLock)
            {
                if (myExeTemplateBuffer != null)
                {
                    return myExeTemplateBuffer;
                }

                string exePath = Environment.ProcessPath!;
                myExeTemplateBuffer = File.ReadAllBytes(exePath);
                return myExeTemplateBuffer;
            }
        }

        private void CreateExe(int i, int sizeKB)
        {
            byte[] template = GetExeTemplate();
            byte[] buffer = new byte[sizeKB*1024];
            Buffer.BlockCopy(template, 0, buffer, 0, template.Length);
            // fill byte buffer with random data to make it unique so AV has some work to do.
            Random.Shared.NextBytes(new Span<byte>(buffer, template.Length, buffer.Length-template.Length));
            string fileName = Path.Combine(myDirectory, $"FileWriterCompile_{i:D3}.exe");
            File.WriteAllBytes(fileName, buffer);
        }

        public void CreateFilesParallel(int n, int sizeKB, int? nThreads)
        {
            ParallelOptions options = new ParallelOptions();
            if( nThreads != null)
            {
                options.MaxDegreeOfParallelism = nThreads.Value;
            }

            Parallel.For(0, n, options, i => CreateFile(i, sizeKB));
        }

        private void CreateFile(int i, int sizeKB)
        {
            int lineCount = 1;
            int writtenBytes = 0;
            string fileName = Path.Combine(myDirectory, $"TestDataFile_{i}{myFileExtension}");
            using(FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using(StreamWriter sw = new StreamWriter(fs))
                {
                    while (writtenBytes < sizeKB * KB)
                    {
                        string line = $"echo This is line {lineCount++}";
                        sw.WriteLine(line);
                        writtenBytes += line.Length;
                    }
                }
            }
        }

        /// <summary>
        /// Run previously generated exe files in parallel. Returns the number of executed files.
        /// </summary>
        /// <param name="nThreads"></param>
        /// <returns></returns>
        internal int SimulateExecute(int? nThreads)
        {
            string[] exes = Directory.GetFiles(myDirectory, "*.exe");

            ParallelOptions options = new ParallelOptions();
            if (nThreads != null)
            {
                options.MaxDegreeOfParallelism = nThreads.Value;
            }

            Parallel.ForEach(exes, options, exe =>
            {
                ProcessStartInfo psi = new ProcessStartInfo(exe);
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                using (Process ?p = Process.Start(psi))
                {
                    p?.WaitForExit();
                }
            });

            return exes.Length;
        }
    }
}
