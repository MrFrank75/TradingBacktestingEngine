namespace BacktestingEngine.Core
{
    public class Candlestick
    {
        public long UnixTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public DateTime Time => UnixTimeToDateTime(UnixTime);

        private static DateTime UnixTimeToDateTime(long unixTime)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixEpoch.AddSeconds(unixTime).ToLocalTime();
        }

    }

}