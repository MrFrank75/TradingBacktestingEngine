using BacktestingEngine.Core;

namespace BacktestingEngine.Indicators
{
    public class ATR
    {
        private readonly int _period;
        private readonly RMA _rmaSmoother;

        public ATR(int period)
        {
            _period = period;
            _rmaSmoother = new RMA(period);
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
                return Decimal.MinValue;

            var trCandles = candles.Skip(candles.Count-_period);

            foreach (var candle in trCandles)
            {
                var listOfCandles = StrategyHelper.GetElementsUntilAndIncluding<Candlestick>(candles, candle);
                var tr = CalculateTrueRange(listOfCandles);
                // Add the True Range value to the list
                trueRangeValues.Add(tr);
            }

            decimal atr = Smooth(trueRangeValues, smoothingType);
            return atr;
        }

        private decimal Smooth(List<decimal> values, SmoothingType smoothingType) {
            switch (smoothingType)
            {
                case SmoothingType.SMA:
                    return SMA.Calculate(values);
                case SmoothingType.RMA:
                    return _rmaSmoother.Calculate(values);
                default:
                    throw new ArgumentException();
            }
        }

    }

    public enum SmoothingType { 
        SMA,
        RMA
    }
}
