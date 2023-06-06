using BacktestingEngine.Core;
using BacktestingEngine.Strategies;
using CsvHelper;

namespace BacktestingEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //read the data and place them in a vector
            List<TradedTicker> tickersToTrade = new List<TradedTicker>
            {
                GetTradedTicker("ETCUSDT", "240"),
                GetTradedTicker("ADAUSDT", "240"),
                GetTradedTicker("ATOMUSDT", "240"),
            };

            var tradesExecutionReport = new List<TradeExecutionResult>();
            var pendingTrade = new TradeExecutionResult();
            var dateFilter = new DateFilter(new DateTime(2022, 1, 1), new DateTime(2023, 1, 1));
            var strategyToExecute = new TripleSupertrendStrategy(dateFilter);
            IStrategyExecutionEngine strategyEngine = new GenericTradingViewStrategyEngine(strategyToExecute, dateFilter);

            foreach (var item in tickersToTrade)
            {
                Console.WriteLine($"Running strategy on {item.Candles.Count} prices for ticker: {item.Ticker}");
                long counter = 0;
                int tick = item.Candles.Count / 20;

                foreach (var price in item.Candles)
                {
                    TradeExecutionResult singleTradeExecutionResult = strategyEngine.ExecuteStrategy(item.Ticker, price);

                    if (singleTradeExecutionResult.State == TradeState.Opened)
                    {
                        pendingTrade = singleTradeExecutionResult;
                    }

                    if (singleTradeExecutionResult.State == TradeState.Closed)
                    {
                        tradesExecutionReport.Add(singleTradeExecutionResult);
                    }
                    counter++;
                    if (counter % tick == 0)
                        Console.Write("=");
                }
                Console.WriteLine("| DONE");

                tradesExecutionReport.Add(pendingTrade);
            }

            var strategySummaryReport = strategyEngine.GetSummaryReport();

            PrintExecutionResults(tradesExecutionReport);
            PrintSummaryReport(strategySummaryReport);
        }

        private static TradedTicker GetTradedTicker(string tickerName, string timeFrame)
        {
            var pricesReader = new PricesReader();
            var tickerPriceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", tickerName, timeFrame).ToList();
            return new TradedTicker
            {
                Candles = tickerPriceCandlesticks,
                Ticker = tickerName,
            };
        }

        public static void PrintExecutionResults(List<TradeExecutionResult> results)
        {
            Console.WriteLine("{0,-5}{1,-10}{2,-10}{3,-10}{4,-15}{5,-25}{6,-25}{7,-14:N2}{8,-14:N2}{9,-10:N5}",
                              "#", "State", "Gain", "Gain%", "Equity", "Opening Date Time",
                              "Closing Date Time", "Opening Price", "Closing Price", "Contracts");
            int counter = 0;
            foreach (var result in results)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (result.Gain>0)
                    Console.ForegroundColor = ConsoleColor.Green;

                counter++;
                Console.WriteLine("{0,-5}{1,-10}{2,-10:N2}{3,-10:N2}{4,-15:N2}{5,-25}{6,-25}{7,-14:N2}{8,-14:N2}{9,-10:N5}",
                                  counter,result.State, result.Gain, result.GainPerc, result.CurrentCapital,
                                  result.OpeningDateTime, result.ClosingDateTime, result.OpeningPrice, result.ClosingPrice,
                                  result.Contracts);
            }
        }


        public static void PrintSummaryReport(BacktestReport result)
        {
            int colWidth = 20; // Set the column width
            Console.ForegroundColor= ConsoleColor.Gray;
            Console.WriteLine();
            string columnSpacing = "{0,-" + colWidth + "}{1,-" + colWidth + "}{2,-" + colWidth + "}{3,-" + colWidth + "}";
            Console.WriteLine(columnSpacing, "Starting cap", "Ending Cap", "Max Drawdown","Max DD%");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(columnSpacing, result.StartingCapital, result.EndingCapital, result.MaxDrawdown, result.MaxDrawdownPercent);
        }



    }
}