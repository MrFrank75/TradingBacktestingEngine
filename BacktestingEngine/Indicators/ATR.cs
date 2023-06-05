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

        public decimal CalculateTrueRange(Candlestick current, Candlestick? previous)
        {
            decimal highMinusLow = current.High - current.Low;

            if (previous == null)
            {
                return highMinusLow;
            }

            decimal highMinusPreviousClose = Math.Abs(current.High - previous.Close);
            decimal lowMinusPreviousClose = Math.Abs(current.Low - previous.Close);

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
                int idxCurrentCandle = candles.IndexOf(candle);
                Candlestick? previous = idxCurrentCandle >= 1 ? candles[candles.IndexOf(candle) - 1] : null;
                var tr = CalculateTrueRange(candle, previous);
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
