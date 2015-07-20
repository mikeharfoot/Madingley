using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Madingley.Serialization
{
    public static class Configuration
    {
        public static void Serialize(Madingley.Common.Configuration configuration, TextWriter textWriter)
        {
            Action<Newtonsoft.Json.JsonWriter, Madingley.Common.FunctionalGroupDefinition> JsonAddPropertyFunctionalGroupDefinition = (JsonWriter, value) =>
            {
                JsonWriter.WriteStartObject();
                Common.Writer.PropertyKeyValuePairs(JsonWriter, "Definitions", value.Definitions, Common.Writer.PropertyString);
                Common.Writer.PropertyKeyValuePairs(JsonWriter, "Properties", value.Properties, Common.Writer.PropertyDouble);
                JsonWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonWriter, string, Madingley.Common.FunctionalGroupDefinitions> JsonAddPropertyFunctionalGroupDefinitions = (JsonWriter, name, value) =>
            {
                JsonWriter.WritePropertyName(name);
                JsonWriter.WriteStartObject();
                Common.Writer.PropertyArray(JsonWriter, "Data", value.Data, JsonAddPropertyFunctionalGroupDefinition);
                Common.Writer.PropertyInlineArray(JsonWriter, "Definitions", value.Definitions, Common.Writer.WriteString);
                Common.Writer.PropertyInlineArray(JsonWriter, "Properties", value.Properties, Common.Writer.WriteString);
                JsonWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonWriter, string, Madingley.Common.ScenarioParameter> JsonAddPropertyScenarioParameter = (JsonWriter, name, value) =>
            {
                JsonWriter.WritePropertyName(name);
                JsonWriter.WriteStartObject();
                Common.Writer.PropertyString(JsonWriter, "ParamString", value.ParamString);
                Common.Writer.PropertyDouble(JsonWriter, "ParamDouble1", value.ParamDouble1);
                Common.Writer.PropertyDouble(JsonWriter, "ParamDouble2", value.ParamDouble2);
                JsonWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonWriter, Madingley.Common.ScenarioParameters> JsonAddScenarioParameter = (JsonWriter, value) =>
            {
                JsonWriter.WriteStartObject();
                Common.Writer.PropertyString(JsonWriter, "Label", value.Label);
                Common.Writer.PropertyInt(JsonWriter, "SimulationNumber", value.SimulationNumber);
                Common.Writer.PropertyKeyValuePairs(JsonWriter, "Parameters", value.Parameters, JsonAddPropertyScenarioParameter);
                JsonWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonWriter, string, Madingley.Common.EcologicalParameters> JsonAddEcologicalParameters = (JsonWriter, name, ecologicalParameters) =>
            {
                JsonWriter.WritePropertyName(name);
                JsonWriter.WriteStartObject();
                Common.Writer.PropertyKeyValuePairs(JsonWriter, "Parameters", ecologicalParameters.Parameters, Common.Writer.PropertyDouble);
                Common.Writer.PropertyInlineArray(JsonWriter, "TimeUnits", ecologicalParameters.TimeUnits, Common.Writer.WriteString);
                JsonWriter.WriteEndObject();
            };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(textWriter))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();
                Common.Writer.PropertyString(writer, "GlobalModelTimeStepUnit", configuration.GlobalModelTimeStepUnit);
                Common.Writer.PropertyInt(writer, "NumTimeSteps", configuration.NumTimeSteps);
                Common.Writer.PropertyInt(writer, "BurninTimeSteps", configuration.BurninTimeSteps);
                Common.Writer.PropertyInt(writer, "ImpactTimeSteps", configuration.ImpactTimeSteps);
                Common.Writer.PropertyInt(writer, "RecoveryTimeSteps", configuration.RecoveryTimeSteps);
                Common.Writer.PropertyBoolean(writer, "RunCellsInParallel", configuration.RunCellsInParallel);
                Common.Writer.PropertyBoolean(writer, "RunSimulationsInParallel", configuration.RunSimulationsInParallel);
                Common.Writer.PropertyString(writer, "RunRealm", configuration.RunRealm);
                Common.Writer.PropertyBoolean(writer, "DrawRandomly", configuration.DrawRandomly);
                Common.Writer.PropertyDouble(writer, "ExtinctionThreshold", configuration.ExtinctionThreshold);
                Common.Writer.PropertyInt(writer, "MaxNumberOfCohorts", configuration.MaxNumberOfCohorts);
                Common.Writer.PropertyBoolean(writer, "DispersalOnly", configuration.DispersalOnly);
                Common.Writer.PropertyString(writer, "DispersalOnlyType", configuration.DispersalOnlyType);
                Common.Writer.PropertyDouble(writer, "PlanktonDispersalThreshold", configuration.PlanktonDispersalThreshold);
                JsonAddPropertyFunctionalGroupDefinitions(writer, "CohortFunctionalGroupDefinitions", configuration.CohortFunctionalGroupDefinitions);
                JsonAddPropertyFunctionalGroupDefinitions(writer, "StockFunctionalGroupDefinitions", configuration.StockFunctionalGroupDefinitions);
                Common.Writer.PropertyInlineArray(writer, "ImpactCellIndices", configuration.ImpactCellIndices, Common.Writer.WriteInt);
                Common.Writer.PropertyBoolean(writer, "ImpactAll", configuration.ImpactAll);
                Common.Writer.PropertyArray(writer, "ScenarioParameters", configuration.ScenarioParameters, JsonAddScenarioParameter);
                Common.Writer.PropertyInt(writer, "ScenarioIndex", configuration.ScenarioIndex);
                Common.Writer.PropertyInt(writer, "Simulation", configuration.Simulation);
                JsonAddEcologicalParameters(writer, "EcologicalParameters", configuration.EcologicalParameters);
                Common.Writer.PropertyInlineArray(writer, "FileNames", configuration.FileNames, Common.Writer.WriteString);
                writer.WriteEndObject();
            }
        }

        public static Madingley.Common.Configuration Deserialize(TextReader sr)
        {
            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.FunctionalGroupDefinition> JsonReadFunctionalGroupDefinition = (reader) =>
            {
                var ret = new Madingley.Common.FunctionalGroupDefinition();

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
                        case "Definitions": ret.Definitions = Common.Reader.ReadKeyValuePairs(reader, Common.Reader.ReadString); break;
                        case "Properties": ret.Properties = Common.Reader.ReadKeyValuePairs(reader, Common.Reader.ReadDouble); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.FunctionalGroupDefinitions> JsonReadFunctionalGroupDefinitions = (reader) =>
                {
                    var ret = new Madingley.Common.FunctionalGroupDefinitions();

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
                            case "Data": ret.Data = Common.Reader.ReadArray(reader, JsonReadFunctionalGroupDefinition); break;
                            case "Definitions": ret.Definitions = Common.Reader.ReadArray(reader, Common.Reader.ReadString); break;
                            case "Properties": ret.Properties = Common.Reader.ReadArray(reader, Common.Reader.ReadString); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }

                    return ret;
                };

            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.ScenarioParameter> JsonReadScenarioParameter = (reader) =>
            {
                var ret = new Madingley.Common.ScenarioParameter();

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
                        case "ParamString": ret.ParamString = Common.Reader.ReadString(reader); break;
                        case "ParamDouble1": ret.ParamDouble1 = Common.Reader.ReadDouble(reader); break;
                        case "ParamDouble2": ret.ParamDouble2 = Common.Reader.ReadDouble(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IDictionary<string, Madingley.Common.ScenarioParameter>> JsonReadKVPScenarioParameter = (reader) =>
            {
                var ret = new Dictionary<string, Madingley.Common.ScenarioParameter>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read() &&
                    reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
                {
                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var key = Convert.ToString(reader.Value);
                    reader.Read();
                    var value = JsonReadScenarioParameter(reader);

                    ret.Add(key, value);
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.ScenarioParameters> JsonReadScenarioParameters = (reader) =>
                {
                    var ret = new Madingley.Common.ScenarioParameters();

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
                            case "Label": ret.Label = Common.Reader.ReadString(reader); break;
                            case "SimulationNumber": ret.SimulationNumber = Common.Reader.ReadInt(reader); break;
                            case "Parameters": ret.Parameters = JsonReadKVPScenarioParameter(reader); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }

                    return ret;
                };

            Func<Newtonsoft.Json.JsonTextReader, Madingley.Common.EcologicalParameters> JsonReadEcologicalParameters = (reader) =>
                {
                    var ret = new Madingley.Common.EcologicalParameters();

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
                            case "Parameters": ret.Parameters = Common.Reader.ReadKeyValuePairs(reader, Common.Reader.ReadDouble); break;
                            case "TimeUnits": ret.TimeUnits = Common.Reader.ReadArray(reader, Common.Reader.ReadString); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }

                    return ret;
                };

            var configuration = new Madingley.Common.Configuration();

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
                        case "GlobalModelTimeStepUnit": configuration.GlobalModelTimeStepUnit = Common.Reader.ReadString(reader); break;
                        case "NumTimeSteps": configuration.NumTimeSteps = Common.Reader.ReadInt(reader); break;
                        case "BurninTimeSteps": configuration.BurninTimeSteps = Common.Reader.ReadInt(reader); break;
                        case "ImpactTimeSteps": configuration.ImpactTimeSteps = Common.Reader.ReadInt(reader); break;
                        case "RecoveryTimeSteps": configuration.RecoveryTimeSteps = Common.Reader.ReadInt(reader); break;
                        case "RunCellsInParallel": configuration.RunCellsInParallel = Common.Reader.ReadBoolean(reader); break;
                        case "RunSimulationsInParallel": configuration.RunSimulationsInParallel = Common.Reader.ReadBoolean(reader); break;
                        case "RunRealm": configuration.RunRealm = Common.Reader.ReadString(reader); break;
                        case "DrawRandomly": configuration.DrawRandomly = Common.Reader.ReadBoolean(reader); break;
                        case "ExtinctionThreshold": configuration.ExtinctionThreshold = Common.Reader.ReadDouble(reader); break;
                        case "MaxNumberOfCohorts": configuration.MaxNumberOfCohorts = Common.Reader.ReadInt(reader); break;
                        case "DispersalOnly": configuration.DispersalOnly = Common.Reader.ReadBoolean(reader); break;
                        case "DispersalOnlyType": configuration.DispersalOnlyType = Common.Reader.ReadString(reader); break;
                        case "PlanktonDispersalThreshold": configuration.PlanktonDispersalThreshold = Common.Reader.ReadDouble(reader); break;
                        case "CohortFunctionalGroupDefinitions": configuration.CohortFunctionalGroupDefinitions = JsonReadFunctionalGroupDefinitions(reader); break;
                        case "StockFunctionalGroupDefinitions": configuration.StockFunctionalGroupDefinitions = JsonReadFunctionalGroupDefinitions(reader); break;
                        case "ImpactCellIndices": configuration.ImpactCellIndices = Common.Reader.ReadArray(reader, Common.Reader.ReadInt); break;
                        case "ImpactAll": configuration.ImpactAll = Common.Reader.ReadBoolean(reader); break;
                        case "ScenarioParameters": configuration.ScenarioParameters = Common.Reader.ReadArray(reader, JsonReadScenarioParameters).ToList(); break;
                        case "ScenarioIndex": configuration.ScenarioIndex = Common.Reader.ReadInt(reader); break;
                        case "Simulation": configuration.Simulation = Common.Reader.ReadInt(reader); break;
                        case "EcologicalParameters": configuration.EcologicalParameters = JsonReadEcologicalParameters(reader); break;
                        case "FileNames": configuration.FileNames = Common.Reader.ReadArray(reader, Common.Reader.ReadString).ToList(); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }
            }

            return configuration;
        }
    }
}
