using System;
using System.IO;

namespace PPM_Encoder
{
    public class BitOutputStream : IDisposable
    {
        private readonly Stream _output;

        private int _currentByte;

        private int _numBitsFilled;

        public BitOutputStream(Stream outStream)
        {
            this._output = outStream;
            _currentByte = 0;
            _numBitsFilled = 0;
        }

        public void Write(int b)
        {
            if (b != 0 && b != 1)
                throw new ArgumentException("Argument must be 0 or 1");
            _currentByte = (_currentByte << 1) | b;
            _numBitsFilled++;
            if (_numBitsFilled == 8)
            {
                _output.WriteByte((byte) _currentByte);
                _currentByte = 0;
                _numBitsFilled = 0;
            }
        }

        public void Close()
        {
            while (_numBitsFilled != 0)
                Write(0);

            _output.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}