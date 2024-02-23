using System;

public class SequenceData
{
    public SequenceData()
    {
        Dictionary<string, List<string>> sequences = new Dictionary<string, List<string>>();


        sequences.Add("S1", new List<string>(new string[] { 0.0, 1.0, 2.0, 3.0, 4.0, 2.0, 5.0, 6.0, 7.0 }));
        sequences.Add("S2", new List<string>(new string[] { 8.0, 1.0, 2.0, 9.0, 10.0, 7.0, 11.00 }));
        sequences.Add("S3", new List<string>(new string[] { 12.0, 13.0, 14.0, 15.0, 16.0, 6.0, 9.00 }));
        sequences.Add("S4", new List<string>(new string[] { 3.0, 6.0, 9.0, 12.0, 15.0, 18.0, 21.00 }));
        sequences.Add("S5", new List<string>(new string[] { 3.0, 6.0, 9.0, 12.0, 15.0, 18.0, 21.00, 22.00, 23.00 }));

        //
        // Prototype for building the prediction engine.
        MultiSequenceLearning experiment = new MultiSequenceLearning();
        var predictor = experiment.Run(sequences);

        //
        // These list are used to see how the prediction works.
        // Predictor is traversing the list element by element. 
        // By providing more elements to the prediction, the predictor delivers more precise result.
        var list1 = new string[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 };
        var list2 = new string[] { 2.0, 3.0, 4.0 };
        var list3 = new string[] { 8.0, 1.0, 2.0 };

        predictor.Reset();
        PredictNextElement(predictor, list1);

        predictor.Reset();
        PredictNextElement(predictor, list2);

        predictor.Reset();
        PredictNextElement(predictor, list3);



    }
}