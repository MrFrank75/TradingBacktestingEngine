using BacktestingEngine.Strategies;

namespace BacktestingEngine.Test.Strategies
{
    public class KeltnerChannelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(20,20,2, 47862.88, 46319.60,47091.24)]

        public void KeltnerChannelCalculationTest(int numOfSamples,int period,decimal multiplier,decimal expectedUpper, decimal expectedLower, decimal expectedMiddle)
        {

            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(numOfSamples).TakeLast(period).ToList();

            var result = KeltnerChannel.Calculate(initialDataSet,period, multiplier);

            Assert.That(result.Upper, Is.EqualTo(expectedUpper).Within(0.01));
            Assert.That(result.Lower, Is.EqualTo(expectedLower).Within(0.01));
            Assert.That(result.Middle, Is.EqualTo(expectedMiddle).Within(0.01));
        }
    }
}