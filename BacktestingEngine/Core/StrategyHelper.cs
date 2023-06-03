namespace BacktestingEngine.Core
{
    public static class StrategyHelper
    {

        public static List<T> GetElementsUntilAndIncluding<T>(List<T> list, T element)
        {
            int index = list.IndexOf(element);
            List<T> ts = index >= 0 ? list.Take(index).ToList() : list;
            ts.Add(element);
            return ts;
        }
    }
}