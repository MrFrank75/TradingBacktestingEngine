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
        [TestCase(10, 0.0083)]
        public void Supertrend_ReturnsExpectedValues(int numOfItems, decimal expectedSuperTrend)
        {
            var sut = new BacktestingEngine.Indicators.Supertrend(10, 3);

            var pricesReader = new PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "MATICUSDT", "1D").ToList();
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