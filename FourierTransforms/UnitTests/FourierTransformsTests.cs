using NUnit.Framework;
using AbrahamOdamteng.CSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AbrahamOdamteng
{
    [TestFixture]
    class FourierTransformsTests
    {

        [Test]
        public void Test_Separate_StartIndexSmallerThanZero()
        {
            var inputSample = Enumerable.Range(0, 16).Select(i => (Complex)i).ToArray();
            Assert.Throws<IndexOutOfRangeException>(
                () => FourierTransforms.Separate(inputSample, -5, 16),
                "startIndex must be greater than or equal to zero");
        }


        [Test]
        public void Test_Separate_StartIndexGreaterThanArraySize()
        {
            var inputSample = Enumerable.Range(0, 16).Select(i => (Complex)i).ToArray();
            Assert.Throws<IndexOutOfRangeException>(
                () => FourierTransforms.Separate(inputSample, 99, 16),
                "startIndex is too large for array 'inputSamples'");
        }


        [Test]
        public void Test_Separate_StartIndexPlusSizeIsGreaterThanArraySize()
        {
            var inputSample = Enumerable.Range(0, 16).Select(i => (Complex)i).ToArray();
            Assert.Throws<IndexOutOfRangeException>(
                () => FourierTransforms.Separate(inputSample, 1, 16),
                "startIndex + size is too large for array 'inputSamples'");
        }


        [Test]
        public void Test_Separate_SeparateSizeNotPowerOfTwo()
        {
            var inputSample = Enumerable.Range(0, 16).Select(i => (Complex)i).ToArray();
            Assert.Throws<ArgumentException>(
                () => FourierTransforms.Separate(inputSample, 0, 15),
                "size must be power of 2");
        }

        [Test]
        public void Test_Separate_Entire_Array()
        {
            var inputSize = 16;
            var inputSample = Enumerable.Range(0, inputSize).Select(i => (Complex)i).ToArray();
            FourierTransforms.Separate(inputSample, 0, inputSize);

            var evenIndexies = Enumerable.Range(0, inputSize)
                .Where((x, i) => i % 2 == 0)
                .Select(x => (Complex)x);

            var oddIndexies = Enumerable.Range(0, inputSize)
                .Where((x, i) => i % 2 != 0)
                .Select(x => (Complex)x);

            var expectedResult = evenIndexies.Concat(oddIndexies).ToArray();
            Assert.IsTrue(inputSample.SequenceEqual(expectedResult));
        }

        [Test]
        public void Test_Separate_First_Half_Array()
        {
            var inputSample = new[]
             {
                new Complex(0, 0),
                new Complex(1, 0),
                new Complex(2, 0),
                new Complex(3, 0),
                new Complex(4, 0),
                new Complex(5, 0),
                new Complex(6, 0),
                new Complex(7, 0)
            };

            FourierTransforms.Separate(inputSample, 0, 4);

            var expectedResult = new[]
            {
                new Complex(0, 0),
                new Complex(2, 0),
                new Complex(1, 0),
                new Complex(3, 0),
                new Complex(4, 0),
                new Complex(5, 0),
                new Complex(6, 0),
                new Complex(7, 0)
            };

            Assert.IsTrue(inputSample.SequenceEqual(expectedResult));
        }


        [Test]
        public void Test_Separate_Second_Half_Array()
        {
            var inputSample = new[]
            {
                new Complex(0, 0),
                new Complex(1, 0),
                new Complex(2, 0),
                new Complex(3, 0),
                new Complex(4, 0),
                new Complex(5, 0),
                new Complex(6, 0),
                new Complex(7, 0)
            };

            FourierTransforms.Separate(inputSample, 4, 4);

            var expectedResult = new[]
            {
                new Complex(0, 0),
                new Complex(1, 0),
                new Complex(2, 0),
                new Complex(3, 0),
                new Complex(4, 0),
                new Complex(6, 0),
                new Complex(5, 0),
                new Complex(7, 0)
            };

            Assert.IsTrue(inputSample.SequenceEqual(expectedResult));
        }

        [Test]
        public void Test_FastFourierTransform_InputSamplesNotPowerOfTwo()
        {
            var inputSample = new Complex[8];

            Assert.Throws<ArgumentException>(
                () => FourierTransforms.FastFourierTransform(inputSample, 0, 10),
                "the number of inputSamples must be power of 2");
        }

        [Test]
        public void Test_FastFourierTransform_SeparateSizeIsSmallerThan2()
        {
            var inputSample = new[] { new Complex(0, 0) };
            FourierTransforms.FastFourierTransform(inputSample, 0, 1);
            Assert.IsTrue(inputSample.SequenceEqual(new Complex[] { new Complex(0, 0) }));
        }


        [Test]
        public void Test_FastFourierTransform_BasicInput()
        {
            var inputSample = new[]
            {
                new Complex(1, 0),
                new Complex(1, 0),
                new Complex(1, 0),
                new Complex(1, 0),
                new Complex(1, 0),
                new Complex(1, 0),
                new Complex(1, 0),
                new Complex(1, 0)
            };
            FourierTransforms.FastFourierTransform(inputSample, 0, inputSample.Length);

            var expectedResult = new[]
            {
                new Complex(8, 0),
                new Complex(0, 0),
                new Complex(0, 0),
                new Complex(0, 0),
                new Complex(0, 0),
                new Complex(0, 0),
                new Complex(0, 0),
                new Complex(0, 0)
            };

            Assert.IsTrue(inputSample.SequenceEqual(expectedResult));
        }


        [Test]
        public void Test_FastFourierTransform_ComplexInput()
        {
            var inputSample = new[]
            {
                new Complex(1, 0),
                new Complex(2, 0),
                new Complex(3, 0),
                new Complex(4, 0),

            };
            FourierTransforms.FastFourierTransform(inputSample, 0, inputSample.Length);

            var expectedResult = new[]
            {
                new Complex(10, 0),
                new Complex(-2, 2),
                new Complex(-2, 0),
                new Complex(-1.9999999999999998,-2)
            };

            Assert.IsTrue(inputSample.SequenceEqual(expectedResult));
        }


        [Test]
        public void Test_DiscreteFourierTransform()
        {
            var inputSample = new[]
            {
                new Complex(1, 0),
                new Complex(2, 0),
                new Complex(3, 0),
                new Complex(4, 0)

            };
            var result = FourierTransforms.DiscreteFourierTransform(inputSample);

            var expectedResult = new[]
            {
                new Complex(10, 0),
                new Complex(-2, 2),
                new Complex(-2, 0),
                new Complex(-2,-2)
            };

            for (int i = 0; i < result.Length; i++)
            {
                var r = result[i];
                var er = expectedResult[i];

                var difference = 1E-10;
                Assert.IsTrue(Math.Abs(r.Real - er.Real) <= difference);
                Assert.IsTrue(Math.Abs(r.Imaginary - er.Imaginary) <= difference);
            }
        }




        [Test]
        public void Test_FastFourierTransform_Abraham()
        {
            var inputSample = new[]
            {
                new Complex(1, 0),
                new Complex(2, 0),
                new Complex(3, 0),
                new Complex(4, 0),

            };
            FourierTransforms.FastFourierTransform(inputSample, 0, inputSample.Length);

            var expectedResult = new[]
            {
                new Complex(10, 0),
                new Complex(-2, 2),
                new Complex(-2, 0),
                new Complex(-1.9999999999999998,-2)
            };

            Assert.IsTrue(inputSample.SequenceEqual(expectedResult));
        }

        [Test]
        public void Test_FindNextPowerOfTwo()
        {
            var res = Utils.FindNextPowerOfTwo(799);
            Assert.AreEqual(res, 1024);
        }


        [Test]
        public void Test_FFT_VS_DFT()
        {
            var csvFilePath = Path.Combine(
                TestContext.CurrentContext.TestDirectory, 
                @"TestData\Wave000.csv");

            var inputSamplesList = CSVUtils.ReadCSVFile<CSVWaveRecord>(csvFilePath);
            var dftInputArray = inputSamplesList.Select(x => new Complex((double)x.Volts, 0)).ToArray();

            Utils.PadWithZeros(inputSamplesList);
            var fftInputArray = inputSamplesList.Select(x => new Complex((double)x.Volts, 0)).ToArray();

            FourierTransforms.DiscreteFourierTransform(dftInputArray);
            FourierTransforms.FastFourierTransform(fftInputArray,0,fftInputArray.Length);
            
        }

        [Test]
        public void Test_WaveFormIntervals()
        {
            var csvFilePath = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                @"TestData\Wave000.csv");

            var inputSamplesList = CSVUtils.ReadCSVFile<CSVWaveRecord>(csvFilePath);
            var seconds = inputSamplesList.Select(x => x.Seconds).ToArray();

            var diffList = new LinkedList<decimal>();
            for(int i = 1; i < seconds.Length; i++)
            {
                var diff = seconds[i] - seconds[i - 1];
                diffList.AddLast(diff);
            }


            
            Assert.IsTrue(diffList.All(x => x >= 6E-8m && x <= 6.1E-8m));
        }

        }
}
