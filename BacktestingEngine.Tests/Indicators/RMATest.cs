
namespace BacktestingEngine.Test.Indicators
{
    public class RMATest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(3, 3, 46748.68)]
        [TestCase(3, 4, 46770.19)]
        [TestCase(3, 10, 47082.39)]
        [TestCase(10, 10, 46940.60)]
        [TestCase(10, 15, 46939.60)]
        public void RMAResultsTest(int period, int datasetSize, decimal expectedRMA)
        {
            var pricesReader = new BacktestingEngine.Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("TESTBROKER", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(datasetSize).ToList();


            var sut = new BacktestingEngine.Indicators.RMA(period);
            var result = 0M;
            foreach (var stick in initialDataSet)
            {
                result = sut.Calculate(stick.Close);

            }

            Assert.That(result, Is.EqualTo(expectedRMA).Within(0.1));
        }


        [Test]
        [TestCase(1, 3)]
        [TestCase(3, 2)]
        public void RMA_ReturnsCorrectValue(int period, double expectedRMA)
        {
            // Arrange
            List<decimal> src = new List<decimal> { 1, 2, 3 };

            // Act
            decimal result = 0;
            BacktestingEngine.Indicators.RMA sut = new BacktestingEngine.Indicators.RMA(period);

            foreach (var singleValue in src)
            {
                result = sut.Calculate(singleValue);

            }

            // Assert
            Assert.That((double)result, Is.EqualTo(expectedRMA).Within(0.0001));
        }

        [Test]
        public void SMA_ReturnsCorrectValue()
        {
            // Arrange
            List<decimal> src = new List<decimal> { 1, 2 };

            // Act
            decimal result = BacktestingEngine.Indicators.SMA.Calculate(src);

            // Assert
            Assert.That((double)result, Is.EqualTo((double)1.5M).Within(0.0001));

        }
    }
}