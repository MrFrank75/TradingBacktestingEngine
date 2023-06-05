using BacktestingEngine.Core;
using BacktestingEngine.Indicators;

namespace BacktestingEngine.Strategies
{
    public class KeltnerChannel
    {
        public decimal Upper { get; private set; }
        public decimal Lower { get; private set; }
        public decimal Middle{ get; private set; }


        public static KeltnerChannel Calculate(List<Candlestick> prices, int period, decimal multiplier, int atrPeriod = 10)
        {
            decimal[] highPrices = prices.Select(p=>p.High).ToArray();
            decimal[] lowPrices = prices.Select(p => p.Low).ToArray();
            decimal[] closePrices = prices.Select(p => p.Close).ToArray();

            // Calculate EMA
            decimal emaValue = EMA.Calculate(prices, period);
            decimal atrValue = new ATR(atrPeriod).CalculateAverageTrueRange(prices, SmoothingType.RMA);

            decimal upperValue = emaValue + (multiplier * atrValue);
            decimal lowerValue = emaValue - (multiplier * atrValue);

            return new KeltnerChannel
            {
                Upper = upperValue,
                Lower = lowerValue,
                Middle = emaValue
            };
        }
    }
}