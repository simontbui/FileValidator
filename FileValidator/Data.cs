using System.Globalization;

namespace FileValidator
{
    public class Data
    {
        private readonly List<List<string>> _data = new List<List<string>>();
        private readonly List<string> headers = new List<string>();
        private bool lotExpIsValid;
        private bool hasPriceCol = false;

        public Data(string file, char delimiter)
        {
            foreach (string line in System.IO.File.ReadAllLines(file))
            {
                List<string> row = new List<string>();
                foreach (string col in line.Split(delimiter))
                {
                    row.Add(col);
                }
                _data.Add(row);
            }

            headers = _data[0];
        }

        private int GetItemExpColNum()
        {
            int colNum = 0;
            foreach (string col in headers)
            {
                if (col.ToLower().Contains("exp") && !col.ToLower().Contains("lot"))
                {
                    colNum = headers.IndexOf(col);
                }
            }

            return colNum;
        }

        private int GetLotExpColNum()
        {
            int colNum = 0;
            foreach (string col in headers)
            {
                if (col.ToLower().Contains("lot") && col.ToLower().Contains("exp"))
                {
                    colNum = headers.IndexOf(col);
                }
            }

            return colNum;
        }

        private int GetLotColNum()
        {
            int colNum = 0;
            foreach (string col in headers)
            {
                if (col.ToLower().Contains("lot") && !col.ToLower().Contains("exp"))
                {
                    colNum = headers.IndexOf(col);
                }
            }

            return colNum;
        }

        private List<Nullable<DateTime>> GetLotExpDates()
        {
            List<Nullable<DateTime>> lotExpDates = new List<Nullable<DateTime>>();
            int colNum = GetLotExpColNum();

            foreach (var line in _data.Skip(1)) // skipping first row of data (headers)
            {
                if (line[colNum] != "")
                {
                    lotExpDates.Add(DateTime.ParseExact(line[colNum], "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    lotExpDates.Add(null);
                }
             
            }
            return lotExpDates;
        }

        private List<Nullable<DateTime>> GetItemExpDates()
        {
            List<Nullable<DateTime>> itemExpDates = new List<Nullable<DateTime>>();
            int colNum = GetItemExpColNum();

            foreach (var line in _data.Skip(1)) // skipping first row of data (headers)
            {
                if (line[colNum] != "")
                {
                    itemExpDates.Add(DateTime.ParseExact(line[colNum], "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {
                    itemExpDates.Add(null);
                }

            }
            return itemExpDates;
        }

        public void Display()
        {
            foreach (var row in _data) 
            { 
                foreach (var col in row)
                {
                    Console.WriteLine(col);
                }
            }
        }

        public void DisplayHeaders()
        {
            Console.Write("Column Names: ");
            Console.WriteLine(String.Join(", ", headers));
            Console.WriteLine("=======================================");
        }

        public void ValidateLotExpDates()
        {
            List<Nullable<DateTime>> itemExpDates = new List<Nullable<DateTime>>();
            List<Nullable<DateTime>> lotExpDates = new List<Nullable<DateTime>>();

            try
            {
                itemExpDates = GetItemExpDates();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.ResetColor();
                Console.WriteLine("Invalid date value in Item Expiration column");
            }

            try
            {
                lotExpDates = GetLotExpDates();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.ResetColor();
                Console.WriteLine("Invalid date value in Lot Expiration column");
            }

            int numDates = itemExpDates.Count;
            bool isValid = false;

            for (int i=0; i<numDates; i++)
            {
                if (itemExpDates[i].HasValue && lotExpDates[i].HasValue)
                {
                    int dateDiff = (lotExpDates[i].Value - itemExpDates[i].Value).Days;
                    if (dateDiff != 365) { isValid = true; }
                }
            }

            if (isValid)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Lot expiration dates are valid.");
                Console.ResetColor();
                lotExpIsValid = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Lot expiration dates are not valid.");
                Console.ResetColor();
                lotExpIsValid = false;
            }
        }

        public void ValidatePriceColumn()
        {
            foreach (string header in headers)
            {
                if (header.ToLower().Contains("price"))
                {
                    hasPriceCol = true;
                }
            }

            if (hasPriceCol)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("File contains a price column.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File is missing a price column.");
                Console.ResetColor();
            }
        }

        public void ValidateFile()
        {
            if (lotExpIsValid && hasPriceCol)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAcceptable File.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid File.");
            }
        }
    }
}
