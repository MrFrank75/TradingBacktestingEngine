using BacktestingEngine.Core;
using Google.Apis.Logging;

namespace BacktestingEngine
{
    internal class DateFilter : IFilter
    {
        private DateTime _dateTimeStart;
        private DateTime _dateTimeEnd;

        public DateFilter(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            _dateTimeStart = dateTimeStart;
            _dateTimeEnd = dateTimeEnd;
        }

        public Candlestick? FilterPrice(Candlestick price)
        {
            if (price.Time < _dateTimeStart || price.Time > _dateTimeEnd)
                return null;

            return price;
        }

        internal void PerformDateCheck(List<Candlestick> candles, string ticker)
        {
            if (candles.First().Time > _dateTimeStart)
            {
                Console.WriteLine($" *** WARNING. {ticker} data begins on {candles.First().Time}");
            }
            if (candles.Last().Time < _dateTimeEnd)
            {
                Console.WriteLine($" *** WARNING. {ticker} data ends on {candles.Last().Time}");
            }
        }
    }
}