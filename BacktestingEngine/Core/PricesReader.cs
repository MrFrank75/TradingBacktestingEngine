using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace BacktestingEngine.Core
{
    public class PricesReader
    {

        public IEnumerable<Candlestick> ReadPricesVector(string broker, string security, string timeframe)
        {
            string fileName = $"CSVDatabase\\{broker} {security}, {timeframe}.csv";

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Candlestick>();
                return records.ToArray();
            }
        }
    }
}