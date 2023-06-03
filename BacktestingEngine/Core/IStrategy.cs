using BacktestingEngine.Core;

namespace BacktestingEngine.Core
{
    internal interface IStrategy
    {
        TradingSignal GenerateSignal(Candlestick price);
    }
}