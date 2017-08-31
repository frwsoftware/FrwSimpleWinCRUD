using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrwSoftware
{

    //http://stackoverflow.com/questions/10824165/converting-a-csv-file-to-json-using-c-sharp
    public static class JsonUtils
    {
        public static string[] SplitQuotedLine(string value)//todo quotes 
        {
            // Use the "quotes" bool if you need to keep/strip the quotes or something...
            //var s = new StringBuilder();
            var regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
            List<string> sl = new List<string>();
            foreach (Match m in regex.Matches(value))
            {
                //s.Append(m.Value);
                sl.Add(m.Value);
            }
            //return s.ToString();
            return sl.ToArray<string>();
        }

        /// <summary>
        /// Converts a CSV string to a Json array format.
        /// </summary>
        /// <remarks>First line in CSV must be a header with field name columns.</remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CsvToJson(string value)
        {
            // Get lines.
            if (value == null) return null;
            string[] lines = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) throw new InvalidDataException("Must have header line.");

            // Get headers.
            //string[] headers = lines.First().Split(new char[] { ',' });//   QuotedLine(new char[] { ',' }, false);
            string[] headers = SplitQuotedLine(lines.First()); 
            // Build JSON array.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            for (int i = 1; i < lines.Length; i++)
            {
                //string[] fields = lines[i].Split(new char[] { ',', ' ' });//  QuotedLine(new char[] { ',', ' ' }, true);//, '"', false);
                string[] fields0 = SplitQuotedLine(lines[i]);
                string[] fields = new string[fields0.Length];
                int j = 0;
                foreach (string f in fields0)
                {
                    string f1 = f;
                    if (f1 == null) f1 = "";
                    if (f1.StartsWith("\"") == false && f1.EndsWith("\"") == false) f1 = "\"" + f1 + "\"";
                    fields[j] = f1;
                    j++;
                }
                if (fields.Length != headers.Length) throw new InvalidDataException("Field count must match header count.");
                var jsonElements = headers.Zip(fields, (header, field) => string.Format("\"{0}\": {1}", header, field)).ToArray();
                string jsonObject = "{" + string.Format("{0}", string.Join(",", jsonElements)) + "}";
                if (i < lines.Length - 1)
                    jsonObject += ",";
                sb.AppendLine(jsonObject);
            }
            sb.AppendLine("]");
            return sb.ToString();
        }
    }
}
