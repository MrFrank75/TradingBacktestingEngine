using BacktestingEngine.Strategies;

namespace BacktestingEngine.Test.Strategies
{
    public class EMATest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(100, 46859.16)]
        [TestCase(20, 47059.60)]
        [TestCase(50, 47175.08)]

        public void EMACalculationTest(int period, decimal expectedEMA)
        {

            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(period).ToList();

            var result = EMA.Calculate(initialDataSet, period);

            Assert.That(result, Is.EqualTo(expectedEMA).Within(0.01));
        }
    }
}