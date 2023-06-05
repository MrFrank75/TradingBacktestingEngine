using BacktestingEngine.Core;
using BacktestingEngine.Indicators;

namespace BacktestingEngine.Strategies
{
    public class SupertrendStrategy
    {
        private decimal multiplier;
        private int period;
        private readonly decimal kcMultiplier;
        private readonly int kcPeriod;
        private readonly IFilter filter;
        private Candlestick previousCandle = new Candlestick { Close = decimal.MinValue};
        private readonly ATR atrCalculator;
        public readonly Supertrend supertrendIndicator;
        private long iterations = 0;
        private List<Candlestick> prices = new List<Candlestick>();

        private bool _trendingPositive = false;
        private bool _trendingNegative = false;
        private decimal _kcPivotLong = 0;
        private bool _condition1_EntryBearMarket = false;
        private bool _condition2_EntryBearMarket = false;
        private bool _condition1_EntryBullMarket = false;
        private bool _exitCondition1 = false;
        private bool _exitCondition2 = false;
        private bool _exitConditionSupertrendBroken = false;
        private TradingSignal _lastTradingSignalReleased = TradingSignal.None;

        public SupertrendStrategy(decimal stMultiplier, int stPeriod, decimal kcMultiplier, int kcPeriod, IFilter filter)
        {
            this.multiplier = stMultiplier;
            this.period = stPeriod;
            this.kcMultiplier = kcMultiplier;
            this.kcPeriod = kcPeriod;
            this.filter = filter;
            this.atrCalculator = new ATR(stPeriod);
            this.supertrendIndicator = new Supertrend(stPeriod, stMultiplier);
        }

        public Tuple<TradingSignal,decimal> GenerateSignal(Candlestick candle)
        {
            iterations++;
            prices.Add(candle);

            if (filter.FilterPrice(candle) == null)
                return new Tuple<TradingSignal, decimal>(TradingSignal.None, 0);

            var ema100 = EMA.Calculate(prices, 100);
            var ema200 = EMA.Calculate(prices, 200);
            KeltnerChannel kcResult = KeltnerChannel.Calculate(prices, period: kcPeriod, multiplier: kcMultiplier);
            SupertrendResult supertrendValue = supertrendIndicator.CalculateSuperTrend(candle);


            if (supertrendValue.CurrentTrend == Trend.Up && supertrendValue.TrendHasChanged)
            {
                _trendingPositive= true;
                _trendingNegative = false;
                _kcPivotLong = kcResult.Upper;
            }

            if (supertrendValue.CurrentTrend == Trend.Down && supertrendValue.TrendHasChanged)
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


            //EXIT CONDITIONS TO EVALUATE IF WE ARE IN A TRADE
            if (_lastTradingSignalReleased == TradingSignal.Buy)
            {
                if (candle.Close > kcResult.Upper && candle.Open > kcResult.Upper)
                {
                    _exitCondition1 = true;
                }

                if (_exitCondition1 && candle.Close < kcResult.Middle)
                {
                    _exitCondition2 = true;
                }

                if (candle.Close < supertrendValue.Value)
                {
                    _exitConditionSupertrendBroken = true;
                }
            }
            
            TradingSignal tradingSignal = TradingSignal.None;
            if ((_exitCondition1 && _exitCondition2) || _exitConditionSupertrendBroken)
            {
                tradingSignal = TradingSignal.Sell;
                _lastTradingSignalReleased= tradingSignal;
                _exitCondition1 = false;
                _exitCondition2= false;
                _exitConditionSupertrendBroken= false;
                _condition1_EntryBearMarket = false;
                _condition2_EntryBearMarket=false;
                _condition1_EntryBullMarket=false;
            }
            else if (_condition1_EntryBullMarket || (_condition1_EntryBearMarket && _condition2_EntryBearMarket))
            {
                tradingSignal = TradingSignal.Buy;
                _lastTradingSignalReleased= tradingSignal;
            }

            return new Tuple<TradingSignal, decimal>(tradingSignal,0);
        }

    }
}