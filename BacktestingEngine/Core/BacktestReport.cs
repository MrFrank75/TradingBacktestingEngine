namespace BacktestingEngine.Core
{
    public class BacktestReport
    {
        public string Ticker { get; set; }
        public decimal StartingCapital { get; set; }
        public decimal EndingCapital { get; set; }
        public decimal Gain{ get; set; }
        public decimal GainPerc { get; set; }
        public decimal NetProfit { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossLoss { get; set; }
        public decimal ProfitFactor { get; set; }
        public decimal ReturnOnInvestment { get; set; }
        public decimal AbsoluteDrawdown { get; set; }
        public decimal MaxDrawdown { get; set; }
        public decimal MaxDrawdownPercent { get; set; }
        public decimal TotalTrades { get; set; }
        public decimal WinningTrades { get; set; }
        public decimal LosingTrades { get; set; }
        public decimal WinRate { get; set; }
        public decimal AverageWin { get; set; }
        public decimal AverageLoss { get; set; }
        public decimal AverageTrade { get; set; }
        public decimal LargestWin { get; set; }
        public decimal LargestLoss { get; set; }
        public decimal Expectancy { get; set; }
        public decimal SharpeRatio { get; set; }
        public decimal SortinoRatio { get; set; }
        public decimal CalmarRatio { get; set; }
        public decimal ZScore { get; set; }
        public decimal InformationRatio { get; set; }
        public decimal UlcerIndex { get; set; }
        public decimal CAGR { get; set; }
        public decimal AnnualizedVolatility { get; set; }
        public decimal AnnualizedSharpeRatio { get; set; }
        public decimal TotalFees { get; set; }
        public decimal AverageTradeFee { get; set; }
        public decimal MaxConsecutiveWins { get; set; }
        public decimal MaxConsecutiveLosses { get; set; }
    }

}