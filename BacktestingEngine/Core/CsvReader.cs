using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace BacktestingEngine.Core
{
    public class CsvReader<T>
    {

        public IEnumerable<T> ReadPricesVector(string broker, string security, string timeframe, string databasePath="CSVDatabase")
        {
            string fileName = $"{broker} {security}, {timeframe}.csv";
            var path = Path.Combine(databasePath, fileName);

            return ReadRecords(fileName);
        }

        public static IEnumerable<T> ReadRecords(string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<T>();
                return records.ToArray();
            }
        }
    }
}