namespace BacktestingEngine.Core
{
    internal class GenericTradingViewStrategyEngine : IStrategyExecutionEngine
    {
        private const int initialCapitalUSD = 1000;
        private const decimal MakerFee = 0.02M / 100M;
        private const decimal TakerFee = 0.04M / 100M;

        private readonly IStrategy _strategy;
        private readonly string _ticker;
        private decimal _percentageUsedForTrade = 100;
        private decimal _currentCapital = initialCapitalUSD;

        private TradeState _currentStrategyState;

        private decimal _currentPositionSize;
        private decimal _openingPrice;
        private DateTime _openingDateTime;
        private decimal _currentContracts;
        private List<TradeExecutionResult> _executedTrades;

        public GenericTradingViewStrategyEngine(IStrategy strategyToExecute, string ticker)
        {
            _strategy = strategyToExecute;
            _ticker = ticker;
            _currentStrategyState = TradeState.Waiting;
            _executedTrades = new List<TradeExecutionResult>();
        }

        public TradeExecutionResult ExecuteStrategy(Candlestick price)
        {

            var signal = _strategy.GenerateSignal(price);
            var nominalBuyOrderSize = _currentCapital * _percentageUsedForTrade / 100;

            if (signal == Core.TradingSignal.Buy && _currentStrategyState == TradeState.Waiting)
            {
                _currentPositionSize = nominalBuyOrderSize - nominalBuyOrderSize * MakerFee;
                _openingPrice = price.Close;
                _openingDateTime = price.Time;
                _currentContracts = _currentPositionSize / price.Close;
                _currentStrategyState = TradeState.Running;
                return new TradeExecutionResult()
                {
                    Ticker = _ticker,
                    OpeningDateTime = _openingDateTime,
                    State = TradeState.Opened
                };
            }

            if (signal == Core.TradingSignal.Sell && _currentStrategyState == TradeState.Running)
            {
                var contractsHeld = _currentContracts;
                var nominalSellOrderSize = _currentContracts * price.Close;
                var profit = nominalSellOrderSize - nominalSellOrderSize * TakerFee - nominalBuyOrderSize;
                var profitPercentage = Math.Round(profit / nominalBuyOrderSize * 100.0M, 2);
                _currentCapital = _currentCapital + profit;
                _currentContracts = 0;
                _currentStrategyState = TradeState.Waiting;
                var tradeResult = new TradeExecutionResult()
                {
                    Ticker = _ticker,
                    OpeningDateTime = _openingDateTime,
                    ClosingDateTime = price.Time,
                    OpeningPrice = _openingPrice,
                    ClosingPrice = price.Close,
                    Contracts = contractsHeld,
                    Gain = profit,
                    GainPerc = profitPercentage,
                    CurrentCapital = _currentCapital,
                    State = TradeState.Closed
                };
                _executedTrades.Add(tradeResult);
                return tradeResult;
            }

            return GetCurrentTradeExecutionStatus();
        }

        private TradeExecutionResult GetCurrentTradeExecutionStatus()
        {
            return new TradeExecutionResult()
            {
                State = _currentContracts == 0 ? TradeState.Waiting : TradeState.Running
            };
        }

        public BacktestReport GetSummaryReport()
        {
            //calculate max drawdown
            var maxDrawdown = GetMaxDrawDown(_executedTrades);

            return new BacktestReport()
            {
                Ticker = _ticker,
                StartingCapital = initialCapitalUSD,
                EndingCapital = Math.Round(_currentCapital,2),
                Gain = Math.Round(_currentCapital-initialCapitalUSD,2),
                GainPerc = Math.Round((_currentCapital - initialCapitalUSD) /initialCapitalUSD*100,2),
                MaxDrawdown = Math.Round(maxDrawdown,2),
                MaxDrawdownPercent = Math.Round((maxDrawdown/initialCapitalUSD*100.0M),2)
            };
        }

        private decimal GetMaxDrawDown(List<TradeExecutionResult> executedTrades)
        {
            var minEquityValue = Decimal.MaxValue;
            foreach (var item in executedTrades)
            {
                if (item.CurrentCapital < minEquityValue)
                    minEquityValue = item.CurrentCapital;
            }
            return initialCapitalUSD - minEquityValue;
        }
    }
}
