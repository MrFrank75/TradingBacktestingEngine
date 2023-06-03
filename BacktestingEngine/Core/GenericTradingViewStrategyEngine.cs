namespace BacktestingEngine.Core
{
    internal class GenericTradingViewStrategyEngine : IStrategyExecutionEngine
    {
        private const int initialCapitalUSD = 1000;
        private const decimal MakerFee = 0.02M / 100M;
        private const decimal TakerFee = 0.04M / 100M;

        private List<Candlestick> _priceCandlesticks;
        private readonly IStrategy _strategy;

        private decimal _percentageUsedForTrade = 100;
        private decimal _currentCapital = initialCapitalUSD;

        private TradeState _currentStrategyState;

        private decimal _currentPositionSize;
        private decimal _openingPrice;
        private DateTime _openingDateTime;
        private decimal _currentContracts;

        public GenericTradingViewStrategyEngine(List<Candlestick> pricesVector, IStrategy strategyToExecute)
        {
            _priceCandlesticks = pricesVector;
            _strategy = strategyToExecute;
            _currentStrategyState = TradeState.Waiting;
        }

        public TradeExecutionResult ExecuteStrategy(Candlestick price)
        {

            var signal = _strategy.GenerateSignal(price);

            if (signal == Core.TradingSignal.Buy && _currentStrategyState == TradeState.Waiting)
            {
                var nominalOrderSize = _currentCapital * _percentageUsedForTrade / 100;
                _currentPositionSize = nominalOrderSize - nominalOrderSize * MakerFee;
                _openingPrice = price.Close;
                _openingDateTime = price.Time;
                _currentContracts = _currentPositionSize / price.Close;
                _currentStrategyState = TradeState.Running;
                return new TradeExecutionResult()
                {
                    OpeningDateTime = _openingDateTime,
                    State = TradeState.Opened
                };
            }

            if (signal == Core.TradingSignal.Sell && _currentStrategyState == TradeState.Running)
            {
                decimal nominalSellOrderSize = _currentContracts * price.Close;
                var contractsHeld = _currentContracts;
                var profit = nominalSellOrderSize - nominalSellOrderSize * TakerFee - _currentContracts * _openingPrice;
                _currentCapital = _currentCapital + profit;
                _currentContracts = 0;
                _currentStrategyState = TradeState.Waiting;
                return new TradeExecutionResult()
                {
                    OpeningDateTime = _openingDateTime,
                    ClosingDateTime = price.Time,
                    OpeningPrice = _openingPrice,
                    ClosingPrice = price.Close,
                    Contracts = contractsHeld,
                    Profit = profit,
                    CurrentCapital = _currentCapital,
                    State = TradeState.Closed
                };
            }

            return new TradeExecutionResult()
            {
                State = _currentContracts == 0 ? TradeState.Waiting : TradeState.Running
            };
        }

        public BacktestReport GetSummaryReport()
        {
            return new BacktestReport()
            {
                StartingCapital = initialCapitalUSD,
                EndingCapital = _currentCapital
            };
        }


    }
}
