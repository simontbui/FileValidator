namespace FileValidator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string file = @"C:\Users\Simon\source\repos\FileValidator\test.csv";
            char delimiter = ',';

            Data contents = new Data(file, delimiter);
            contents.DisplayHeaders();
            contents.ValidateLotExpDates();
            contents.ValidatePriceColumn();
            contents.ValidateFile();

            Console.ReadKey();
        }
    }
}