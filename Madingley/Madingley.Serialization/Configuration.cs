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
            Action<Newtonsoft.Json.JsonTextWriter, Madingley.Common.FunctionalGroupDefinition> JsonAddPropertyFunctionalGroupDefinition = (jsonTextWriter, value) =>
            {
                jsonTextWriter.WriteStartObject();
                Common.Writer.KeyValuePairArray(jsonTextWriter, "Definitions", value.Definitions, Common.Writer.PropertyString);
                Common.Writer.KeyValuePairArray(jsonTextWriter, "Properties", value.Properties, Common.Writer.PropertyDouble);
                jsonTextWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, Madingley.Common.FunctionalGroupDefinitions> JsonAddPropertyFunctionalGroupDefinitions = (jsonTextWriter, name, value) =>
            {
                jsonTextWriter.WritePropertyName(name);
                jsonTextWriter.WriteStartObject();
                Common.Writer.PropertyArray(jsonTextWriter, "Data", value.Data, JsonAddPropertyFunctionalGroupDefinition);
                Common.Writer.PropertyInlineArray(jsonTextWriter, "Definitions", value.Definitions, Common.Writer.WriteString);
                Common.Writer.PropertyInlineArray(jsonTextWriter, "Properties", value.Properties, Common.Writer.WriteString);
                jsonTextWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, Madingley.Common.ScenarioParameter> JsonAddPropertyScenarioParameter = (jsonTextWriter, name, value) =>
            {
                jsonTextWriter.WritePropertyName(name);
                jsonTextWriter.WriteStartObject();
                Common.Writer.PropertyString(jsonTextWriter, "ParamString", value.ParamString);
                Common.Writer.PropertyDouble(jsonTextWriter, "ParamDouble1", value.ParamDouble1);
                Common.Writer.PropertyDouble(jsonTextWriter, "ParamDouble2", value.ParamDouble2);
                jsonTextWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Madingley.Common.ScenarioParameters> JsonAddScenarioParameter = (jsonTextWriter, value) =>
            {
                jsonTextWriter.WriteStartObject();
                Common.Writer.PropertyString(jsonTextWriter, "Label", value.Label);
                Common.Writer.PropertyInt(jsonTextWriter, "SimulationNumber", value.SimulationNumber);
                Common.Writer.KeyValuePairArray(jsonTextWriter, "Parameters", value.Parameters, JsonAddPropertyScenarioParameter);
                jsonTextWriter.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, Madingley.Common.EcologicalParameters> JsonAddEcologicalParameters = (jsonTextWriter, name, ecologicalParameters) =>
            {
                jsonTextWriter.WritePropertyName(name);
                jsonTextWriter.WriteStartObject();
                Common.Writer.KeyValuePairArray(jsonTextWriter, "Parameters", ecologicalParameters.Parameters, Common.Writer.PropertyDouble);
                Common.Writer.PropertyInlineArray(jsonTextWriter, "TimeUnits", ecologicalParameters.TimeUnits, Common.Writer.WriteString);
                jsonTextWriter.WriteEndObject();
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
                        case "Definitions": ret.Definitions = Common.Reader.JsonReadKVPs(reader, Common.Reader.JsonReadString); break;
                        case "Properties": ret.Properties = Common.Reader.JsonReadKVPs(reader, Common.Reader.JsonReadDouble); break;
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
                            case "Data": ret.Data = Common.Reader.JsonReadArray(reader, JsonReadFunctionalGroupDefinition); break;
                            case "Definitions": ret.Definitions = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadString); break;
                            case "Properties": ret.Properties = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadString); break;
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
                        case "ParamString": ret.ParamString = Common.Reader.JsonReadString(reader); break;
                        case "ParamDouble1": ret.ParamDouble1 = Common.Reader.JsonReadDouble(reader); break;
                        case "ParamDouble2": ret.ParamDouble2 = Common.Reader.JsonReadDouble(reader); break;
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
                            case "Label": ret.Label = Common.Reader.JsonReadString(reader); break;
                            case "SimulationNumber": ret.SimulationNumber = Common.Reader.JsonReadInt(reader); break;
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
                            case "Parameters": ret.Parameters = Common.Reader.JsonReadKVPs(reader, Common.Reader.JsonReadDouble); break;
                            case "TimeUnits": ret.TimeUnits = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadString); break;
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
                        case "GlobalModelTimeStepUnit": configuration.GlobalModelTimeStepUnit = Common.Reader.JsonReadString(reader); break;
                        case "NumTimeSteps": configuration.NumTimeSteps = Common.Reader.JsonReadInt(reader); break;
                        case "BurninTimeSteps": configuration.BurninTimeSteps = Common.Reader.JsonReadInt(reader); break;
                        case "ImpactTimeSteps": configuration.ImpactTimeSteps = Common.Reader.JsonReadInt(reader); break;
                        case "RecoveryTimeSteps": configuration.RecoveryTimeSteps = Common.Reader.JsonReadInt(reader); break;
                        case "RunCellsInParallel": configuration.RunCellsInParallel = Common.Reader.JsonReadBool(reader); break;
                        case "RunSimulationsInParallel": configuration.RunSimulationsInParallel = Common.Reader.JsonReadBool(reader); break;
                        case "RunRealm": configuration.RunRealm = Common.Reader.JsonReadString(reader); break;
                        case "DrawRandomly": configuration.DrawRandomly = Common.Reader.JsonReadBool(reader); break;
                        case "ExtinctionThreshold": configuration.ExtinctionThreshold = Common.Reader.JsonReadDouble(reader); break;
                        case "MaxNumberOfCohorts": configuration.MaxNumberOfCohorts = Common.Reader.JsonReadInt(reader); break;
                        case "DispersalOnly": configuration.DispersalOnly = Common.Reader.JsonReadBool(reader); break;
                        case "DispersalOnlyType": configuration.DispersalOnlyType = Common.Reader.JsonReadString(reader); break;
                        case "PlanktonDispersalThreshold": configuration.PlanktonDispersalThreshold = Common.Reader.JsonReadDouble(reader); break;
                        case "CohortFunctionalGroupDefinitions": configuration.CohortFunctionalGroupDefinitions = JsonReadFunctionalGroupDefinitions(reader); break;
                        case "StockFunctionalGroupDefinitions": configuration.StockFunctionalGroupDefinitions = JsonReadFunctionalGroupDefinitions(reader); break;
                        case "ImpactCellIndices": configuration.ImpactCellIndices = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadInt); break;
                        case "ImpactAll": configuration.ImpactAll = Common.Reader.JsonReadBool(reader); break;
                        case "ScenarioParameters": configuration.ScenarioParameters = Common.Reader.JsonReadArray(reader, JsonReadScenarioParameters).ToList(); break;
                        case "ScenarioIndex": configuration.ScenarioIndex = Common.Reader.JsonReadInt(reader); break;
                        case "Simulation": configuration.Simulation = Common.Reader.JsonReadInt(reader); break;
                        case "EcologicalParameters": configuration.EcologicalParameters = JsonReadEcologicalParameters(reader); break;
                        case "FileNames": configuration.FileNames = Common.Reader.JsonReadArray(reader, Common.Reader.JsonReadString).ToList(); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }
            }

            return configuration;
        }
    }
}
