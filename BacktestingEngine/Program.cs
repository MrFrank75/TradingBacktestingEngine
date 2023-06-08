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
            List<TradeSetup> tradeSetups = GetTradeSetups();

            var tradesExecutionReports = new List<TradeExecutionResult>();
            var strategyBacktestReports = new List<BacktestReport>();
            var pendingTrade = new TradeExecutionResult();

            foreach (var tradeSetup in tradeSetups)
            {
                string ticker = tradeSetup.Configuration.Ticker;
                Console.WriteLine($"Running strategy on {tradeSetup.Candles.Count} prices for ticker: {ticker}");
                long counter = 0;
                int tick = tradeSetup.Candles.Count / 20;
                var dateFilter = new DateFilter(tradeSetup.Configuration.TradingStartDate, tradeSetup.Configuration.TradingEndDate);
                var strategyToExecute = new TripleSupertrendStrategy(dateFilter);
                IStrategyExecutionEngine strategyEngine = new GenericTradingViewStrategyEngine(strategyToExecute, dateFilter, tradeSetup.Configuration.Ticker);

                foreach (var price in tradeSetup.Candles)
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

            PrintSummaryReport(strategyBacktestReports);
            PrintExecutionResults(tradesExecutionReports);
        }

        private static List<TradeSetup> GetTradeSetups()
        {
            var pricesReader = new CsvReader<Candlestick>();
            var tradeConfigurations = CsvReader<TradeConfiguration>.ReadRecords("TradeSetupConfiguration\\SupertrendSetup.csv");
            var tradeSetups = new List<TradeSetup>();

            foreach (var configuration in tradeConfigurations)
            {

                var tickerPriceCandlesticks = pricesReader.ReadPricesVector("B", configuration.Ticker, configuration.Timeframe,"PrivateCsvDatabase").ToList();
                tradeSetups.Add(new TradeSetup
                {
                    Candles = tickerPriceCandlesticks,
                    Configuration = configuration,
                });    
            }
            return tradeSetups;
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
            int colWidth = 15; // Set the column width
            Console.ForegroundColor= ConsoleColor.Gray;
            Console.WriteLine();
            string columnSpacing = "{0,-" + colWidth + "}{1,-" + colWidth + "}{2,-" + colWidth + "}{3,-" + colWidth + "}{4,-" + colWidth + "}{5,-" + colWidth + "}{6,-" + colWidth + "}";
            Console.WriteLine(columnSpacing, "Ticker", "Start Equity", "Final Equity","Gain", "Gain%", "Max Drawdown","Max DD%");

            foreach (var report in backtestReports)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(columnSpacing,report.Ticker, report.StartingCapital, report.EndingCapital,report.Gain, report.GainPerc, report.MaxDrawdown, report.MaxDrawdownPercent);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
        }



    }
}