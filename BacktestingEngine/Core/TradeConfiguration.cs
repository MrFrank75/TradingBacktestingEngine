namespace BacktestingEngine
{
    public class TradeConfiguration
    {
        public string Ticker { get; set; }

        public DateTime TradingStartDate { get; set; }
        public DateTime TradingEndDate { get; set; }
        public string Timeframe { get; set; }
    }
}