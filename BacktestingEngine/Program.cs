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

            var tradesExecutionReports = new List<TradeExecutionResult>();
            var strategyBacktestReports = new List<BacktestReport>();
            var pendingTrade = new TradeExecutionResult();
            var dateFilter = new DateFilter(new DateTime(2022, 1, 1), new DateTime(2023, 1, 1));
            var strategyToExecute = new TripleSupertrendStrategy(dateFilter);

            foreach (var tradedTicker in tickersToTrade)
            {
                Console.WriteLine($"Running strategy on {tradedTicker.Candles.Count} prices for ticker: {tradedTicker.Ticker}");
                long counter = 0;
                int tick = tradedTicker.Candles.Count / 20;
                IStrategyExecutionEngine strategyEngine = new GenericTradingViewStrategyEngine(strategyToExecute, dateFilter, tradedTicker.Ticker);

                foreach (var price in tradedTicker.Candles)
                {
                    TradeExecutionResult singleTradeExecutionResult = strategyEngine.ExecuteStrategy(price);

                    if (singleTradeExecutionResult.State == TradeState.Opened)
                    {
                        pendingTrade = singleTradeExecutionResult;
                    }

                    if (singleTradeExecutionResult.State == TradeState.Closed)
                    {
                        tradesExecutionReports.Add(singleTradeExecutionResult);
                    }
                    counter++;
                    if (counter % tick == 0)
                        Console.Write("=");
                }
                Console.WriteLine("| DONE");

                tradesExecutionReports.Add(pendingTrade);
                strategyBacktestReports.Add(strategyEngine.GetSummaryReport());
            }


            PrintExecutionResults(tradesExecutionReports);
            PrintSummaryReport(strategyBacktestReports);
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
                              "#", "Ticker", "Gain", "Gain%", "Equity", "Opening Date Time",
                              "Closing Date Time", "Opening Price", "Closing Price", "Contracts");
            int counter = 0;
            foreach (var result in results)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (result.Gain>0)
                    Console.ForegroundColor = ConsoleColor.Green;

                counter++;
                Console.WriteLine("{0,-5}{1,-10}{2,-10:N2}{3,-10:N2}{4,-15:N2}{5,-25}{6,-25}{7,-14:N2}{8,-14:N2}{9,-10:N5}",
                                  counter,result.Ticker, result.Gain, result.GainPerc, result.CurrentCapital,
                                  result.OpeningDateTime, result.ClosingDateTime, result.OpeningPrice, result.ClosingPrice,
                                  result.Contracts);
            }
        }


        public static void PrintSummaryReport(List<BacktestReport> backtestReports)
        {
            int colWidth = 20; // Set the column width
            Console.ForegroundColor= ConsoleColor.Gray;
            Console.WriteLine();
            string columnSpacing = "{0,-" + colWidth + "}{1,-" + colWidth + "}{2,-" + colWidth + "}{3,-" + colWidth + "}{4,-" + colWidth + "}{5,-" + colWidth + "}{6,-" + colWidth + "}";
            Console.WriteLine(columnSpacing, "Ticker", "Start Equity", "Final Equity","Gain", "Gain%", "Max Drawdown","Max DD%");

            foreach (var report in backtestReports)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(columnSpacing,report.Ticker, report.StartingCapital, report.EndingCapital,report.Gain, report.GainPerc, report.MaxDrawdown, report.MaxDrawdownPercent);

            }
        }



    }
}