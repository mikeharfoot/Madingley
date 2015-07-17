using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Madingley.Serialization
{
    public static class ModelState
    {
        public static void Serialize(Madingley.Common.ModelState modelState, TextWriter sw)
        {
            Action<Newtonsoft.Json.JsonTextWriter, string, bool> JsonAddPropertyBool = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, int> JsonAddPropertyInt = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, double> JsonAddPropertyDouble = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, long> JsonAddPropertyLong = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value.ToString());
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

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<int>> JsonAddPropertyIntArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.Formatting = Newtonsoft.Json.Formatting.None;
                sb.WriteStartArray();
                value.ToList().ForEach(v => sb.WriteValue(v));
                sb.WriteEndArray();
                sb.Formatting = Newtonsoft.Json.Formatting.Indented;
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<KeyValuePair<string, double>>> JsonAddKVPDoubles = (sb, name, kvps) =>
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

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<KeyValuePair<string, double[]>>> JsonAddPropertyCellEnvironment = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartObject();
                value.ToList().ForEach(kvp => JsonAddPropertyDoubleArray(sb, kvp.Key, kvp.Value));
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Common.Cohort> JsonAddPropertyCohort = (sb, value) =>
            {
                sb.WriteStartObject();
                JsonAddPropertyInt(sb, "BirthTimeStep", value.BirthTimeStep);
                JsonAddPropertyInt(sb, "MaturityTimeStep", value.MaturityTimeStep);
                JsonAddPropertyIntArray(sb, "IDs", value.IDs);
                JsonAddPropertyDouble(sb, "JuvenileMass", value.JuvenileMass);
                JsonAddPropertyDouble(sb, "AdultMass", value.AdultMass);
                JsonAddPropertyDouble(sb, "IndividualBodyMass", value.IndividualBodyMass);
                JsonAddPropertyDouble(sb, "IndividualReproductivePotentialMass", value.IndividualReproductivePotentialMass);
                JsonAddPropertyDouble(sb, "MaximumAchievedBodyMass", value.MaximumAchievedBodyMass);
                JsonAddPropertyDouble(sb, "Abundance", value.Abundance);
                JsonAddPropertyBool(sb, "Merged", value.Merged);
                JsonAddPropertyDouble(sb, "ProportionTimeActive", value.ProportionTimeActive);
                JsonAddPropertyDouble(sb, "TrophicIndex", value.TrophicIndex);
                JsonAddPropertyDouble(sb, "LogOptimalPreyBodySizeRatio", value.LogOptimalPreyBodySizeRatio);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, IEnumerable<Common.Cohort>> JsonAddPropertyCohortArray = (sb, value) =>
            {
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyCohort(sb, v));
                sb.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IList<IEnumerable<Common.Cohort>>> JsonAddPropertyCohortArrayArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyCohortArray(sb, v));
                sb.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Common.Stock> JsonAddPropertyStock = (sb, value) =>
            {
                sb.WriteStartObject();
                JsonAddPropertyDouble(sb, "IndividualBodyMass", value.IndividualBodyMass);
                JsonAddPropertyDouble(sb, "TotalBiomass", value.TotalBiomass);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, IEnumerable<Common.Stock>> JsonAddPropertyStockArray = (sb, value) =>
            {
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyStock(sb, v));
                sb.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IList<IEnumerable<Common.Stock>>> JsonAddPropertyStockArrayArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyStockArray(sb, v));
                sb.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Common.GridCell> JsonAddPropertyGridCell = (sb, gridCell) =>
            {

                sb.WriteStartObject();
                JsonAddPropertyDouble(sb, "Latitude", gridCell.Latitude);
                JsonAddPropertyDouble(sb, "Longitude", gridCell.Longitude);
                JsonAddPropertyCohortArrayArray(sb, "Cohorts", gridCell.Cohorts);
                JsonAddPropertyStockArrayArray(sb, "Stocks", gridCell.Stocks);
                JsonAddPropertyCellEnvironment(sb, "Environment", gridCell.Environment);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<Common.GridCell>> JsonAddPropertyGridCellArray = (sb, name, gridCells) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartArray();
                gridCells.ToList().ForEach(gridCell => JsonAddPropertyGridCell(sb, gridCell));
                sb.WriteEndArray();
            };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();

                JsonAddPropertyInt(writer, "TimestepsComplete", modelState.TimestepsComplete);
                JsonAddKVPDoubles(writer, "GlobalDiagnosticVariables", modelState.GlobalDiagnosticVariables);
                JsonAddPropertyGridCellArray(writer, "GridCells", modelState.GridCells);
                JsonAddPropertyLong(writer, "NextCohortID", modelState.NextCohortID);

                writer.WriteEndObject();
            }
        }

        public static Madingley.Common.ModelState Deserialize(TextReader sr)
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

            Func<Newtonsoft.Json.JsonTextReader, Int64> JsonReadLong = (reader) =>
            {
                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.String);
                Debug.Assert(reader.ValueType == typeof(string));
                return Convert.ToInt64(reader.Value);
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

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<int>> JsonReadIntArray = (reader) =>
            {
                var ret = new List<int>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadInt(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IDictionary<string, double>> JsonReadKVPDoubles = (reader) =>
            {
                var ret = new Dictionary<string, double>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var key = Convert.ToString(reader.Value);
                    reader.Read();
                    var value = JsonReadDouble(reader);

                    ret.Add(key, value);
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Common.Cohort> JsonReadCohort = (reader) =>
            {
                var ret = new Common.Cohort();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var property = Convert.ToString(reader.Value);
                    reader.Read();

                    switch (property)
                    {
                        case "BirthTimeStep": ret.BirthTimeStep = JsonReadInt(reader); break;
                        case "MaturityTimeStep": ret.MaturityTimeStep = JsonReadInt(reader); break;
                        case "IDs": ret.IDs = JsonReadIntArray(reader); break;
                        case "JuvenileMass": ret.JuvenileMass = JsonReadDouble(reader); break;
                        case "AdultMass": ret.AdultMass = JsonReadDouble(reader); break;
                        case "IndividualBodyMass": ret.IndividualBodyMass = JsonReadDouble(reader); break;
                        case "IndividualReproductivePotentialMass": ret.IndividualReproductivePotentialMass = JsonReadDouble(reader); break;
                        case "MaximumAchievedBodyMass": ret.MaximumAchievedBodyMass = JsonReadDouble(reader); break;
                        case "Abundance": ret.Abundance = JsonReadDouble(reader); break;
                        case "Merged": ret.Merged = JsonReadBool(reader); break;
                        case "ProportionTimeActive": ret.ProportionTimeActive = JsonReadDouble(reader); break;
                        case "TrophicIndex": ret.TrophicIndex = JsonReadDouble(reader); break;
                        case "LogOptimalPreyBodySizeRatio": ret.LogOptimalPreyBodySizeRatio = JsonReadDouble(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<Common.Cohort>> JsonReadCohortArray = (reader) =>
            {
                var ret = new List<Common.Cohort>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadCohort(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IList<IEnumerable<Common.Cohort>>> JsonReadCohortArrayArray = (reader) =>
                {
                    var ret = new List<IEnumerable<Common.Cohort>>();

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                    while (reader.Read())
                    {
                        if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                        ret.Add(JsonReadCohortArray(reader));
                    }

                    return ret;
                };

            Func<Newtonsoft.Json.JsonTextReader, Common.Stock> JsonReadStock = (reader) =>
            {
                var ret = new Common.Stock();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var property = Convert.ToString(reader.Value);
                    reader.Read();

                    switch (property)
                    {
                        case "IndividualBodyMass": ret.IndividualBodyMass = JsonReadDouble(reader); break;
                        case "TotalBiomass": ret.TotalBiomass = JsonReadDouble(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<Common.Stock>> JsonReadStockArray = (reader) =>
            {
                var ret = new List<Common.Stock>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadStock(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IList<IEnumerable<Common.Stock>>> JsonReadStockArrayArray = (reader) =>
            {
                var ret = new List<IEnumerable<Common.Stock>>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadStockArray(reader));
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

            Func<Newtonsoft.Json.JsonTextReader, Common.GridCell> JsonReadGridCall = (reader) =>
            {
                var ret = new Common.GridCell();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var property = Convert.ToString(reader.Value);
                    reader.Read();

                    switch (property)
                    {
                        case "Latitude": ret.Latitude = JsonReadDouble(reader); break;
                        case "Longitude": ret.Longitude = JsonReadDouble(reader); break;
                        case "Cohorts": ret.Cohorts = JsonReadCohortArrayArray(reader); break;
                        case "Stocks": ret.Stocks = JsonReadStockArrayArray(reader); break;
                        case "Environment": ret.Environment = JsonReadCellEnvironment(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IList<Common.GridCell>> JsonReadGridCallArray = (reader) =>
                {
                    var ret = new List<Common.GridCell>();

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                    while (reader.Read())
                    {
                        if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                        ret.Add(JsonReadGridCall(reader));
                    }

                    return ret;
                };

            var modelState = new Madingley.Common.ModelState();

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
                            case "TimestepsComplete": modelState.TimestepsComplete = JsonReadInt(reader); break;
                            case "GlobalDiagnosticVariables": modelState.GlobalDiagnosticVariables = JsonReadKVPDoubles(reader); break;
                            case "GridCells": modelState.GridCells = JsonReadGridCallArray(reader); break;
                            case "NextCohortID": modelState.NextCohortID = JsonReadLong(reader); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }
                }
            }

            return modelState;
        }
    }
}
