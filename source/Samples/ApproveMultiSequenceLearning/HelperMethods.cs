using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using MultiSequenceLearning;
using Newtonsoft.Json;

namespace MultiSequenceLearning
{
    public class HelperMethods
    {
        // Constants for default settings
        private const int DefaultRandomSeed = 42;
        private const double MaxScalarValue = 20.0;
        private const int DefaultCellsPerColumn = 25;
        private const double DefaultGlobalInhibitionDensity = 0.02;
        private const double DefaultPotentialRadiusFactor = 0.15;
        private const double DefaultMaxSynapsesPerSegmentFactor = 0.02;
        private const double DefaultMaxBoost = 10.0;
        private const int DefaultDutyCyclePeriod = 25;
        private const double DefaultMinPctOverlapDutyCycles = 0.75;
        private const int DefaultActivationThreshold = 15;
        private const double DefaultConnectedPermanence = 0.5;
        private const double DefaultPermanenceDecrement = 0.25;
        private const double DefaultPermanenceIncrement = 0.15;
        private const double DefaultPredictedSegmentDecrement = 0.1;

        /// <summary>
        /// HTM Config for creating Connections
        /// </summary>
        public static HtmConfig FetchHTMConfig(int inputBits, int numColumns)
        {
            return new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
            {
                Random = new ThreadSafeRandom(DefaultRandomSeed),
                CellsPerColumn = DefaultCellsPerColumn,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = DefaultGlobalInhibitionDensity * numColumns,
                PotentialRadius = (int)(DefaultPotentialRadiusFactor * inputBits),
                MaxBoost = DefaultMaxBoost,
                DutyCyclePeriod = DefaultDutyCyclePeriod,
                MinPctOverlapDutyCycles = DefaultMinPctOverlapDutyCycles,
                MaxSynapsesPerSegment = (int)(DefaultMaxSynapsesPerSegmentFactor * numColumns),
                ActivationThreshold = DefaultActivationThreshold,
                ConnectedPermanence = DefaultConnectedPermanence,
                PermanenceDecrement = DefaultPermanenceDecrement,
                PermanenceIncrement = DefaultPermanenceIncrement,
                PredictedSegmentDecrement = DefaultPredictedSegmentDecrement
            };
        }

        /// <summary>
        /// Get the encoder with settings
        /// </summary>
        public static EncoderBase GetEncoder(int inputBits)
        {
            var settings = new Dictionary<string, object>
            {
                { "W", 15 },
                { "N", inputBits },
                { "Radius", -1.0 },
                { "MinVal", 0.0 },
                { "Periodic", false },
                { "Name", "scalar" },
                { "ClipInput", false },
                { "MaxVal", MaxScalarValue }
            };

            return new ScalarEncoder(settings);
        }

        /// <summary>
        /// Reads dataset from the file
        /// </summary>
        public static List<Sequence> ReadDataset(string path)
        {
            Console.WriteLine("Reading Sequence...");
            try
            {
                string fileContent = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Sequence>>(fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read the dataset: {ex.Message}");
                return new List<Sequence>(); // Return an empty list in case of failure
            }
        }

        

        

        
    }
}