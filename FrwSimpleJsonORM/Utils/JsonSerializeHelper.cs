using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Schema;
using System.IO.IsolatedStorage;
using Newtonsoft.Json.Converters;

namespace FrwSoftware
{
    // http://www.newtonsoft.com/json
    public class JsonSerializeHelper
    {
        public static void SaveToFile(object oToSave, string fileName)
        {
            File.WriteAllText(fileName, SaveToString(oToSave), Encoding.UTF8);
        }
        public static string SaveToString(object oToSave)
        {
            //short
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;//Json.NET will skip writing JSON properties if the.NET value is null when serializing and will skip setting fields / properties if the JSON property is null when deserializing
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            //By default Json.NET writes dates in the ISO 8601 format, e.g. "2012-03-21T05:40Z" (в реальности "2016-10-13T11:20:02.7187476+03:00")
            return JsonConvert.SerializeObject(oToSave, Formatting.Indented, settings);
        }

        public static T LoadFromString<T>(string str) where T : new()
        {
            //MissingMemberHandling By default Json.NET ignores JSON if there is no field or property for its value to be set to during deserialization.
            return (T)JsonConvert.DeserializeObject<T>(str);
        }
        public static T LoadFromFile<T>(string fileName) where T : new()
        {
            //MissingMemberHandling By default Json.NET ignores JSON if there is no field or property for its value to be set to during deserialization.
            if (File.Exists(fileName))
            {
                return LoadFromString<T>(File.ReadAllText(fileName, Encoding.UTF8));
            }
            else return new T();
        }

        public static dynamic LoadDynamic(string fileName)
        {
            if (File.Exists(fileName))
            {
                return JsonConvert.DeserializeObject(File.ReadAllText(fileName, Encoding.UTF8));
            }
            else return null;
        }

        public static dynamic LoadForType(string fileName, Type t)
        {
            if (File.Exists(fileName))
            {
                return JsonConvert.DeserializeObject(File.ReadAllText(fileName, Encoding.UTF8), t);
            }
            else return null;
        }

        public static dynamic DeserializeString(string str, Type t)
        {

            return JsonConvert.DeserializeObject(str, t);

        }

    }

}


