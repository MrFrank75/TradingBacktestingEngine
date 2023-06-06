namespace BacktestingEngine.Core
{
    internal interface IStrategyExecutionEngine
    {
        TradeExecutionResult ExecuteStrategy(string ticker, Candlestick price);
        BacktestReport GetSummaryReport();
    }
}