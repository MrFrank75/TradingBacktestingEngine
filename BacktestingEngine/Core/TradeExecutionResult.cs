namespace BacktestingEngine.Core
{
    public class TradeExecutionResult
    {
        public string Ticker { get; set; }
        public TradeState State { get; set; }
        public decimal Gain { get; internal set; }
        public decimal GainPerc { get; internal set; }
        public decimal CurrentCapital { get; internal set; }
        public DateTime OpeningDateTime { get; internal set; }
        public DateTime ClosingDateTime { get; internal set; }
        public decimal ClosingPrice { get; internal set; }
        public decimal Contracts { get; internal set; }
        public decimal OpeningPrice { get; internal set; }
    }
}
