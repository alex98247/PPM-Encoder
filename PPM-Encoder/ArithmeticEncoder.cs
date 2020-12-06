namespace PPM_Encoder
{
    public class ArithmeticEncoder
    {
        private readonly BitOutputStream _output;
        private int _numUnderflow;

        private readonly long _halfRange;

        private readonly long _quarterRange;

        private readonly long _stateMask;

        private long _low;

        private long _high;

        public ArithmeticEncoder(BitOutputStream outStream)
        {
            const long fullRange = 1L << 32;
            _halfRange = fullRange >> 1; 
            _quarterRange = _halfRange >> 1; 
            _stateMask = fullRange - 1;

            _low = 0;
            _high = _stateMask;

            _output = outStream;
            _numUnderflow = 0;
        }

        public void Write(IFrequencyTable freqs, int symbol)
        {
            var range = _high - _low + 1;

            long total = freqs.GetTotal();
            long symLow = freqs.GetLow(symbol);
            long symHigh = freqs.GetHigh(symbol);

            // Update range
            var newLow = _low + symLow * range / total;
            var newHigh = _low + symHigh * range / total - 1;
            _low = newLow;
            _high = newHigh;

            // Shift bits
            while (((_low ^ _high) & _halfRange) == 0)
            {
                Shift();
                _low = ((_low << 1) & _stateMask);
                _high = ((_high << 1) & _stateMask) | 1;
            }

            // Underflow
            while ((_low & ~_high & _quarterRange) != 0)
            {
                _numUnderflow++;
                _low = (_low << 1) ^ _halfRange;
                _high = ((_high ^ _halfRange) << 1) | _halfRange | 1;
            }
        }

        public void Finish() => _output.Write(1);

        private void Shift()
        {
            var bit = (int) (_low >> 31);
            _output.Write(bit);

            for (; _numUnderflow > 0; _numUnderflow--)
                _output.Write(bit ^ 1);
        }
    }
}