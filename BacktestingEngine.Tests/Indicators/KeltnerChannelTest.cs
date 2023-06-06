using BacktestingEngine.Core;
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
        [TestCase(20,20,2, 47871.75, 46247.44,47059.60)]
        [TestCase(21,20,2, 47883.94, 46298.53,47091.24)]

        public void KeltnerChannelCalculationTest(int numOfSamples,int period,decimal multiplier,decimal expectedUpper, decimal expectedLower, decimal expectedMiddle)
        {

            var pricesReader = new BacktestingEngine.Core.CsvReader<Candlestick>();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(numOfSamples).ToList();

            var result = KeltnerChannel.Calculate(initialDataSet,period, multiplier);

            Assert.That(result.Upper, Is.EqualTo(expectedUpper).Within(0.01));
            Assert.That(result.Lower, Is.EqualTo(expectedLower).Within(0.01));
            Assert.That(result.Middle, Is.EqualTo(expectedMiddle).Within(0.01));
        }
    }
}