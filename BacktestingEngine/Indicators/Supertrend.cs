using BacktestingEngine.Core;
using BacktestingEngine.Indicators;

namespace BacktestingEngine.Indicators
{
    public class Supertrend {
        private readonly int period;
        private readonly decimal multiplier;
        private decimal[] upperBand = new decimal[2];
        private decimal[] lowerBand = new decimal[2];
        private decimal[] superTrend = new decimal[2];
        private decimal[] atrValues = new decimal[2];

        private int CURRENT = 0;
        private int PREVIOUS = 1;
        private ATR atrCalculator;
        private List<Candlestick> prices;

        private Trend previousTrend = Trend.None;
        private Candlestick previousCandle = new Candlestick { Close = 0};

        public Supertrend(int period, decimal multiplier)
        {
            this.atrCalculator = new ATR(period);
            this.prices = new List<Candlestick>();
            this.period = period;
            this.multiplier = multiplier;
        }

        public SupertrendResult CalculateSuperTrend(Candlestick candle)
        {
            prices.Add(candle);
            var  currentTrend = previousTrend;
            var trendHasChangedToDown = false;
            var trendHasChangedToUp = false;

            decimal hl2 = (candle.High + candle.Low) / 2;

            // Calculate the Average True Range (ATR)
            atrValues[CURRENT] = atrCalculator.CalculateAverageTrueRange(prices, SmoothingType.RMA);
            var atr = atrValues[CURRENT];

            if (prices.Count >= period)
            {
                upperBand[CURRENT] = hl2 + this.multiplier * atr;
                lowerBand[CURRENT] = hl2 - this.multiplier * atr;

                lowerBand[CURRENT] = lowerBand[CURRENT] > lowerBand[PREVIOUS] || previousCandle.Close < lowerBand[PREVIOUS] ? lowerBand[CURRENT] : lowerBand[PREVIOUS];
                upperBand[CURRENT] = upperBand[CURRENT] < upperBand[PREVIOUS] || previousCandle.Close > upperBand[PREVIOUS] ? upperBand[CURRENT] : upperBand[PREVIOUS];

                var prevSuperTrend = superTrend[PREVIOUS];
                if (atrValues[PREVIOUS] == 0)
                {
                    currentTrend = Trend.Down;
                }
                else if (prevSuperTrend == upperBand[PREVIOUS])
                {
                    currentTrend = candle.Close > upperBand[CURRENT] ? Trend.Up : Trend.Down;
                }
                else
                    currentTrend = candle.Close < lowerBand[CURRENT] ? Trend.Down : Trend.Up;

                superTrend[CURRENT] = currentTrend == Trend.Up ? lowerBand[CURRENT] : upperBand[CURRENT];

                trendHasChangedToDown = previousTrend == Trend.Up && candle.Close < superTrend[CURRENT];
                if (trendHasChangedToDown)
                {
                    currentTrend = Trend.Down;
                }

                trendHasChangedToUp = previousTrend == Trend.Down && candle.Close > superTrend[CURRENT];
                if (trendHasChangedToUp)
                {
                    currentTrend = Trend.Up;
                }
            }

            //prepare everything for the next iteration
            atrValues[PREVIOUS] = atr;
            superTrend[PREVIOUS] = superTrend[CURRENT];
            upperBand[PREVIOUS] = upperBand[CURRENT];
            lowerBand[PREVIOUS] = lowerBand[CURRENT];
            previousCandle = candle;
            previousTrend = currentTrend;

            return new SupertrendResult()
            {
                CurrentTrend = currentTrend,
                TrendHasChanged = trendHasChangedToDown || trendHasChangedToUp,
                Value = superTrend[CURRENT]
            };
        }
    }
}