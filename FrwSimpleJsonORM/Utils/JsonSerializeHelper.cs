/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
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
        
        public static string SaveToStringWithNullAndDefaultValues(object oToSave)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(oToSave, Formatting.Indented, settings);
        }

        public static T LoadFromString<T>(string str) where T : new()
        {
            //MissingMemberHandling By default Json.NET ignores JSON if there is no field or property for its value to be set to during deserialization.
            return (T)JsonConvert.DeserializeObject<T>(str);
        }
        public static object LoadFromString(string str, Type t)
        {
            //MissingMemberHandling By default Json.NET ignores JSON if there is no field or property for its value to be set to during deserialization.
            return JsonConvert.DeserializeObject(str, t);
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
    //http://stackoverflow.com/questions/40256323/json-net-serialize-datetime-minvalue-as-null
    //todo - this method gets error 
    public class DateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter jsonWriter, object inputObject, JsonSerializer jsonSerializer)
        {
            // Typecast the input object
            var dateTimeObject = inputObject as DateTime?;

            // Set the properties of the Json Writer
            jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

            if (dateTimeObject == DateTime.MinValue)
                jsonWriter.WriteValue((DateTime?)null);
            else
                jsonWriter.WriteValue(dateTimeObject);
        }
        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            DateTime? readValue = reader.ReadAsDateTime();

            return (readValue == null) ? DateTime.MinValue : readValue;

        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime?).IsAssignableFrom(objectType);
        }
    }
    public class MinDateTimeConverter : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return DateTime.MinValue;

            return (DateTime)reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTimeValue = (DateTime)value;
            if (dateTimeValue == DateTime.MinValue)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value);
        }
    }


}


