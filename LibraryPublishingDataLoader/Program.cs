using LibraryPublishingDataLoader.Loader;
using LumenWorks.Framework.IO.Csv;

namespace LibraryPublishingDataLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    throw new Exception("Must provide input directory to parse and output directory to write");
                }

                string inputDir = args[0];
                string outputDir = args[1];

                InstitutionDataFileLoader loader = new InstitutionDataFileLoader
                {
                    InputDir = inputDir,
                    OutputDir = outputDir
                };

                loader.LoadCsvReaders();
                loader.ParseCsvFiles();
                loader.SaveOutputFiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encountered exception while processing: {ex}");
                Console.WriteLine();
            }
            finally
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}