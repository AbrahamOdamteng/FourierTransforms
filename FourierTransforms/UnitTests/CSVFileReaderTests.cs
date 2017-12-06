using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;
using AbrahamOdamteng.CSV;

namespace AbrahamOdamteng.UnitTests
{
    [TestFixture]
    class CSVFileReaderTests
    {
        [Test]
        public void Test_CSV_ReadFile()
        {
            var csvFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\Wave000.csv");

            var result = CSVUtils.ReadCSVFile<CSVWaveRecord>(csvFilePath);
            Assert.AreEqual(5747, result.Count());

            var first = result.First();
            Assert.AreEqual(0m, first.Seconds);
            Assert.AreEqual(0m, first.Volts);

            var second = result.Skip(1).First();
            Assert.AreEqual(6.009475E-08m, second.Seconds);
            Assert.AreEqual(0.01737575m, second.Volts);
        }
    }
}
