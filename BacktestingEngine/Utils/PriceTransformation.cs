using BacktestingEngine.Core;

namespace BacktestingEngine.Utils
{
    internal static class PriceTransformation
    {

        public static List<Candlestick> ToHHCandleSticks(List<Candlestick> regularPrices)
        {
            var hhSticks = new List<Candlestick>();
            foreach (var candlestick in regularPrices)
            {
                hhSticks.Add(GetCurrentHHCandleStick(hhSticks, candlestick));
            }
            return hhSticks;
        }

        private static Candlestick GetCurrentHHCandleStick(List<Candlestick> hhCandlesticks, Candlestick newCandlestick)
        {
            decimal haOpen = 0M;
            if (hhCandlesticks.Count == 0)
                haOpen = (newCandlestick.Open + newCandlestick.Close) / 2;
            else
                haOpen = (hhCandlesticks.Last().Open + hhCandlesticks.Last().Close) / 2;

            decimal haClose = (newCandlestick.Open + newCandlestick.High + newCandlestick.Low + newCandlestick.Close) / 4;
            decimal haHigh = Math.Max(Math.Max(newCandlestick.High, haOpen), haClose);
            decimal haLow = Math.Min(Math.Min(newCandlestick.Low, haOpen), haClose);

            return new Candlestick
            {
                UnixTime = newCandlestick.UnixTime,
                Open = haOpen,
                High = haHigh,
                Low = haLow,
                Close = haClose
            };
        }
    }
}