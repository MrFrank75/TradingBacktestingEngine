using BacktestingEngine.Core;

namespace BacktestingEngine.Indicators
{
    public class ATR
    {
        private readonly int _period;

        public ATR(int period)
        {
            _period = period;
        }

        public decimal CalculateTrueRange(List<Candlestick> candles)
        {
            Candlestick lastCandle= candles.Last();
            decimal highMinusLow = lastCandle.High - lastCandle.Low;

            if (candles.Count<2)
                return highMinusLow;

            decimal highMinusPreviousClose = Math.Abs(lastCandle.High - candles[candles.Count - 1].Close);
            decimal lowMinusPreviousClose = Math.Abs(lastCandle.Low - candles[candles.Count - 1].Close);

            decimal trueRange = Math.Max(highMinusLow, Math.Max(highMinusPreviousClose, lowMinusPreviousClose));
            return trueRange;
        }

        public decimal CalculateAverageTrueRange(List<Candlestick> candles, SmoothingType smoothingType = SmoothingType.SMA)
        {
            var trueRangeValues = new List<decimal>();
            if (candles.Count < _period)
                return 0;

            foreach (var candle in candles)
            {
                var listOfCandles = StrategyHelper.GetElementsUntilAndIncluding<Candlestick>(candles, candle);
                var tr = CalculateTrueRange(listOfCandles);
                trueRangeValues.Add(tr);
            }

            decimal atr = Smooth(trueRangeValues, smoothingType);
            return atr;
        }

        private decimal Smooth(List<decimal> values, SmoothingType smoothingType) {
            switch (smoothingType)
            {
                case SmoothingType.SMA:
                    return SMA.Calculate(values.TakeLast(_period).ToList());
                case SmoothingType.RMA:
                    return new RMA(_period).Calculate(values);
                default:
                    throw new ArgumentException();
            }
        }

    }
}
