using BacktestingEngine.Core;

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
    }
}