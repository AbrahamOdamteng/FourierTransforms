using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbrahamOdamteng.CSV;
using System.IO;

namespace AbrahamOdamteng
{
    public static class CSVUtils
    {
        public static LinkedList<T> ReadCSVFile<T>(string csvFilePath)
        {
            using (var f = File.OpenText(csvFilePath))
            {
                var reader = new CsvHelper.CsvReader(f);
                var result = reader.GetRecords<T>().ToArray();
                var ll = new LinkedList<T>(result);
                return ll;
            }
        }


        public static void WriteCSVFile<T>(string csvFilePath, IEnumerable<T> samples)
        {
            using (var f = File.CreateText(csvFilePath))
            {
                var writer = new CsvHelper.CsvWriter(f);
                writer.WriteRecords(samples);
            }

        }
    }
}
