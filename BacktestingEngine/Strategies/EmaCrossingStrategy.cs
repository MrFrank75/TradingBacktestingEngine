using BacktestingEngine.Core;

namespace BacktestingEngine.Strategies
{
    internal class EmaCrossingStrategy : IStrategy
    {
        private readonly List<Candlestick> _priceCandlesticks;

        public EmaCrossingStrategy(List<Candlestick> priceCandlesticks)
        {
            _priceCandlesticks = priceCandlesticks;
        }

        public bool EvaluateCloseLongCondition(Candlestick price)
        {
            double[] closePrices = StrategyHelper.GetElementsUntilAndIncluding(_priceCandlesticks, price).Select(x => Convert.ToDouble(x.Close)).ToArray();
            double[] sma14 = new double[closePrices.Length];
            double[] sma28 = new double[closePrices.Length];

            if (closePrices.Length <= 0)
            {
                return false;
            }

            TicTacTec.TA.Library.Core.Sma(0, closePrices.Length - 1, closePrices, 14, out int outSma14Idx, out int outNbElementSmaFast, sma14);
            TicTacTec.TA.Library.Core.Sma(0, closePrices.Length - 1, closePrices, 28, out int outSmaSlowIdx, out int outNbElementSmaSlow, sma28);

            return outNbElementSmaFast > 1 && outNbElementSmaSlow > 1 ? sma14[outNbElementSmaFast - 1] < sma28[outNbElementSmaSlow - 1] && sma14[outNbElementSmaFast - 2] >= sma28[outNbElementSmaSlow - 2] : false;
        }

        public bool EvaluateOpenLongCondition(Candlestick price)
        {
            double[] closePrices = StrategyHelper.GetElementsUntilAndIncluding(_priceCandlesticks, price).Select(x => Convert.ToDouble(x.Close)).ToArray();
            double[] sma14 = new double[closePrices.Length];
            double[] sma28 = new double[closePrices.Length];

            if (closePrices.Length <= 0)
            {
                return false;
            }

            TicTacTec.TA.Library.Core.Sma(0, closePrices.Length - 1, closePrices, 14, out int outSma14Idx, out int outNbElementSmaFast, sma14);
            TicTacTec.TA.Library.Core.Sma(0, closePrices.Length - 1, closePrices, 28, out int outSmaSlowIdx, out int outNbElementSmaSlow, sma28);

            return outNbElementSmaFast > 1 && outNbElementSmaSlow > 1 ? sma14[outNbElementSmaFast - 1] > sma28[outNbElementSmaSlow - 1] && sma14[outNbElementSmaFast - 2] <= sma28[outNbElementSmaSlow - 2] : false;
        }

        public TradingSignal GenerateSignal(Candlestick price)
        {
            throw new NotImplementedException();
        }
    }
}