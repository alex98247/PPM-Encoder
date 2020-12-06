using System;
using System.IO;

namespace PPM_Encoder
{
    public class PpmCompressor
    {
        private const int ModelOrder = 4;
        private const int EscapeSymbol = 256;

        public void Compress(Stream inStream, BitOutputStream outStream)
        {
            var enc = new ArithmeticEncoder(outStream);
            var model = new PpmModel(ModelOrder, EscapeSymbol);

            var history = new int[0];
            while (true)
            {
                var symbol = inStream.ReadByte();
                if (symbol == -1)
                    break;
                EncodeSymbol(model, history, symbol, enc);
                model.IncrementContexts(history, symbol);

                if (model.ModelOrder >= 1)
                {
                    if (history.Length < model.ModelOrder)
                        Array.Resize(ref history, history.Length + 1);
                    Array.Copy(history, 0, history, 1, history.Length - 1);
                    history[0] = symbol;
                }
            }

            EncodeSymbol(model, history, EscapeSymbol, enc);
            enc.Finish();
        }


        private static void EncodeSymbol(PpmModel model, int[] history, int symbol, ArithmeticEncoder enc)
        {
            var order = history.Length;

            while (order >= 0)
            {
                var isBreak = false;
                var ctx = model.RootContext;
                for (var i = 0; i < order; i++)
                {
                    ctx = ctx.Subcontexts[history[i]];
                    if (ctx == null)
                    {
                        order--;
                        isBreak = true;
                        break;
                    }
                }

                if (isBreak)
                    continue;

                if (symbol != EscapeSymbol && ctx.Frequencies.Get(symbol) > 0)
                {
                    enc.Write(ctx.Frequencies, symbol);
                    return;
                }

                enc.Write(ctx.Frequencies, EscapeSymbol);
                order--;
            }

            enc.Write(model.orderMinus1Freqs, symbol);
        }
    }
}