using BacktestingEngine.Core;

namespace BacktestingEngine
{
    public class TradeSetup
    {
        public TradeConfiguration Configuration { get; set; } = new TradeConfiguration();
        
        public List<Candlestick> Candles { get; set; } = new List<Candlestick>();
    }
}