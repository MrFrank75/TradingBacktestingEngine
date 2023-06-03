namespace BacktestingEngine.Indicators
{
    /// <summary>
    /// this class has a state. You need to reinstanciate it if you want to make a new calculation for a new series
    /// </summary>
    public class RMA
    {
        private readonly int _period;
        private readonly List<decimal> _valuesArray;
        private decimal _alpha;
        private decimal _prevRma;

        public RMA(int period)
        {
            _period = period;
            _valuesArray = new List<decimal>();
            _alpha = 1.0M/period;
            _prevRma = 0;
        }

        public decimal Calculate(decimal value) 
        {
            decimal rma = 0;
            _valuesArray.Add(value);
            if (_valuesArray.Count > _period) {
                _valuesArray.RemoveAt(0);   
            }
            if (_valuesArray.Count < _period) {
                return 0;
            }

            if (_prevRma == 0)
            {
                rma = SMA.Calculate(_valuesArray);
            }
            else
            {
                rma = _alpha * value+ ((1-_alpha)* _prevRma);
            }
            _prevRma = rma;
            return rma;
        }

        public decimal Calculate(List<decimal> values)
        {
            decimal rma = 0;
            if (values.Count < _period)
            {
                return 0;
            }

            List<decimal> neededValues = values.TakeLast(_period).ToList();


            if (_prevRma == 0)
            {
                rma = SMA.Calculate(neededValues);
            }
            else
            {
                rma = _alpha * neededValues.Last() + ((1 - _alpha) * _prevRma);
            }
            _prevRma = rma;
            return rma;
        }
    }
}
