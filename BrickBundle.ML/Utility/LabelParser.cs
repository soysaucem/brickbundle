/*****************************************************************************
This file is based on ParserUtility Example from Tensorflow.NET
https://github.com/SciSharp/TensorFlow.NET
******************************************************************************/

using Newtonsoft.Json;

namespace BrickBundle.ML.Utility
{
    public class LabelMapItem
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public static class LabelParser
    {
        public static LabelMapItem[] ParsePbtxtFile(string filePath)
        {
            string line;
            string newText = "[";

            using (var reader = new System.IO.StreamReader(filePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string newline = string.Empty;

                    if (line.Contains("{"))
                    {
                        newline = line.Replace("item", "").Trim();
                        newText += newline;
                    }
                    else if (line.Contains("}"))
                    {
                        newText = newText.Remove(newText.Length - 1);
                        newText += line;
                        newText += ",";
                    }
                    else if (line.Contains("id: ") || line.Contains("name: "))
                    {
                        newline = line.Replace(":", "\":").Trim();
                        newline = "\"" + newline;
                        newline += ",";
                        newText += newline;
                    }
                    else
                    {
                        newText += " ";
                    }
                }

                newText = newText.Remove(newText.Length - 1);
                newText += "]";

                reader.Close();
            }
            return JsonConvert.DeserializeObject<LabelMapItem[]>(newText);
        }
    }
}
