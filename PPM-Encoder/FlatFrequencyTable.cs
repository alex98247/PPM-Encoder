using System;

namespace PPM_Encoder
{
    public class FlatFrequencyTable : IFrequencyTable
    {
        private const int NumSymbols = 257;

        public int Get(int symbol) => 1;

        public int GetTotal() => NumSymbols;

        public int GetLow(int symbol) => symbol;

        public int GetHigh(int symbol) => symbol + 1;

        public void Increment(int symbol) => new InvalidOperationException();
    }
}