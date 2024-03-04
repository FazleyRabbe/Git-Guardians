using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MultiSequenceLearning
{
    public class MultiSequenceLearning
    {
        public Predictor Run(List<Sequence> sequences)
        {
            Console.WriteLine($"Hello NeocortexApi! Experiment by Git_Gurdians {nameof(MultiSequenceLearning)}");
            int inputBits = 100;
            int numColumns = 1024;
            HtmConfig cfg = HelperMethods.FetchHTMConfig(inputBits, numColumns);
            EncoderBase encoder = HelperMethods.GetEncoder(inputBits);
            return RunExperiment(inputBits, cfg, encoder, sequences);
        }
        private Predictor RunExperiment(int inputBits, HtmConfig cfg, EncoderBase encoder, List<Sequence> sequences)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int maxMatchCnt = 0;
            var mem = new Connections(cfg);
            bool isInStableState = false;
            HtmClassifier<string, ComputeCycle> cls = new HtmClassifier<string, ComputeCycle>();
            var numUniqueInputs = GetNumberOfInputs(sequences);
            CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");
            TemporalMemory tm = new TemporalMemory();

            Console.WriteLine("------------ START ------------");

            HomeostaticPlasticityController hpc = new HomeostaticPlasticityController(mem, numUniqueInputs * 150, (isStable, numPatterns, actColAvg, seenInputs) =>
            {
                if (isStable)
                    
                    Debug.WriteLine($"STABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
                else
                    
                    Debug.WriteLine($"INSTABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");

                isInStableState = isStable;

            }, numOfCyclesToWaitOnChange: 50);


            SpatialPoolerMT sp = new SpatialPoolerMT(hpc);
            sp.Init(mem);
            tm.Init(mem);
            layer1.HtmModules.Add("encoder", encoder);
            layer1.HtmModules.Add("sp", sp);

            int[] prevActiveCols = new int[0];

            int cycle = 0;
            int matches = 0;

            var lastPredictedValues = new List<string>(new string[] { "0" });

            int maxCycles = 3500;

 
            //  New-born stage. In this stage, the SP is trained on the input patterns.
            

            for (int i = 0; i < maxCycles && isInStableState == false; i++)
            {
                matches = 0;

                cycle++;

                Debug.WriteLine($"-------------- Newborn SP Cycle {cycle} ---------------");
                Console.WriteLine($"-------------- Newborn SP Cycle {cycle} ---------------");

                foreach (var inputs in sequences)
                {
                    foreach (var input in inputs.data)
                    {
                        Debug.WriteLine($" -- {inputs.name} - {input} --");

                        var lyrOut = layer1.Compute(input, true);

                        if (isInStableState)
                            break;
                    }

                    if (isInStableState)
                        break;
                }
            }

            // Clear all learned patterns in the classifier.
            cls.ClearState();

            // We activate here the Temporal Memory algorithm.
            layer1.HtmModules.Add("tm", tm);

            //
            // Loop over all sequences.
           //Main loop

            Debug.WriteLine("------------ END ------------");

            return new Predictor(layer1, mem, cls);
        }


        /// <summary>
        /// Gets the number of inputs.
        /// </summary>
        /// <param name="sequences">Alle sequences.</param>
        /// <returns></returns>
        private int GetNumberOfInputs(List<Sequence> sequences)
        {
            int num = 0;

            foreach (var inputs in sequences)
            {
                //num += inputs.Value.Distinct().Count();
                num += inputs.data.Length;
            }

            return num;
        }
        /// <summary>
        /// Gets the key. The key is a combination of the previous inputs and the current input. 
        /// </summary>
        /// <param name="prevInputs"></param>
        /// <param name="input"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static string GetKey(List<string> prevInputs, double input, string sequence)
        {
            string key = String.Empty;

            for (int i = 0; i < prevInputs.Count; i++)
            {
                if (i > 0)
                    key += "-";
                key += (prevInputs[i]);
            }
            return $"{sequence}_{key}";
        }
    }
}
