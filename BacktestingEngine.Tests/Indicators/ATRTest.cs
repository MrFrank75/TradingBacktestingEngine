namespace BacktestingEngine.Test.Strategies
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

            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(period).ToList();

            var result = sut.CalculateTrueRange(initialDataSet);

            Assert.AreEqual(expectedTR, result);
        }

        [Test]
        [TestCase(1, 523.02, 1)]
        [TestCase(2, 449.48, 2)]
        [TestCase(3, 368.64, 3)]
        [TestCase(3, 246.47, 4)]
        [TestCase(10, 376.71, 10)]
        [TestCase(10, 382.75, 11)]
        [TestCase(10, 370.25, 12)]

        public void AverageTrueRangeResultsTest(int period, decimal expectedTR, int datasetSize)
        {
            var sut = new BacktestingEngine.Indicators.ATR(period);

            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(datasetSize).ToList();

            var result = sut.CalculateAverageTrueRange(initialDataSet);

            Assert.AreEqual(expectedTR, Math.Round(result, 2));
        }

        [Test]
        [TestCase(10, 406.08, 20)]
        public void AverageTrueRangeRMACalculationResultsTest(int period, decimal expectedTR, int datasetSize)
        {
            var sut = new BacktestingEngine.Indicators.ATR(period);

            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(datasetSize).ToList();

            var result = sut.CalculateAverageTrueRange(initialDataSet,Indicators.SmoothingType.RMA);

            Assert.AreEqual(expectedTR, Math.Round(result, 2));
        }
    }
}