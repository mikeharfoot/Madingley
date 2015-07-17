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
            Action<Newtonsoft.Json.JsonTextWriter, Madingley.Common.Cohort> JsonAddPropertyCohort = (sb, value) =>
            {
                sb.WriteStartObject();
                Common.Writer.PropertyInt(sb, "BirthTimeStep", value.BirthTimeStep);
                Common.Writer.PropertyInt(sb, "MaturityTimeStep", value.MaturityTimeStep);
                Common.Writer.PropertyInlineArray(sb, "IDs", value.IDs, Common.Writer.WriteInt);
                Common.Writer.PropertyDouble(sb, "JuvenileMass", value.JuvenileMass);
                Common.Writer.PropertyDouble(sb, "AdultMass", value.AdultMass);
                Common.Writer.PropertyDouble(sb, "IndividualBodyMass", value.IndividualBodyMass);
                Common.Writer.PropertyDouble(sb, "IndividualReproductivePotentialMass", value.IndividualReproductivePotentialMass);
                Common.Writer.PropertyDouble(sb, "MaximumAchievedBodyMass", value.MaximumAchievedBodyMass);
                Common.Writer.PropertyDouble(sb, "Abundance", value.Abundance);
                Common.Writer.PropertyBoolean(sb, "Merged", value.Merged);
                Common.Writer.PropertyDouble(sb, "ProportionTimeActive", value.ProportionTimeActive);
                Common.Writer.PropertyDouble(sb, "TrophicIndex", value.TrophicIndex);
                Common.Writer.PropertyDouble(sb, "LogOptimalPreyBodySizeRatio", value.LogOptimalPreyBodySizeRatio);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Madingley.Common.Stock> JsonAddPropertyStock = (sb, value) =>
            {
                sb.WriteStartObject();
                Common.Writer.PropertyDouble(sb, "IndividualBodyMass", value.IndividualBodyMass);
                Common.Writer.PropertyDouble(sb, "TotalBiomass", value.TotalBiomass);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Madingley.Common.GridCell> JsonAddPropertyGridCell = (sb, gridCell) =>
            {
                sb.WriteStartObject();
                Common.Writer.PropertyDouble(sb, "Latitude", gridCell.Latitude);
                Common.Writer.PropertyDouble(sb, "Longitude", gridCell.Longitude);
                Common.Writer.PropertyArray(sb, "Cohorts", gridCell.Cohorts, (jsonTextWriter, value) => Common.Writer.PropertyArrayValue(jsonTextWriter, value, JsonAddPropertyCohort));
                Common.Writer.PropertyArray(sb, "Stocks", gridCell.Stocks, (jsonTextWriter, value) => Common.Writer.PropertyArrayValue(jsonTextWriter, value, JsonAddPropertyStock));
                Common.Writer.KeyValuePairArray(sb, "Environment", gridCell.Environment, (jsonTextWriter, key, val) => Common.Writer.PropertyInlineArray(jsonTextWriter, key, val, Common.Writer.WriteDouble));
                sb.WriteEndObject();
            };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();
                Common.Writer.PropertyInt(writer, "TimestepsComplete", modelState.TimestepsComplete);
                Common.Writer.KeyValuePairArray(writer, "GlobalDiagnosticVariables", modelState.GlobalDiagnosticVariables, Common.Writer.PropertyDouble);
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
                        case "BirthTimeStep": ret.BirthTimeStep = Common.Reader.JsonReadInt(reader); break;
                        case "MaturityTimeStep": ret.MaturityTimeStep = Common.Reader.JsonReadInt(reader); break;
                        case "IDs": ret.IDs = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadInt); break;
                        case "JuvenileMass": ret.JuvenileMass = Common.Reader.JsonReadDouble(reader); break;
                        case "AdultMass": ret.AdultMass = Common.Reader.JsonReadDouble(reader); break;
                        case "IndividualBodyMass": ret.IndividualBodyMass = Common.Reader.JsonReadDouble(reader); break;
                        case "IndividualReproductivePotentialMass": ret.IndividualReproductivePotentialMass = Common.Reader.JsonReadDouble(reader); break;
                        case "MaximumAchievedBodyMass": ret.MaximumAchievedBodyMass = Common.Reader.JsonReadDouble(reader); break;
                        case "Abundance": ret.Abundance = Common.Reader.JsonReadDouble(reader); break;
                        case "Merged": ret.Merged = Common.Reader.JsonReadBool(reader); break;
                        case "ProportionTimeActive": ret.ProportionTimeActive = Common.Reader.JsonReadDouble(reader); break;
                        case "TrophicIndex": ret.TrophicIndex = Common.Reader.JsonReadDouble(reader); break;
                        case "LogOptimalPreyBodySizeRatio": ret.LogOptimalPreyBodySizeRatio = Common.Reader.JsonReadDouble(reader); break;
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
                        case "IndividualBodyMass": ret.IndividualBodyMass = Common.Reader.JsonReadDouble(reader); break;
                        case "TotalBiomass": ret.TotalBiomass = Common.Reader.JsonReadDouble(reader); break;
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
                    var value = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadDouble);

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
                        case "Latitude": ret.Latitude = Common.Reader.JsonReadDouble(reader); break;
                        case "Longitude": ret.Longitude = Common.Reader.JsonReadDouble(reader); break;
                        case "Cohorts": ret.Cohorts = Common.Reader.JsonReadArray(reader, r => Common.Reader.JsonReadArray(r, JsonReadCohort)).ToList(); break;
                        case "Stocks": ret.Stocks = Common.Reader.JsonReadArray(reader, r => Common.Reader.JsonReadArray(r, JsonReadStock)).ToList(); break;
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
                        case "TimestepsComplete": modelState.TimestepsComplete = Common.Reader.JsonReadInt(reader); break;
                        case "GlobalDiagnosticVariables": modelState.GlobalDiagnosticVariables = Common.Reader.JsonReadKVPs(reader, Common.Reader.JsonReadDouble); break;
                        case "GridCells": modelState.GridCells = Common.Reader.JsonReadArray(reader, JsonReadGridCell).ToList(); break;
                        case "NextCohortID": modelState.NextCohortID = Common.Reader.JsonReadLong(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }
            }

            return modelState;
        }
    }
}
