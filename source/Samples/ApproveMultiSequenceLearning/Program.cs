using NeoCortexApi;
using NeoCortexApi.Encoders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static MultiSequenceLearning.MultiSequenceLearning;

namespace MultiSequenceLearning
{
    class Program
    {
        private const string DatasetFolder = "dataset";
        private const string ReportFolder = "report";
        private const string DatasetFileName = "dataset_01.json";
        private const string TestsetFileName = "test_01.json";

        static void Main(string[] args)
        {
            //Reading Input Dataset
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            List<Sequence> sequences = ReadDataset(Path.Combine(basePath, DatasetFolder, DatasetFileName));
            //Reading Test  dataset
            List<Sequence> sequencesTest = ReadDataset(Path.Combine(basePath, DatasetFolder, TestsetFileName));
            List<Report> reports = RunMultiSequenceLearningExperiment(sequences, sequencesTest);
            WriteReport(reports, basePath);
        }

        private static List<Sequence> ReadDataset(string datasetPath)
        {
            try
            {
                Console.WriteLine($"Reading Dataset: {datasetPath}");
                return JsonConvert.DeserializeObject<List<Sequence>>(File.ReadAllText(datasetPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading dataset: {ex.Message}");
                return new List<Sequence>();
            }
        }

        

        private static string EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

       

        private static List<Report> RunMultiSequenceLearningExperiment(List<Sequence> sequences, List<Sequence> sequencesTest)
        {
            var reports = new List<Report>();
            var experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            foreach (Sequence item in sequencesTest)
            {
                var report = new Report
                {
                    SequenceName = item.name,
                    SequenceData = item.data
                };

                double accuracy = PredictNextElement(predictor, item.data, report);
                report.Accuracy = accuracy;
                reports.Add(report);

                Console.WriteLine($"Accuracy for {item.name} sequence: {accuracy}%");
            }

            return reports;
        }

       //Predict Element Function 
       //Predict Next Element 
       //Aquracy 
       // Report 2 making 
       // Update all if make changes 
        
    }

    
}