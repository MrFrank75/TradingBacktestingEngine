using BacktestingEngine.Core;

namespace BacktestingEngine.Strategies
{
    internal class TripleSupertrendStrategy : IStrategy
    {
        SupertrendStrategy stStrategy1 = new SupertrendStrategy(3, 10, 2,20);
        public TripleSupertrendStrategy()
        {
        }

        public bool EvaluateCloseLongCondition(Candlestick price)
        {
            return false;
        }

        public Core.TradingSignal GenerateSignal(Candlestick price)
        {
            return stStrategy1.GenerateSignal(price).Item1;
        }

        public bool EvaluateOpenLongCondition(Candlestick price)
        {
            return false;
        }


    }

}