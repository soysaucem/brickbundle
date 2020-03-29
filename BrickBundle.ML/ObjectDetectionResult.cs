namespace BrickBundle.ML
{
    public class ObjectDetectionResult
    {
        public ObjectDetectionResult(string objectClass, float score)
        {
            Class = objectClass;
            Score = score;
        }
        public string Class { get; set; }
        public float Score { get; set; }
    }
}
