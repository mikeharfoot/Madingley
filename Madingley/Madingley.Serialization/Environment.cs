using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Madingley.Serialization
{
    public static class Environment
    {
        public static void Serialize(Madingley.Common.Environment environment, TextWriter sw)
        {
            Action<Newtonsoft.Json.JsonTextWriter, string, bool> JsonAddPropertyBoolean = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, double> JsonAddPropertyDouble = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<double>> JsonAddPropertyDoubleArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.Formatting = Newtonsoft.Json.Formatting.None;
                sb.WriteStartArray();
                value.ToList().ForEach(v => sb.WriteValue(v));
                sb.WriteEndArray();
                sb.Formatting = Newtonsoft.Json.Formatting.Indented;
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<string>> JsonAddPropertyStringArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.Formatting = Newtonsoft.Json.Formatting.None;
                sb.WriteStartArray();
                value.ToList().ForEach(v => sb.WriteValue(v));
                sb.WriteEndArray();
                sb.Formatting = Newtonsoft.Json.Formatting.Indented;
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<KeyValuePair<string, string>>> JsonAddKVPStrings = (sb, name, kvps) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartObject();
                kvps.ToList().ForEach(
                    kvp =>
                    {
                        sb.WritePropertyName(kvp.Key);
                        sb.WriteValue(kvp.Value);
                    });
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Tuple<int, int>> JsonAddPropertyFocusCell = (sb, value) =>
            {
                sb.WriteStartArray();
                sb.WriteValue(value.Item1);
                sb.WriteValue(value.Item2);
                sb.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<Tuple<int, int>>> JsonAddPropertyFocusCells = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.Formatting = Newtonsoft.Json.Formatting.None;
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyFocusCell(sb, v));
                sb.WriteEndArray();
                sb.Formatting = Newtonsoft.Json.Formatting.Indented;
            };

            Action<Newtonsoft.Json.JsonTextWriter, IEnumerable<KeyValuePair<string, double[]>>> JsonAddPropertyCellEnvironment = (sb, value) =>
            {
                sb.WriteStartObject();
                value.ToList().ForEach(kvp => JsonAddPropertyDoubleArray(sb, kvp.Key, kvp.Value));
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<IDictionary<string, double[]>>> JsonAddPropertyCellEnvironmentArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyCellEnvironment(sb, v));
                sb.WriteEndArray();
            };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();

                JsonAddPropertyDouble(writer, "CellSize", environment.CellSize);
                JsonAddPropertyDouble(writer, "BottomLatitude", environment.BottomLatitude);
                JsonAddPropertyDouble(writer, "TopLatitude", environment.TopLatitude);
                JsonAddPropertyDouble(writer, "LeftmostLongitude", environment.LeftmostLongitude);
                JsonAddPropertyDouble(writer, "RightmostLongitude", environment.RightmostLongitude);
                JsonAddKVPStrings(writer, "Units", environment.Units);
                JsonAddPropertyBoolean(writer, "SpecificLocations", environment.SpecificLocations);
                JsonAddPropertyFocusCells(writer, "FocusCells", environment.FocusCells);
                JsonAddPropertyCellEnvironmentArray(writer, "CellEnvironment", environment.CellEnvironment);
                JsonAddPropertyStringArray(writer, "FileNames", environment.FileNames);

                writer.WriteEndObject();
            }
        }

        public static Madingley.Common.Environment Deserialize(TextReader sr)
        {
            Func<Newtonsoft.Json.JsonTextReader, bool> JsonReadBool = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.Boolean);
                Debug.Assert(reader.ValueType == typeof(bool));
                return Convert.ToBoolean(reader.Value);
            };

            Func<Newtonsoft.Json.JsonTextReader, double> JsonReadDouble = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.Float);
                Debug.Assert(reader.ValueType == typeof(double));
                return Convert.ToDouble(reader.Value);
            };

            Func<Newtonsoft.Json.JsonTextReader, int> JsonReadInt = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.Integer);
                Debug.Assert(reader.ValueType == typeof(Int32) || reader.ValueType == typeof(Int64));
                return Convert.ToInt32(reader.Value);
            };

            Func<Newtonsoft.Json.JsonTextReader, string> JsonReadString = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.String);
                Debug.Assert(reader.ValueType == typeof(string));
                return Convert.ToString(reader.Value);
            };

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<double>> JsonReadDoubleArray = (reader) =>
            {
                var ret = new List<double>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadDouble(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<string>> JsonReadStringArray = (reader) =>
            {
                var ret = new List<string>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadString(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IDictionary<string, string>> JsonReadKVPStrings = (reader) =>
            {
                var ret = new Dictionary<string, string>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var key = Convert.ToString(reader.Value);
                    reader.Read();
                    var value = JsonReadString(reader);

                    ret.Add(key, value);
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Tuple<int, int>> JsonReadFocusCell = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                reader.Read();
                var item1 = JsonReadInt(reader);

                reader.Read();
                var item2 = JsonReadInt(reader);

                reader.Read();
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.EndArray);

                return Tuple.Create(item1, item2);
            };

            Func<Newtonsoft.Json.JsonTextReader, IList<Tuple<int, int>>> JsonReadFocusCells = (reader) =>
            {
                var ret = new List<Tuple<int, int>>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadFocusCell(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IDictionary<string, double[]>> JsonReadCellEnvironment = (reader) =>
            {
                var ret = new Dictionary<string, double[]>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var key = Convert.ToString(reader.Value);
                    reader.Read();
                    var value = JsonReadDoubleArray(reader);

                    ret.Add(key, value.ToArray());
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IList<IDictionary<string, double[]>>> JsonReadCellEnvironmentArray = (reader) =>
                {
                    var ret = new List<IDictionary<string, double[]>>();

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                    while (reader.Read())
                    {
                        if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                        ret.Add(JsonReadCellEnvironment(reader));
                    }

                    return ret;
                };

            var environment = new Madingley.Common.Environment();

            using (var reader = new Newtonsoft.Json.JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.StartObject) continue;

                    if (reader.Value != null)
                    {
                        Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                        Debug.Assert(reader.ValueType == typeof(string));

                        var property = Convert.ToString(reader.Value);
                        reader.Read();

                        switch (property)
                        {
                            case "CellSize": environment.CellSize = JsonReadDouble(reader); break;
                            case "BottomLatitude": environment.BottomLatitude = JsonReadDouble(reader); break;
                            case "TopLatitude": environment.TopLatitude = JsonReadDouble(reader); break;
                            case "LeftmostLongitude": environment.LeftmostLongitude = JsonReadDouble(reader); break;
                            case "RightmostLongitude": environment.RightmostLongitude = JsonReadDouble(reader); break;
                            case "Units": environment.Units = JsonReadKVPStrings(reader); break;
                            case "SpecificLocations": environment.SpecificLocations = JsonReadBool(reader); break;
                            case "FocusCells": environment.FocusCells = JsonReadFocusCells(reader); break;
                            case "CellEnvironment": environment.CellEnvironment = JsonReadCellEnvironmentArray(reader); break;
                            case "FileNames": environment.FileNames = JsonReadStringArray(reader).ToList(); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }
                }
            }

            return environment;
        }
    }
}
