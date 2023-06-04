using BacktestingEngine.Core;
using BacktestingEngine.Indicators;
using System.ComponentModel;
using System.Threading;

namespace BacktestingEngine.Strategies
{
    public class SupertrendStrategy
    {
        private decimal multiplier;
        private int period;
        private readonly decimal kcMultiplier;
        private readonly int kcPeriod;
        private decimal[] upperBand = new decimal[2];
        private decimal[] lowerBand = new decimal[2];
        private decimal[] superTrend = new decimal[2];
        private decimal[] atrValues = new decimal[2];
        private Candlestick previousCandle = new Candlestick { Close = decimal.MinValue};
        private readonly ATR atrCalculator;
        private long iterations = 0;
        private List<Candlestick> prices = new List<Candlestick>();
        private int CURRENT = 0;
        private int PREVIOUS = 1;
        private Trend previousTrend = Trend.None;

        private bool _trendingPositive = false;
        private bool _trendingNegative = false;
        private decimal _kcPivotLong = 0;
        private bool _condition1_EntryBearMarket = false;
        private bool _condition2_EntryBearMarket = false;
        private bool _condition1_EntryBullMarket = false;
        private bool _exitCondition1 = false;
        private bool _exitCondition2 = false;

        public SupertrendStrategy(decimal stMultiplier, int stPeriod, decimal kcMultiplier, int kcPeriod)
        {
            this.multiplier = stMultiplier;
            this.period = stPeriod;
            this.kcMultiplier = kcMultiplier;
            this.kcPeriod = kcPeriod;
            this.atrCalculator = new ATR(stPeriod);
        }

        public Tuple<TradingSignal,decimal> GenerateSignal(Candlestick candle)
        {
            iterations++;
            prices.Add(candle);

            if (iterations < period)
            {
                previousCandle = candle;
                return new Tuple<TradingSignal, decimal>(TradingSignal.None, superTrend[CURRENT]);
            }

            var ema100 = EMA.Calculate(prices,100);
            var ema200 = EMA.Calculate(prices,200);
            KeltnerChannel kcResult = KeltnerChannel.Calculate(prices, period: kcPeriod, multiplier: kcMultiplier);
            bool trendHasChangedToDown, trendHasChangedToUp;
            CalculateSuperTrend(candle, out trendHasChangedToDown, out trendHasChangedToUp);

            if (trendHasChangedToUp)
            {
                _trendingPositive= true;
                _trendingNegative = false;
                _kcPivotLong = kcResult.Upper;
            }

            if (trendHasChangedToDown)
            {
                _trendingPositive = false;
                _trendingNegative = true;
            }

            //ENTRY CONDITIONS
            if (_trendingPositive && ema100 < ema200 && candle.Close > _kcPivotLong) {
                _condition1_EntryBearMarket = true;    
            };

            if (_condition1_EntryBearMarket && candle.Close < kcResult.Middle)
            {
                _condition2_EntryBearMarket = true;
            }

            if (_trendingPositive && ema100 > ema200)
            {
                _condition1_EntryBullMarket = true;
            }


            //EXIT CONDITIONS
            if (candle.Close>kcResult.Upper && candle.Open>kcResult.Upper)
            {
                _exitCondition1 = true;
            }

            if (_exitCondition1 && candle.Close < kcResult.Middle) {
                _exitCondition2 = true;
            }


            TradingSignal tradingSignal = TradingSignal.None;
            if (_exitCondition1 && _exitCondition2)
            {
                tradingSignal = TradingSignal.Sell;
                _exitCondition1 = false;
                _exitCondition2= false;
                _condition1_EntryBearMarket = false;
                _condition2_EntryBearMarket=false;
                _condition1_EntryBullMarket=false;
            }
            else if (_condition1_EntryBullMarket || (_condition1_EntryBearMarket && _condition2_EntryBearMarket))
            {
                tradingSignal = TradingSignal.Buy;
            }

            return new Tuple<TradingSignal, decimal>(tradingSignal, superTrend[CURRENT]);
        }

        private void CalculateSuperTrend(Candlestick candle, out bool trendHasChangedToDown, out bool trendHasChangedToUp)
        {
            decimal hl2 = (candle.High + candle.Low) / 2;

            // Calculate the Average True Range (ATR)
            atrValues[CURRENT] = Math.Round(atrCalculator.CalculateAverageTrueRange(prices, SmoothingType.RMA), 2);
            var atr = atrValues[CURRENT];

            upperBand[CURRENT] = hl2 + multiplier * atr;
            lowerBand[CURRENT] = hl2 - multiplier * atr;

            lowerBand[CURRENT] = lowerBand[CURRENT] > lowerBand[PREVIOUS] || previousCandle.Close < lowerBand[PREVIOUS] ? lowerBand[CURRENT] : lowerBand[PREVIOUS];
            upperBand[CURRENT] = upperBand[CURRENT] < upperBand[PREVIOUS] || previousCandle.Close > upperBand[PREVIOUS] ? upperBand[CURRENT] : upperBand[PREVIOUS];

            var currentTrend = Trend.None;
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

            //prepare everything for the next iteration
            atrValues[PREVIOUS] = atr;
            superTrend[PREVIOUS] = superTrend[CURRENT];
            upperBand[PREVIOUS] = upperBand[CURRENT];
            lowerBand[PREVIOUS] = lowerBand[CURRENT];
            previousCandle = candle;
            previousTrend = currentTrend;
        }

    }
}