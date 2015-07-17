using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

namespace Madingley.Serialization.Common
{
    public static class Writer
    {
        public static void WriteBoolean(JsonTextWriter jsonTextWriter, bool value)
        {
            jsonTextWriter.WriteValue(value);
        }

        public static void WriteDouble(JsonTextWriter jsonTextWriter, double value)
        {
            jsonTextWriter.WriteValue(value);
        }

        public static void WriteInt(JsonTextWriter jsonTextWriter, int value)
        {
            jsonTextWriter.WriteValue(value);
        }

        public static void WriteLong(JsonTextWriter jsonTextWriter, long value)
        {
            jsonTextWriter.WriteValue(value.ToString());
        }

        public static void WriteString(JsonTextWriter jsonTextWriter, string value)
        {
            jsonTextWriter.WriteValue(value);
        }

        public static void PropertyBoolean(JsonTextWriter jsonTextWriter, string name, bool value)
        {
            jsonTextWriter.WritePropertyName(name);
            jsonTextWriter.WriteValue(value);
        }

        public static void PropertyDouble(JsonTextWriter jsonTextWriter, string name, double value)
        {
            jsonTextWriter.WritePropertyName(name);
            jsonTextWriter.WriteValue(value);
        }

        public static void PropertyInt(JsonTextWriter jsonTextWriter, string name, int value)
        {
            jsonTextWriter.WritePropertyName(name);
            jsonTextWriter.WriteValue(value);
        }

        public static void PropertyLong(JsonTextWriter jsonTextWriter, string name, long value)
        {
            jsonTextWriter.WritePropertyName(name);
            jsonTextWriter.WriteValue(value.ToString());
        }

        public static void PropertyString(JsonTextWriter jsonTextWriter, string name, string value)
        {
            jsonTextWriter.WritePropertyName(name);
            jsonTextWriter.WriteValue(value);
        }

        public static void PropertyInlineArray<T>(JsonTextWriter jsonTextWriter, string name, IEnumerable<T> value, Action<JsonTextWriter, T> writeValue)
        {
            jsonTextWriter.WritePropertyName(name);
            jsonTextWriter.Formatting = Formatting.None;
            jsonTextWriter.WriteStartArray();
            value.ToList().ForEach(v => writeValue(jsonTextWriter, v));
            jsonTextWriter.WriteEndArray();
            jsonTextWriter.Formatting = Formatting.Indented;
        }

        public static void PropertyArrayValue<T>(JsonTextWriter jsonTextWriter, IEnumerable<T> value, Action<JsonTextWriter, T> writeValue)
        {
            jsonTextWriter.WriteStartArray();
            value.ToList().ForEach(v => writeValue(jsonTextWriter, v));
            jsonTextWriter.WriteEndArray();
        }

        public static void PropertyArray<T>(JsonTextWriter jsonTextWriter, string name, IEnumerable<T> value, Action<JsonTextWriter, T> writeValue)
        {
            jsonTextWriter.WritePropertyName(name);
            PropertyArrayValue(jsonTextWriter, value, writeValue);
        }

        public static void KeyValuePairArrayValue<T>(JsonTextWriter jsonTextWriter, IEnumerable<KeyValuePair<string, T>> value, Action<JsonTextWriter, string, T> writeValue)
        {
            jsonTextWriter.WriteStartObject();
            value.ToList().ForEach(kvp => writeValue(jsonTextWriter, kvp.Key, kvp.Value));
            jsonTextWriter.WriteEndObject();
        }

        public static void KeyValuePairArray<T>(JsonTextWriter jsonTextWriter, string name, IEnumerable<KeyValuePair<string, T>> value, Action<JsonTextWriter, string, T> writeValue)
        {
            jsonTextWriter.WritePropertyName(name);
            KeyValuePairArrayValue(jsonTextWriter, value, writeValue);
        }
    }

    public static class Reader
    {
        public static bool JsonReadBool(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.Boolean);
            Debug.Assert(reader.ValueType == typeof(bool));
            return Convert.ToBoolean(reader.Value);
        }

        public static double JsonReadDouble(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.Float);
            Debug.Assert(reader.ValueType == typeof(double));
            return Convert.ToDouble(reader.Value);
        }

        public static int JsonReadInt(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.Integer);
            Debug.Assert(reader.ValueType == typeof(Int32) || reader.ValueType == typeof(Int64));
            return Convert.ToInt32(reader.Value);
        }

        public static long JsonReadLong(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.String);
            Debug.Assert(reader.ValueType == typeof(string));
            return Convert.ToInt64(reader.Value);
        }

        public static string JsonReadString(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.String);
            Debug.Assert(reader.ValueType == typeof(string));
            return Convert.ToString(reader.Value);
        }

        public static IEnumerable<T> JsonReadArray<T>(JsonTextReader reader, Func<JsonTextReader, T> readValue)
        {
            var ret = new List<T>();

            Debug.Assert(reader.TokenType == JsonToken.StartArray);

            while (reader.Read() &&
                reader.TokenType != JsonToken.EndArray)
            {
                ret.Add(readValue(reader));
            }

            return ret;
        }

        public static IDictionary<string, T> JsonReadKVPs<T>(JsonTextReader reader, Func<JsonTextReader, T> readValue)
        {
            var ret = new Dictionary<string, T>();

            Debug.Assert(reader.TokenType == JsonToken.StartObject);

            while (reader.Read() &&
                reader.TokenType != JsonToken.EndObject)
            {
                Debug.Assert(reader.TokenType == JsonToken.PropertyName);
                Debug.Assert(reader.ValueType == typeof(string));

                var key = Convert.ToString(reader.Value);
                reader.Read();
                var value = readValue(reader);

                ret.Add(key, value);
            }

            return ret;
        }
    }
}
