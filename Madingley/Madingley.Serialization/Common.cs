using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

namespace Madingley.Serialization.Common
{
    public static class Writer
    {
        public static void WriteBoolean(JsonWriter jsonWriter, bool value)
        {
            jsonWriter.WriteValue(value);
        }

        public static void WriteDouble(JsonWriter jsonWriter, double value)
        {
            jsonWriter.WriteValue(value);
        }

        public static void WriteInt(JsonWriter jsonWriter, int value)
        {
            jsonWriter.WriteValue(value);
        }

        public static void WriteLong(JsonWriter jsonWriter, long value)
        {
            jsonWriter.WriteValue(value.ToString());
        }

        public static void WriteString(JsonWriter jsonWriter, string value)
        {
            jsonWriter.WriteValue(value);
        }

        public static void WriteArray<T>(JsonWriter jsonWriter, IEnumerable<T> value, Action<JsonWriter, T> writeValue)
        {
            jsonWriter.WriteStartArray();
            value.ToList().ForEach(v => writeValue(jsonWriter, v));
            jsonWriter.WriteEndArray();
        }

        public static void WriteKeyValuePairs<T>(JsonWriter jsonWriter, IEnumerable<KeyValuePair<string, T>> value, Action<JsonWriter, string, T> writeValue)
        {
            jsonWriter.WriteStartObject();
            value.ToList().ForEach(kvp => writeValue(jsonWriter, kvp.Key, kvp.Value));
            jsonWriter.WriteEndObject();
        }

        public static void PropertyBoolean(JsonWriter jsonWriter, string name, bool value)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteValue(value);
        }

        public static void PropertyDouble(JsonWriter jsonWriter, string name, double value)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteValue(value);
        }

        public static void PropertyInt(JsonWriter jsonWriter, string name, int value)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteValue(value);
        }

        public static void PropertyLong(JsonWriter jsonWriter, string name, long value)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteValue(value.ToString());
        }

        public static void PropertyString(JsonWriter jsonWriter, string name, string value)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteValue(value);
        }

        public static void PropertyInlineArray<T>(JsonWriter jsonWriter, string name, IEnumerable<T> value, Action<JsonWriter, T> writeValue)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.Formatting = Formatting.None;
            jsonWriter.WriteStartArray();
            value.ToList().ForEach(v => writeValue(jsonWriter, v));
            jsonWriter.WriteEndArray();
            jsonWriter.Formatting = Formatting.Indented;
        }

        public static void PropertyArray<T>(JsonWriter jsonWriter, string name, IEnumerable<T> value, Action<JsonWriter, T> writeValue)
        {
            jsonWriter.WritePropertyName(name);
            WriteArray(jsonWriter, value, writeValue);
        }
        public static void PropertyInlineKeyValuePairs<T>(JsonWriter jsonWriter, string name, IEnumerable<KeyValuePair<string, T>> value, Action<JsonWriter, string, T> writeValue)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.Formatting = Formatting.None;
            jsonWriter.WriteStartObject();
            value.ToList().ForEach(kvp => writeValue(jsonWriter, kvp.Key, kvp.Value));
            jsonWriter.WriteEndObject();
            jsonWriter.Formatting = Formatting.Indented;
        }

        public static void PropertyKeyValuePairs<T>(JsonWriter jsonWriter, string name, IEnumerable<KeyValuePair<string, T>> value, Action<JsonWriter, string, T> writeValue)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteStartObject();
            value.ToList().ForEach(kvp => writeValue(jsonWriter, kvp.Key, kvp.Value));
            jsonWriter.WriteEndObject();
        }
    }

    public static class Reader
    {
        public static bool ReadBoolean(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.Boolean);
            Debug.Assert(reader.ValueType == typeof(bool));
            return Convert.ToBoolean(reader.Value);
        }

        public static double ReadDouble(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.Float);
            Debug.Assert(reader.ValueType == typeof(double));
            return Convert.ToDouble(reader.Value);
        }

        public static int ReadInt(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.Integer);
            Debug.Assert(reader.ValueType == typeof(Int32) || reader.ValueType == typeof(Int64));
            return Convert.ToInt32(reader.Value);
        }

        public static long ReadLong(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.String);
            Debug.Assert(reader.ValueType == typeof(string));
            return Convert.ToInt64(reader.Value);
        }

        public static string ReadString(JsonTextReader reader)
        {
            Debug.Assert(reader.TokenType == JsonToken.String);
            Debug.Assert(reader.ValueType == typeof(string));
            return Convert.ToString(reader.Value);
        }

        public static IEnumerable<T> ReadArray<T>(JsonTextReader reader, Func<JsonTextReader, T> readValue)
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

        public static IDictionary<string, T> ReadKeyValuePairs<T>(JsonTextReader reader, Func<JsonTextReader, T> readValue)
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
