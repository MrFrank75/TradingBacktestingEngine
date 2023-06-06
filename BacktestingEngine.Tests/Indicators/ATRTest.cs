using BacktestingEngine.Core;

namespace BacktestingEngine.Test.Indicators
{
    public class ATRTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(1, 523.02)]
        [TestCase(2, 375.93)]
        [TestCase(3, 206.98)]
        [TestCase(4, 156.51)]
        [TestCase(5, 296.10)]

        public void TrueRangeResultsTest(int period, decimal expectedTR)
        {
            var sut = new BacktestingEngine.Indicators.ATR(period);

            var pricesReader = new BacktestingEngine.Core.CsvReader<Candlestick>();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(period).ToList();

            var result = sut.CalculateTrueRange(initialDataSet.Last(), initialDataSet[initialDataSet.Count - 1]);

            Assert.AreEqual(expectedTR, result);
        }

        [Test]
        [TestCase(1, 523.02, 1)]
        [TestCase(2, 449.48, 2)]
        [TestCase(3, 368.64, 3)]
        [TestCase(3, 297.93, 4)]
        [TestCase(10, 376.71, 10)]
        [TestCase(10, 397.38, 11)]
        [TestCase(10, 382.74, 12)]
        [TestCase(10, 406.08, 20)]

        public void ReturnsExpectedResultUsingRMASmoothingTest(int period, decimal expectedTR, int datasetSize)
        {
            var sut = new BacktestingEngine.Indicators.ATR(period);

            var pricesReader = new BacktestingEngine.Core.CsvReader<Candlestick>();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(datasetSize).ToList();

            var result = sut.CalculateAverageTrueRange(initialDataSet,BacktestingEngine.Indicators.SmoothingType.RMA);

            Assert.That(Math.Round(result, 2), Is.EqualTo(expectedTR));
        }

        [Test]
        [TestCase(10, 376.71, 10)]
        [TestCase(10, 382.75, 11)]
        [TestCase(10, 370.25, 12)]
        public void AverageTrueRangeRMACalculationResultsTest(int period, decimal expectedTR, int datasetSize)
        {
            var sut = new BacktestingEngine.Indicators.ATR(period);

            var pricesReader = new BacktestingEngine.Core.CsvReader<Candlestick>();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(datasetSize).ToList();

            var result = sut.CalculateAverageTrueRange(initialDataSet,BacktestingEngine.Indicators.SmoothingType.SMA);

            Assert.That(Math.Round(result, 2), Is.EqualTo(expectedTR));
        }
    }
}