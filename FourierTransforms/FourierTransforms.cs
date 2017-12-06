using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace AbrahamOdamteng
{
    public static class FourierTransforms
    {
        public static void FastFourierTransform(Complex[] inputSamples,int startIndex, int size)
        {

            if (size < 2) return;
            if (!Utils.IsPowerOfTwo(size))
            {
                throw new ArgumentException("the number of inputSamples must be power of 2");
            }

            var n = size / 2;
            Separate(inputSamples, startIndex,size);
            FastFourierTransform(inputSamples, startIndex, n);
            FastFourierTransform(inputSamples, startIndex + n, n);

            for(int k =0; k < n; k++)
            {
                var ie = k + startIndex;
                var io = k + n + startIndex;
                Complex even = inputSamples[ie];
                Complex odd = inputSamples[io];
                Complex w = Complex.Exp(new Complex(0, -2d * Math.PI * k / size));

                inputSamples[ie] = even + w * odd;
                inputSamples[io] = even - w * odd;
            }
        }

        
        internal static void Separate(Complex[] inputSamples, int startIndex, int size)
        {
            if (startIndex < 0)
            {
                throw new IndexOutOfRangeException("startIndex must be greater than or equal to zero");
            }
                
            if (startIndex >= inputSamples.Length)
            {
                throw new IndexOutOfRangeException("startIndex is too large for array 'inputSamples'");
            }

            if ((startIndex + size -1) >= inputSamples.Length)
            {
                throw new IndexOutOfRangeException("startIndex + size is too large for array 'inputSamples'");
            }

            if ((size != 0) && (size & (size - 1)) != 0)
            {
                throw new ArgumentException("size must be power of 2");
            }

            var n = size / 2;
            var oddIndexes = new Complex[size];
            //Odd indexies
            for (int i = 0; i < n; i++)
            {
                oddIndexes[i] = inputSamples[i * 2 + 1 + startIndex];
            }

            //Even indexies
            for (int i = 0; i < n; i++)
            {
                inputSamples[startIndex + i] = inputSamples[i * 2 + startIndex];
            }

            for (int i = 0; i < n; i++)
            {
                inputSamples[startIndex + n +i] = oddIndexes[i];
            }
        }

        public static Complex[] DiscreteFourierTransform(Complex[] inputSamples)
        {
            var result = new LinkedList<Complex>();

            for(int k =0; k< inputSamples.Length; k++)
            {
                var val = new Complex(0, 0);
                for(int n = 0;n < inputSamples.Length; n++)
                {
                    val += inputSamples[n] * Complex.Exp(new Complex(0, (-2.0 * Math.PI * k * n) / inputSamples.Length));
                }
                result.AddLast(val);
            }

            return result.ToArray();
        }
    }
}
