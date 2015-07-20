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
            Action<Newtonsoft.Json.JsonWriter, Tuple<int, int>> JsonAddPropertyFocusCell = (jsonWriter, value) =>
            {
                jsonWriter.WriteStartArray();
                Common.Writer.WriteInt(jsonWriter, value.Item1);
                Common.Writer.WriteInt(jsonWriter, value.Item2);
                jsonWriter.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonWriter, IEnumerable<KeyValuePair<string, double[]>>> JsonAddPropertyCellEnvironment = (jsonWriter, value) =>
            {
                Common.Writer.WriteKeyValuePairs(jsonWriter, value, (JsonWriter, key, val) => Common.Writer.PropertyInlineArray(JsonWriter, key, val, Common.Writer.WriteDouble));
            };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();
                Common.Writer.PropertyDouble(writer, "CellSize", environment.CellSize);
                Common.Writer.PropertyDouble(writer, "BottomLatitude", environment.BottomLatitude);
                Common.Writer.PropertyDouble(writer, "TopLatitude", environment.TopLatitude);
                Common.Writer.PropertyDouble(writer, "LeftmostLongitude", environment.LeftmostLongitude);
                Common.Writer.PropertyDouble(writer, "RightmostLongitude", environment.RightmostLongitude);
                Common.Writer.PropertyKeyValuePairs(writer, "Units", environment.Units, Common.Writer.PropertyString);
                Common.Writer.PropertyBoolean(writer, "SpecificLocations", environment.SpecificLocations);
                Common.Writer.PropertyInlineArray(writer, "FocusCells", environment.FocusCells, JsonAddPropertyFocusCell);
                Common.Writer.PropertyArray(writer, "CellEnvironment", environment.CellEnvironment, JsonAddPropertyCellEnvironment);
                Common.Writer.PropertyInlineArray(writer, "FileNames", environment.FileNames, Common.Writer.WriteString);
                writer.WriteEndObject();
            }
        }

        public static Madingley.Common.Environment Deserialize(TextReader sr)
        {
            Func<Newtonsoft.Json.JsonTextReader, Tuple<int, int>> JsonReadFocusCell = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                reader.Read();
                var item1 = Common.Reader.ReadInt(reader);

                reader.Read();
                var item2 = Common.Reader.ReadInt(reader);

                reader.Read();
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.EndArray);

                return Tuple.Create(item1, item2);
            };

            Func<Newtonsoft.Json.JsonTextReader, IDictionary<string, double[]>> JsonReadCellEnvironment = (reader) =>
            {
                var ret = new Dictionary<string, double[]>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read() &&
                    reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
                {
                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var key = Convert.ToString(reader.Value);
                    reader.Read();
                    var value = Common.Reader.ReadArray(reader, Common.Reader.ReadDouble);

                    ret.Add(key, value.ToArray());
                }

                return ret;
            };

            var environment = new Madingley.Common.Environment();

            using (var reader = new Newtonsoft.Json.JsonTextReader(sr))
            {
                reader.Read();
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read() &&
                    reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
                {
                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var property = Convert.ToString(reader.Value);
                    reader.Read();

                    switch (property)
                    {
                        case "CellSize": environment.CellSize = Common.Reader.ReadDouble(reader); break;
                        case "BottomLatitude": environment.BottomLatitude = Common.Reader.ReadDouble(reader); break;
                        case "TopLatitude": environment.TopLatitude = Common.Reader.ReadDouble(reader); break;
                        case "LeftmostLongitude": environment.LeftmostLongitude = Common.Reader.ReadDouble(reader); break;
                        case "RightmostLongitude": environment.RightmostLongitude = Common.Reader.ReadDouble(reader); break;
                        case "Units": environment.Units = Common.Reader.ReadKeyValuePairs(reader, Common.Reader.ReadString); break;
                        case "SpecificLocations": environment.SpecificLocations = Common.Reader.ReadBoolean(reader); break;
                        case "FocusCells": environment.FocusCells = Common.Reader.ReadArray(reader, JsonReadFocusCell).ToList(); break;
                        case "CellEnvironment": environment.CellEnvironment = Common.Reader.ReadArray(reader, JsonReadCellEnvironment).ToList(); break;
                        case "FileNames": environment.FileNames = Common.Reader.ReadArray(reader, Common.Reader.ReadString).ToList(); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }
            }

            return environment;
        }
    }
}
