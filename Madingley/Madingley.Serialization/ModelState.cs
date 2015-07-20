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
            Action<Newtonsoft.Json.JsonWriter, Madingley.Common.Cohort> JsonAddPropertyCohort = (jsonWriter, value) =>
            {
                jsonWriter.WriteStartObject();
                Common.Writer.PropertyInt(jsonWriter, "BirthTimeStep", value.BirthTimeStep);
                Common.Writer.PropertyInt(jsonWriter, "MaturityTimeStep", value.MaturityTimeStep);
                Common.Writer.PropertyInlineArray(jsonWriter, "IDs", value.IDs, Common.Writer.WriteInt);
                Common.Writer.PropertyDouble(jsonWriter, "JuvenileMass", value.JuvenileMass);
                Common.Writer.PropertyDouble(jsonWriter, "AdultMass", value.AdultMass);
                Common.Writer.PropertyDouble(jsonWriter, "IndividualBodyMass", value.IndividualBodyMass);
                Common.Writer.PropertyDouble(jsonWriter, "IndividualReproductivePotentialMass", value.IndividualReproductivePotentialMass);
                Common.Writer.PropertyDouble(jsonWriter, "MaximumAchievedBodyMass", value.MaximumAchievedBodyMass);
                Common.Writer.PropertyDouble(jsonWriter, "Abundance", value.Abundance);
                Common.Writer.PropertyBoolean(jsonWriter, "Merged", value.Merged);
                Common.Writer.PropertyDouble(jsonWriter, "ProportionTimeActive", value.ProportionTimeActive);
                Common.Writer.PropertyDouble(jsonWriter, "TrophicIndex", value.TrophicIndex);
                Common.Writer.PropertyDouble(jsonWriter, "LogOptimalPreyBodySizeRatio", value.LogOptimalPreyBodySizeRatio);
                jsonWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonWriter, Madingley.Common.Stock> JsonAddPropertyStock = (jsonWriter, value) =>
            {
                jsonWriter.WriteStartObject();
                Common.Writer.PropertyDouble(jsonWriter, "IndividualBodyMass", value.IndividualBodyMass);
                Common.Writer.PropertyDouble(jsonWriter, "TotalBiomass", value.TotalBiomass);
                jsonWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonWriter, Madingley.Common.GridCell> JsonAddPropertyGridCell = (jsonWriter, gridCell) =>
            {
                jsonWriter.WriteStartObject();
                Common.Writer.PropertyDouble(jsonWriter, "Latitude", gridCell.Latitude);
                Common.Writer.PropertyDouble(jsonWriter, "Longitude", gridCell.Longitude);
                Common.Writer.PropertyArray(jsonWriter, "Cohorts", gridCell.Cohorts, (JsonWriter, value) => Common.Writer.WriteArray(JsonWriter, value, JsonAddPropertyCohort));
                Common.Writer.PropertyArray(jsonWriter, "Stocks", gridCell.Stocks, (JsonWriter, value) => Common.Writer.WriteArray(JsonWriter, value, JsonAddPropertyStock));
                Common.Writer.PropertyKeyValuePairs(jsonWriter, "Environment", gridCell.Environment, (JsonWriter, key, val) => Common.Writer.PropertyInlineArray(JsonWriter, key, val, Common.Writer.WriteDouble));
                jsonWriter.WriteEndObject();
            };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();
                Common.Writer.PropertyInt(writer, "TimestepsComplete", modelState.TimestepsComplete);
                Common.Writer.PropertyKeyValuePairs(writer, "GlobalDiagnosticVariables", modelState.GlobalDiagnosticVariables, Common.Writer.PropertyDouble);
                Common.Writer.PropertyArray(writer, "GridCells", modelState.GridCells, JsonAddPropertyGridCell);
                Common.Writer.PropertyLong(writer, "NextCohortID", modelState.NextCohortID);
                writer.WriteEndObject();
            }
        }

        public static Madingley.Common.ModelState Deserialize(TextReader sr)
        {
            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.Cohort> JsonReadCohort = (reader) =>
            {
                var ret = new Madingley.Common.Cohort();

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
                        case "BirthTimeStep": ret.BirthTimeStep = Common.Reader.ReadInt(reader); break;
                        case "MaturityTimeStep": ret.MaturityTimeStep = Common.Reader.ReadInt(reader); break;
                        case "IDs": ret.IDs = Common.Reader.ReadArray(reader, Common.Reader.ReadInt); break;
                        case "JuvenileMass": ret.JuvenileMass = Common.Reader.ReadDouble(reader); break;
                        case "AdultMass": ret.AdultMass = Common.Reader.ReadDouble(reader); break;
                        case "IndividualBodyMass": ret.IndividualBodyMass = Common.Reader.ReadDouble(reader); break;
                        case "IndividualReproductivePotentialMass": ret.IndividualReproductivePotentialMass = Common.Reader.ReadDouble(reader); break;
                        case "MaximumAchievedBodyMass": ret.MaximumAchievedBodyMass = Common.Reader.ReadDouble(reader); break;
                        case "Abundance": ret.Abundance = Common.Reader.ReadDouble(reader); break;
                        case "Merged": ret.Merged = Common.Reader.ReadBoolean(reader); break;
                        case "ProportionTimeActive": ret.ProportionTimeActive = Common.Reader.ReadDouble(reader); break;
                        case "TrophicIndex": ret.TrophicIndex = Common.Reader.ReadDouble(reader); break;
                        case "LogOptimalPreyBodySizeRatio": ret.LogOptimalPreyBodySizeRatio = Common.Reader.ReadDouble(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.Stock> JsonReadStock = (reader) =>
            {
                var ret = new Madingley.Common.Stock();

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
                        case "IndividualBodyMass": ret.IndividualBodyMass = Common.Reader.ReadDouble(reader); break;
                        case "TotalBiomass": ret.TotalBiomass = Common.Reader.ReadDouble(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
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

            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.GridCell> JsonReadGridCell = (reader) =>
            {
                var ret = new Madingley.Common.GridCell();

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
                        case "Latitude": ret.Latitude = Common.Reader.ReadDouble(reader); break;
                        case "Longitude": ret.Longitude = Common.Reader.ReadDouble(reader); break;
                        case "Cohorts": ret.Cohorts = Common.Reader.ReadArray(reader, r => Common.Reader.ReadArray(r, JsonReadCohort)).ToList(); break;
                        case "Stocks": ret.Stocks = Common.Reader.ReadArray(reader, r => Common.Reader.ReadArray(r, JsonReadStock)).ToList(); break;
                        case "Environment": ret.Environment = JsonReadCellEnvironment(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            var modelState = new Madingley.Common.ModelState();

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
                        case "TimestepsComplete": modelState.TimestepsComplete = Common.Reader.ReadInt(reader); break;
                        case "GlobalDiagnosticVariables": modelState.GlobalDiagnosticVariables = Common.Reader.ReadKeyValuePairs(reader, Common.Reader.ReadDouble); break;
                        case "GridCells": modelState.GridCells = Common.Reader.ReadArray(reader, JsonReadGridCell).ToList(); break;
                        case "NextCohortID": modelState.NextCohortID = Common.Reader.ReadLong(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }
            }

            return modelState;
        }
    }
}
