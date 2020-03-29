using System;

namespace BrickBundle.ML
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get singleton ObjecDetection object
            //ObjectDetection.Get() will always return the same object
            var objectDetection = ObjectDetection.GetInstance();

            // Predict image
            var results = objectDetection.GetPredictionResults(args[0]);

            // Show scores results
            foreach (var result in results)
            {
                Console.WriteLine($"class:{result.Class}, score:{result.Score}");
            }
        }
    }
}
