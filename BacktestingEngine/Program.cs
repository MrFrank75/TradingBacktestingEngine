using BacktestingEngine.Core;
using BacktestingEngine.Strategies;

namespace BacktestingEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //read the data and place them in a vector
            var pricesReader = new PricesReader();

            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE","ETCUSDT","240").ToList();

            var tradesExecutionReport = new List<TradeExecutionResult>();
            var strategyToExecute = new TripleSupertrendStrategy();
            var dateFilter = new DateFilter(new DateTime(2019, 1, 1), new DateTime(2023, 1, 1));

            IStrategyExecutionEngine strategyEngine = new GenericTradingViewStrategyEngine(priceCandlesticks, strategyToExecute, dateFilter);

            foreach (var price in priceCandlesticks)
            {
                TradeExecutionResult singleTradeExecutionResult = strategyEngine.ExecuteStrategy(price);
                if (singleTradeExecutionResult.State == TradeState.Closed)
                {
                    tradesExecutionReport.Add(singleTradeExecutionResult);
                }
            }

            var strategySummaryReport = strategyEngine.GetSummaryReport();

            PrintExecutionResults(tradesExecutionReport);
            PrintSummaryReport(strategySummaryReport);
        }


        public static void PrintExecutionResults(List<TradeExecutionResult> results)
        {
            Console.WriteLine("{0,-5}{1,-10}{2,-10}{3,-15}{4,-25}{5,-25}{6,-14:N2}{7,-14:N2}{8,-10:N5}",
                              "#", "State", "Profit", "Current Capital", "Opening Date Time",
                              "Closing Date Time", "Opening Price", "Closing Price", "Contracts");
            int counter = 0;
            foreach (var result in results)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (result.Profit>0)
                    Console.ForegroundColor = ConsoleColor.Green;

                counter++;
                Console.WriteLine("{0,-5}{1,-10}{2,-10:N2}{3,-15:N2}{4,-25}{5,-25}{6,-14:N2}{7,-14:N2}{8,-10:N5}",
                                  counter,result.State, result.Profit, result.CurrentCapital,
                                  result.OpeningDateTime, result.ClosingDateTime, result.OpeningPrice, result.ClosingPrice,
                                  result.Contracts);
            }
        }


        public static void PrintSummaryReport(BacktestReport result)
        {
            int colWidth = 20; // Set the column width
            Console.ForegroundColor= ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("{0,-" + colWidth + "}{1,-" + colWidth + "}","Starting Capital", "Ending Capital");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0,-" + colWidth + "}{1,-" + colWidth + "}",result.StartingCapital, result.EndingCapital);
        }



    }
}