using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using AbrahamOdamteng.CSV;


namespace AbrahamOdamteng
{
    static class Utils
    {
        public static bool IsPowerOfTwo(int n)
        {
            return (n != 0) && (n & (n - 1)) == 0;
        }



        public static void printOutput(Complex[] dftArray, decimal frequencyResolution, string outputFilePath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Herz,Magnitude");
            for(int i = 0; i < dftArray.Length; i++)
            {
                sb.AppendLine(string.Format("{0},{1}", i * frequencyResolution, Complex.Abs(dftArray[i])));
            }

            File.WriteAllText(outputFilePath, sb.ToString());
        }


        public static int FindNextPowerOfTwo(int n)
        {
            var next = Math.Pow(2, Math.Ceiling(Math.Log(n) / Math.Log(2)));
            return (int)next;
        }



        public static LinkedList<FrequencyMagnitude> GetFrequencyDomain(Complex[] ft, decimal frequencyResolution)
        {

            var frequencyDomain = new LinkedList<FrequencyMagnitude>();
            
            for(int i = 0; i < ft.Length; i++)
            {
                var val = new FrequencyMagnitude()
                {
                    Magnitude = Complex.Abs(ft[i]),
                    Frequency = i * frequencyResolution
                };
                frequencyDomain.AddLast(val);

                
            }

            return frequencyDomain;
        }


        public static void PadWithZeros(LinkedList<CSVWaveRecord> inputSamples)
        {
            if (inputSamples.Count == 0 || IsPowerOfTwo(inputSamples.Count)) return;

            var currentSize = inputSamples.Count;
            var next = FindNextPowerOfTwo(currentSize);
            var paddSize = next - currentSize;

            if (inputSamples.First.Value.Seconds != 0)
            {
                throw new Exception("The first time interval of any waveform must be zero");

            }

            var interval = inputSamples.Skip(1).First().Seconds;

            var latestTime = inputSamples.Last.Value.Seconds;

            for (int i = 0; i < paddSize; i++)
            {
                var val = new CSVWaveRecord()
                {
                    Seconds = latestTime + interval,
                    Volts = 0m
                };
                latestTime = val.Seconds;
                inputSamples.AddLast(val);
            }

            if (IsPowerOfTwo(inputSamples.Count)) return ;
            throw new Exception("Padding Failed");
        }


        public static decimal CalculateFrequencyResolution(LinkedList<CSVWaveRecord> inputSamples)
        {

            var firstTimeStamp = inputSamples.First.Value.Seconds;
            if (firstTimeStamp != 0m) throw new Exception("First time stamp must be zero");

            var lastTimeStamp = inputSamples.Last.Value.Seconds;
            var duration = lastTimeStamp;

            var count = (decimal)inputSamples.Count;
            decimal sampleRate = count / duration;
            var frequencyResolution = sampleRate / count;

            return frequencyResolution;
        }


        public static void CompareAnswers(LinkedList<FrequencyFileName> dftAnswer, LinkedList<FrequencyFileName> fftAnswer, string outputPath)
        {
            var res = new LinkedList<CSVAnswerErrorRecord>();
            foreach(var filename in dftAnswer.Select(x => x.FileName))
            {
                var dft = dftAnswer.Single(x => x.FileName == filename);
                var fft = fftAnswer.Single(x => x.FileName == filename);
                var errorRecord = new CSVAnswerErrorRecord();

                errorRecord.FileName = filename;
                errorRecord.DFTFrequency = dft.Frequency;
                errorRecord.FFTFrequency = fft.Frequency;

                if(Math.Max(dft.Frequency, fft.Frequency) == 0)
                {
                    errorRecord.Error = 0;
                }
                else
                {
                    errorRecord.Error = (Math.Abs(dft.Frequency - fft.Frequency) / Math.Max(dft.Frequency, fft.Frequency)) * 100;
                }
                
                res.AddLast(errorRecord);
            }

            CSVUtils.WriteCSVFile<CSVAnswerErrorRecord>(outputPath, res.OrderBy(x => x.Error));

        }

    }
}
