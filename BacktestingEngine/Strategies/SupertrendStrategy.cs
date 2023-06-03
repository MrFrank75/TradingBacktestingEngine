﻿using BacktestingEngine.Core;
using BacktestingEngine.Indicators;
using System.ComponentModel;
using System.Threading;

namespace BacktestingEngine.Strategies
{
    public class SupertrendStrategy
    {
        private decimal multiplier;
        private int period;
        private decimal[] upperBand = new decimal[2];
        private decimal[] lowerBand = new decimal[2];
        private decimal[] superTrend = new decimal[2];
        private decimal[] atrValues = new decimal[2];
        private Candlestick previousCandle = new Candlestick { Close = decimal.MinValue};
        private readonly ATR atrCalculator;
        private long iterations = 0;
        private List<Candlestick> latestCandlesticks = new List<Candlestick>();
        private int CURRENT = 0;
        private int PREVIOUS = 1;
        private Trend previousTrend = Trend.None;


        public SupertrendStrategy(decimal multiplier, int period)
        {
            this.multiplier = multiplier;
            this.period = period;
            this.atrCalculator = new ATR(period);
        }

        public Tuple<TradingSignal,decimal> GenerateSignal(Candlestick candle)
        {
            iterations++;
            latestCandlesticks.Add(candle);
            decimal hl2 = (candle.High+candle.Low)/2;

            if (iterations < period)
            {
                previousCandle = candle;
                return new Tuple<TradingSignal, decimal>(TradingSignal.None, superTrend[CURRENT]);
            }

            // Calculate the Average True Range (ATR)
            atrValues[CURRENT] = Math.Round(atrCalculator.CalculateAverageTrueRange(latestCandlesticks,SmoothingType.RMA),2);
            var atr = atrValues[CURRENT];

            upperBand[CURRENT] = hl2 + multiplier * atr;
            lowerBand[CURRENT] = hl2 - multiplier * atr;

            lowerBand[CURRENT] = lowerBand[CURRENT] > lowerBand[PREVIOUS] || previousCandle.Close < lowerBand[PREVIOUS] ? lowerBand[CURRENT] : lowerBand[PREVIOUS];
            upperBand[CURRENT] = upperBand[CURRENT] < upperBand[PREVIOUS] || previousCandle.Close > upperBand[PREVIOUS] ? upperBand[CURRENT] : upperBand[PREVIOUS];

            var currentTrend = Trend.None;
            var prevSuperTrend = superTrend[PREVIOUS];
            if (atrValues[PREVIOUS] == 0) {
                currentTrend = Trend.Down;
            } else if (prevSuperTrend == upperBand[PREVIOUS]) {
                currentTrend = candle.Close > upperBand[CURRENT] ? Trend.Up : Trend.Down;
            }
            else
                currentTrend = candle.Close < lowerBand[CURRENT] ? Trend.Down : Trend.Up;
            superTrend[CURRENT] = currentTrend == Trend.Up ? lowerBand[CURRENT] : upperBand[CURRENT];


            var tradingSignal = TradingSignal.None;
            if (previousTrend == Trend.Up && candle.Close< superTrend[CURRENT])
            {
                currentTrend = Trend.Down;
                tradingSignal = TradingSignal.Sell;
            }
            else if (previousTrend == Trend.Down && candle.Close > superTrend[CURRENT])
            {
                currentTrend = Trend.Up;
                tradingSignal = TradingSignal.Buy;
            }

            //prepare everything for the next iteration
            atrValues[PREVIOUS] = atr;
            superTrend[PREVIOUS] = superTrend[CURRENT];
            upperBand[PREVIOUS] = upperBand[CURRENT];
            lowerBand[PREVIOUS] = lowerBand[CURRENT];
            previousCandle = candle;
            previousTrend = currentTrend;


            return new Tuple<TradingSignal, decimal>(tradingSignal, superTrend[CURRENT]);
        }


    }
}