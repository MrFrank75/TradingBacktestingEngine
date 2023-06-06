using BacktestingEngine.Core;

namespace BacktestingEngine
{
    public class TradedTicker
    {
        public string Ticker { get; set; }
        public List<Candlestick> Candles { get; set; }
    }
}