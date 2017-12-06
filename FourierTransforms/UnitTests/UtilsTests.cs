using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AbrahamOdamteng.CSV;

namespace AbrahamOdamteng.UnitTests
{
    [TestFixture]
    class UtilsTests
    {
        [Test]
        public void Test_PadWithZeros()
        {
            var ll = new LinkedList<CSVWaveRecord>();
            ll.AddLast(new CSVWaveRecord() { Seconds = 0m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 2m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 4m, Volts = 6 });

            Utils.PadWithZeros(ll);
            Assert.AreEqual(4, ll.Count);
            var last = ll.Last;

            Assert.AreEqual(6m, last.Value.Seconds);
            Assert.AreEqual(0m, last.Value.Volts);
        }


        [Test]
        public void Test_PadWithZeros_Error()
        {
            var ll = new LinkedList<CSVWaveRecord>();
            ll.AddLast(new CSVWaveRecord() { Seconds = 2m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 4m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 6m, Volts = 6 });

            Assert.Throws(typeof(Exception), () => Utils.PadWithZeros(ll));

        }


        [Test]
        public void Test_CalculateFrequencyResolution_ERROR()
        {
            var ll = new LinkedList<CSVWaveRecord>();
            ll.AddLast(new CSVWaveRecord() { Seconds = 2m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 4m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 6m, Volts = 6 });

            Assert.Throws(typeof(Exception), () => Utils.CalculateFrequencyResolution(ll));
        }


        [Test]
        public void Test_CalculateFrequencyResolution()
        {
            var ll = new LinkedList<CSVWaveRecord>();
            ll.AddLast(new CSVWaveRecord() { Seconds = 0m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 2m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 4m, Volts = 6 });
            ll.AddLast(new CSVWaveRecord() { Seconds = 6m, Volts = 6 });

            var result = Utils.CalculateFrequencyResolution(ll);
            Assert.AreEqual(0.1666666666666666666666666667m, result);
        }


        //[Test]
        //public void Test_CompareAnswers()
        //{
        //    var dftAnswers = new LinkedList<FrequencyFileName>();
        //    dftAnswers.AddLast(new FrequencyFileName { FileName = "FileNam1", Frequency = 21 });
        //    dftAnswers.AddLast(new FrequencyFileName { FileName = "FileNam2", Frequency = 7 });

        //    var fftAnswers = new LinkedList<FrequencyFileName>();
        //    fftAnswers.AddLast(new FrequencyFileName { FileName = "FileNam1", Frequency = 26 });
        //    fftAnswers.AddLast(new FrequencyFileName { FileName = "FileNam2", Frequency = 37 });

        //    Utils.CompareAnswers(dftAnswers, fftAnswers, @"C:\Users\abraham\Desktop\CompareAnswers.csv");
        //}


        //[Test]
        //public void Test_CompareAnswers_LoadFiles()
        //{
        //    var dftAnswers = CSVUtils.ReadCSVFile<FrequencyFileName>(@"C:\Users\abraham\Desktop\DFTZP\Answer_DFTZP.CSV");
        //    var fftAnswers = CSVUtils.ReadCSVFile<FrequencyFileName>(@"C:\Users\abraham\Desktop\FFTs\Answer_FFT.CSV");

        //    Utils.CompareAnswers(dftAnswers, fftAnswers, @"C:\Users\abraham\Desktop\CompareAnswers.csv");
        //}
    }
}
