namespace BacktestingEngine.Core
{
    internal interface IStrategyExecutionEngine
    {
        TradeExecutionResult ExecuteStrategy(Candlestick price);
        BacktestReport GetSummaryReport();
    }
}