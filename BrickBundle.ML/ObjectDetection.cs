/*******************************************************************************
Based on examples from Tensorflow.NET
https://github.com/SciSharp/TensorFlow.NET
*******************************************************************************/
using BrickBundle.ML.Utility;
using NumSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tensorflow;
using static Tensorflow.Binding;

namespace BrickBundle.ML
{
    public class ObjectDetection
    {
        #region Constants
        private const float MIN_SCORE = 0.5f;
        private static readonly string modelDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "model");
        private static readonly string imgDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "images");
        private const string pbFile = "frozen_inference_graph.pb";
        private const string labelFile = "labelmap.pbtxt";
        #endregion

        #region Fields
        /// <summary>
        /// The Object Detection Model.
        /// </summary>
        private readonly Graph graph;
        /// <summary>
        /// Map the binary result from ML model to human friendly class name.
        /// </summary>
        private Dictionary<int, string> labelMap = new Dictionary<int, string>();
        #endregion

        #region Static Fields
        private static ObjectDetection instance = null;
        #endregion

        #region Constructors
        private ObjectDetection(Graph graph, Dictionary<int, string> labelMap)
        {
            this.graph = graph;
            this.labelMap = labelMap;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets results with scores of at least <see cref="MIN_SCORE"/>.
        /// </summary>
        /// <returns>(class, score)</returns>
        public List<ObjectDetectionResult> GetPredictionResults(string jpegFile)
        {
            return GetResults(Predict(ReadTensorFromImageFile(jpegFile)));
        }

        /// <summary>
        /// Gets results with scores of at least <see cref="MIN_SCORE"/>.
        /// </summary>
        /// <returns>(class, score)</returns>
        public List<ObjectDetectionResult> GetPredictionResults(Stream jpegjpegStream)
        {
            return GetResults(Predict(ReadTensorFromFileStream(jpegjpegStream)));
        }

        /// <summary>
        /// Get Raw prediction results.
        /// </summary>
        /// <returns>
        /// <param name="sess">Tensorflow session</param>
        /// <param name="imgArr"></param>
        /// rawResult[1]: Bounding boxes
        /// rawResult[2]: Prediction Scores
        /// rawResult[3]: Prediction classes
        /// </returns>
        private NDArray[] Predict(Session sess, NDArray imgArr)
        {
            // Init the graph
            var graph = tf.get_default_graph();
            Tensor tensorNum = graph.OperationByName("num_detections");
            Tensor tensorBoxes = graph.OperationByName("detection_boxes");
            Tensor tensorScores = graph.OperationByName("detection_scores");
            Tensor tensorClasses = graph.OperationByName("detection_classes");
            Tensor imgTensor = graph.OperationByName("image_tensor");
            Tensor[] outTensorArr = new Tensor[] { tensorNum, tensorBoxes, tensorScores, tensorClasses };
            // Predict (This is where the magic happens)
            var results = sess.run(outTensorArr, new FeedItem(imgTensor, imgArr));
            return results;
        }

        /// <summary>
        /// Get Raw prediction results.
        /// </summary>
        /// <returns>
        /// rawResult[1]: Bounding boxes
        /// rawResult[2]: Prediction Scores
        /// rawResult[3]: Prediction classes
        /// </returns>
        private NDArray[] Predict(NDArray imgArr)
        {
            using (var sess = tf.Session(graph))
            {
                return Predict(sess, imgArr);
            }
        }

        /// <summary>
        /// Gets results with scores of at least <see cref="MIN_SCORE"/>.
        /// </summary>
        /// <param name="results">Raw prediciotn results</param>
        /// <returns>(class, score)</returns>
        private List<ObjectDetectionResult> GetResults(NDArray[] results)
        {
            var result = new List<ObjectDetectionResult>();
            var scores = results[2].AsIterator<float>();
            var classes = results[3].AsIterator<int>();
            for (int i = 0; i < scores.size; i++)
            {
                float score = scores.MoveNext();
                int myclass = classes.MoveNext();
                if (score >= MIN_SCORE)
                {
                    result.Add(new ObjectDetectionResult(labelMap[myclass], score));
                }
            }
            return result;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Gets the current ObjectDetection if there is one, otherwise creates a new ObjectDetection instance.
        /// </summary>
        public static ObjectDetection GetInstance()
        {
            if (instance == null)
            {
                instance = CreateInstance();
            }
            return instance;
        }

        /// <summary>
        /// Creates a new ObjectDetection instance.
        /// </summary>
        public static ObjectDetection CreateInstance()
        {
            if (!Directory.Exists(imgDir))
            {
                Directory.CreateDirectory(imgDir);
            }
            return instance = new ObjectDetection(LoadGraph(), LoadLabelMap());
        }

        /// <summary>
        /// Loads LabelMap from pbtxt file.
        /// </summary>
        private static Dictionary<int, string> LoadLabelMap()
        {
            var labelMap = new Dictionary<int, string>();
            var labelItems = LabelParser.ParsePbtxtFile(Path.Combine(modelDir, labelFile));
            foreach (var labelItem in labelItems)
            {
                labelMap.Add(labelItem.ID, labelItem.Name);
            }
            return labelMap;
        }

        /// <summary>
        /// Loads the Machine Learning Model.
        /// </summary>
        private static Graph LoadGraph()
        {
            try
            {
                var graph = new Graph().as_default();
                graph.Import(Path.Combine(modelDir, pbFile));
                return graph;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Converts image into NDArray.
        /// </summary>
        /// <param name="fileName">image file name (JPEG type)</param>
        private static NDArray ReadTensorFromImageFile(string fileName)
        {
            var graph = tf.Graph().as_default();
            var file_reader = tf.read_file(fileName, "file_reader");
            var decodeJpeg = tf.image.decode_jpeg(file_reader, channels: 3, name: "DecodeJpeg");
            var casted = tf.cast(decodeJpeg, TF_DataType.TF_UINT8);
            var dims_expander = tf.expand_dims(casted, 0);
            using (var sess = tf.Session(graph))
            {
                return sess.run(dims_expander);
            }
        }

        /// <summary>
        /// Converts image into NDArray.
        /// </summary>
        /// <param name="stream">image stream (JPEG type)</param>
        private static NDArray ReadTensorFromFileStream(Stream stream)
        {
            var filename = Path.Combine(imgDir, $"{Guid.NewGuid()}.jpg");
            using (var filestream = File.Create(Path.Combine(imgDir, filename)))
            {
                stream.CopyTo(filestream);
            }
            var result = ReadTensorFromImageFile(Path.Combine(imgDir, filename));
            File.Delete(filename);
            return result;
        }
        #endregion
    }
}
