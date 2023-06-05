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
        [TestCase(100,100, 46859.16)]
        [TestCase(20,20, 47059.60)]
        [TestCase(21,20, 47091.24)]
        [TestCase(22,20, 47112.99)]
        [TestCase(40,20, 47217.75)]
        [TestCase(41,20, 47272.47)]
        [TestCase(50,50, 47175.08)]

        public void EMACalculationTest(int datasetSize, int period, decimal expectedEMA)
        {

            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(datasetSize).ToList();

            var result = EMA.Calculate(initialDataSet, period);

            Assert.That(result, Is.EqualTo(expectedEMA).Within(0.01));
        }
    }
}