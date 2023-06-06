using BacktestingEngine.Core;
using BacktestingEngine.Indicators;

namespace BacktestingEngine.Test.Indicators
{
    public class SupertrendTest
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        [TestCase(10,48204.71)]
        [TestCase(11,48199.21)]
        [TestCase(12,48030.46)]
        public void SupertrendBTC_ReturnsExpectedValues(int numOfItems, decimal expectedSuperTrend)
        {
            var sut = new BacktestingEngine.Indicators.Supertrend(10, 3);

            var pricesReader = new CsvReader<Candlestick>();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(numOfItems).ToList();

            SupertrendResult result = new SupertrendResult();
            foreach (var candlestick in initialDataSet)
            {
                result = sut.CalculateSuperTrend(candlestick);
            }


            Assert.That(result.Value, Is.EqualTo(expectedSuperTrend).Within(0.1m));
        }

        [Test]
        [TestCase(10, 0.0083)]
        [TestCase(11, 0.0079)]
        [TestCase(18, 0.0030)]
        public void SupertrendMATIC_ReturnsExpectedValues(int numOfItems, decimal expectedSuperTrend)
        {
            var sut = new BacktestingEngine.Indicators.Supertrend(10, 3);

            var pricesReader = new CsvReader<Candlestick>();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "MATICUSDT", "1D").ToList();
            var initialDataSet = priceCandlesticks.Take(numOfItems).ToList();
            
            SupertrendResult result = new SupertrendResult();
            foreach (var candlestick in initialDataSet)
            {
                result = sut.CalculateSuperTrend(candlestick);
            }


            Assert.That(result.Value, Is.EqualTo(expectedSuperTrend).Within(0.0001m));
        }
    }
}