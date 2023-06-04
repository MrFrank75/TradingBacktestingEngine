using BacktestingEngine.Core;

namespace BacktestingEngine.Indicators
{
    public class SupertrendResult
    {
        public SupertrendResult()
        {
        }

        public Trend CurrentTrend { get; set; }
        public bool TrendHasChanged { get; set; }
        public decimal Value { get; set; }
    }
}