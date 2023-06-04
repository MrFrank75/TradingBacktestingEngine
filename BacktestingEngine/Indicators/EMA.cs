using BacktestingEngine.Core;

namespace BacktestingEngine.Strategies
{
    public static class EMA
    {

        public static decimal Calculate(List<Candlestick> candlesticks, int period)
        {
            var closePrices = candlesticks.Select(c => (double)c.Close).ToArray();
            int outBegIdx, outNbElement;
            double[] emaValues = new double[closePrices.Length];
            TicTacTec.TA.Library.Core.RetCode ema100Result = TicTacTec.TA.Library.Core.Ema(0, candlesticks.Count - 1, closePrices, period, out outBegIdx, out outNbElement, emaValues);
            if (ema100Result == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                return (decimal)emaValues.First();
            }

            return 0;
        }
    }
}