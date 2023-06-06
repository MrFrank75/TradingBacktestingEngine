using BacktestingEngine.Core;

namespace BacktestingEngine
{
    public class TradeSetup
    {
        public TradeConfiguration Configuration { get; set; }
        
        public List<Candlestick> Candles { get; set; }
    }

    public class TradeConfiguration
    {
        public string Ticker { get; set; }

        public DateTime TradingStartDate { get; set; }
        public DateTime TradingEndDate { get; set; }
        public string Timeframe { get; set; }
    }
}