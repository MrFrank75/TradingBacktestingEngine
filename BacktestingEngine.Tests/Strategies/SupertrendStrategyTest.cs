using BacktestingEngine.Core;

namespace BacktestingEngine.Test.Strategies
{
    public class SupertrendStrategyTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(10,TradingSignal.None, 47451.28)]
        [TestCase(11,TradingSignal.None, 47404.45)]
        [TestCase(12,TradingSignal.None, 47264.99)]
        [TestCase(17,TradingSignal.Buy, 46975.4)]
        [TestCase(18,TradingSignal.None, 47182.01)]
        [TestCase(19,TradingSignal.None, 47216.75)]
        [TestCase(20,TradingSignal.None, 47216.75)]
        [TestCase(21,TradingSignal.None, 47216.75)]
        [TestCase(100,TradingSignal.None, 47216.75)]
        public void RunningStrategyForGivenPeriodWillProduceExpectedTrendValue(int numOfItems, TradingSignal expectedSignal, decimal expectedSuperTrend)
        {
            var sut = new BacktestingEngine.Strategies.SupertrendStrategy(1,10,2,20);

            var pricesReader = new Core.PricesReader();
            var priceCandlesticks = pricesReader.ReadPricesVector("BINANCE", "BTCUSDT", "60").ToList();
            var initialDataSet = priceCandlesticks.Take(numOfItems).ToList();

            var actualStrategyResult = new Tuple<TradingSignal, decimal>(TradingSignal.None, 0);
            foreach (var candlestick in initialDataSet)
            {
                actualStrategyResult = sut.GenerateSignal(candlestick);
            }

            
            Assert.That(actualStrategyResult.Item1, Is.EqualTo(expectedSignal));
            Assert.That(Math.Round(actualStrategyResult.Item2,2), Is.EqualTo(expectedSuperTrend).Within(0.1m));
        }
    }
}