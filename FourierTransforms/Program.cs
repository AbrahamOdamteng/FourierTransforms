using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using AbrahamOdamteng.CSV;
using System.Threading;
using System.Diagnostics;

namespace AbrahamOdamteng
{
    enum Algorithm { DFT, FFT, Both}


    class Program
    {

        static bool ZeroPadDFT;

        static void Main(string[] args)
        {
            //========================================================================
            //Control Options!!!

            var algorithm = Algorithm.DFT;//USE THIS TO CONTROL WHICH ALGORITHM IS USED.
            ZeroPadDFT = false; // Should the DFT be padded with zeros up to the nearest power of two.

            var inputDirectoryPath  = @".\InputFiles\";

            var filenamePattern = "wave*.csv";

            var outputDFTFolderPath = @".\OutputFiles\DFTs\";
            var outputFFTFolderPath = @".\OutputFiles\FFTs\";

            if (ZeroPadDFT)
            {
                outputDFTFolderPath = @".\OutputFiles\DFTZP\";
            }

            Directory.CreateDirectory(outputDFTFolderPath);
            Directory.CreateDirectory(outputFFTFolderPath);
            Directory.CreateDirectory(inputDirectoryPath);
            GenerateWaveForms(inputDirectoryPath);

            var answerDFTPath = Path.Combine(outputDFTFolderPath, "Answer_DFT.CSV");
            var answerFFTPath = Path.Combine(outputFFTFolderPath, "Answer_FFT.CSV");

            if (ZeroPadDFT)
            {
                answerDFTPath = Path.Combine(outputDFTFolderPath, "Answer_DFTZP.CSV");
            }
            //========================================================================

            var prog = new Program();

            switch (algorithm)
            {
                case Algorithm.DFT:
                    prog.ParallelProcessWaveForms(inputDirectoryPath, filenamePattern, outputDFTFolderPath, answerDFTPath, DFT);
                    break;
                case Algorithm.FFT:
                    prog.ParallelProcessWaveForms(inputDirectoryPath, filenamePattern, outputFFTFolderPath, answerFFTPath, FFT);
                    break;
                case Algorithm.Both:
                    prog.ParallelProcessWaveForms(inputDirectoryPath, filenamePattern, outputFFTFolderPath, answerFFTPath, FFT);
                    prog.ParallelProcessWaveForms(inputDirectoryPath, filenamePattern, outputDFTFolderPath, answerDFTPath, DFT);
                    break;
            }

            Console.WriteLine("Finished press any key to close");
            Console.Read();
        }

        static void GenerateWaveForms(string workingDirectory)
        {
            var item = Environment.CurrentDirectory;
            var fileName = Path.Combine(Environment.CurrentDirectory, @"WaveFormGenerator\WaveGen.exe");
            var info = new ProcessStartInfo()
            {
                WorkingDirectory = workingDirectory,
                FileName = fileName
            };
            var proc =  Process.Start(info);
            proc.WaitForExit();
        }


        void ParallelProcessWaveForms(string inputDirectoryPath,string filenamePattern,  string outputDirectoryPath, string answerPath, Func<string, string, LinkedList<FrequencyMagnitude>> algorithm)
        {
            var cumulativeStopWatch = new System.Diagnostics.Stopwatch();
            cumulativeStopWatch.Start();

            var lockObject = new object();
            var result = new LinkedList<FrequencyFileName>();
            var files = Directory.GetFiles(inputDirectoryPath, filenamePattern);
            var filesRemaining = files.Length;
            Parallel.ForEach(files, (filePath) =>
            {

                Console.WriteLine("Processing File: " + filePath);
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                //------------------------------------------------

                var frequencyDomain = algorithm(filePath, outputDirectoryPath);

                //------------------------------------------------
                var highestMagnitude = frequencyDomain.Aggregate(
                    (max, current) => (max == null || max.Magnitude < current.Magnitude) ? current : max);

                var fileName = Path.GetFileName(filePath);

                var ff = new FrequencyFileName()
                {
                    FileName = Path.GetFileName(filePath),
                    Frequency = highestMagnitude.Frequency
                };

                lock (lockObject)
                {
                    result.AddLast(ff);

                    sw.Stop();
                    Console.WriteLine(string.Format("Processed File: {0} , Duration: {1}",fileName, sw.Elapsed));

                    filesRemaining--;
                    Console.WriteLine("Files Remaining: " + filesRemaining);
                    Console.WriteLine("Cumulative Duration: " + cumulativeStopWatch.Elapsed);
                }
            });

            cumulativeStopWatch.Stop();
            Console.WriteLine("Total Duration: " + cumulativeStopWatch.Elapsed);

            CSVUtils.WriteCSVFile<FrequencyFileName>(answerPath, result.OrderBy(x => x.Frequency));
        }
        

        static LinkedList<FrequencyMagnitude> FFT(string filePath, string ouputDirectoryPath)
        {
            var inputSamplesList = CSVUtils.ReadCSVFile<CSVWaveRecord>(filePath);
            Utils.PadWithZeros(inputSamplesList);
            var frequencyResolution = Utils.CalculateFrequencyResolution(inputSamplesList);
            var inputSamplesArray = inputSamplesList.Select(x => new Complex((double)x.Volts, 0)).ToArray();

            FourierTransforms.FastFourierTransform(inputSamplesArray, 0, inputSamplesArray.Length);

            var size = inputSamplesArray.Length / 2;
            var leftHalfArray = new Complex[size];
            Array.Copy(inputSamplesArray, leftHalfArray, size);
            inputSamplesArray = null;

            var intermediateFilePath = Path.Combine(ouputDirectoryPath, 
                Path.GetFileName(filePath).Replace("Wave", "FFT"));

            Utils.printOutput(leftHalfArray, frequencyResolution, intermediateFilePath);
            return Utils.GetFrequencyDomain(leftHalfArray, frequencyResolution);
        }


        static LinkedList<FrequencyMagnitude> DFT(string filePath, string ouputDirectoryPath)
        {
            var inputSamplesList = CSVUtils.ReadCSVFile<CSVWaveRecord>(filePath);
            if (ZeroPadDFT)
            {
                Utils.PadWithZeros(inputSamplesList);
            }
            var frequencyResolution = Utils.CalculateFrequencyResolution(inputSamplesList);
            var inputSamplesArray = inputSamplesList.Select(x => new Complex((double)x.Volts, 0)).ToArray();

            var dftValues = FourierTransforms.DiscreteFourierTransform(inputSamplesArray);

            inputSamplesArray = null;
            var size = dftValues.Length / 2;
            var leftHalfArray = new Complex[size];
            Array.Copy(dftValues, leftHalfArray, size);
            dftValues = null;

            var intermediateFile = Path.Combine(ouputDirectoryPath, 
                Path.GetFileName(filePath).Replace("Wave", "DFT"));
            Utils.printOutput(leftHalfArray, frequencyResolution, intermediateFile);

            return Utils.GetFrequencyDomain(leftHalfArray, frequencyResolution);
        }
    }
}
