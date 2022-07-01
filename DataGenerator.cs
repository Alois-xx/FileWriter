namespace FileWriter
{
    internal class DataGenerator
    {
        string myDirectory;
        const int KB = 1024;

        /// <summary>
        /// Make Virus scanner nervous
        /// </summary>
        const string FileExtension = ".cmd";

        public DataGenerator(string dir)
        {
            if( !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                // delete old test data
                foreach(var file in Directory.GetFiles(dir, "*" + FileExtension))
                {
                    File.Delete(file);
                }
            }

            
            myDirectory = dir;
        }

        public void CreateFiles(int n, int sizeKB)
        {
            for(int i = 0; i < n; i++)
            {
                CreateFile(i, sizeKB);
            }
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

        public void CreateFilesTask(int n, int sizekB)
        {
            List<Task> tasks = new List<Task>();
            for(int i= 0; i < n; i++)
            {
                int captured = i;
                tasks.Add( Task.Run( () => CreateFile(captured, sizekB)));
            }
            Task.WaitAll(tasks.ToArray());
        }


        private void CreateFile(int i, int sizeKB)
        {
            int lineCount = 1;
            int writtenBytes = 0;
            string fileName = Path.Combine(myDirectory, $"TestDataFile_{i}{FileExtension}");
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
    }
}
