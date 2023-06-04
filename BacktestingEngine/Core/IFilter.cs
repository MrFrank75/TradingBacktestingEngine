namespace BacktestingEngine.Core
{
    public interface IFilter
    {
        Candlestick? FilterPrice(Candlestick price);
    }
}