namespace BacktestingEngine.Indicators
{
    public static class SMA
    {

        public static decimal Calculate(List<decimal> src)
        {
            decimal sum = 0.0m;

            for (int i = 0; i < src.Count; i++)
            {
                sum += src[i];
            }

            return sum / src.Count;
        }
    }
}