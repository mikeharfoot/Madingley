using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Madingley.Serialization
{
    public static class Configuration
    {
        public static void Serialize(Madingley.Common.Configuration configuration, TextWriter sw)
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

            Action<Newtonsoft.Json.JsonTextWriter, string, int> JsonAddPropertyInt = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, string> JsonAddPropertyString = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteValue(value);
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

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<string>> JsonAddPropertyStringArray = (sb, name, value) =>
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

            Action<Newtonsoft.Json.JsonTextWriter, Common.FunctionalGroupDefinition> JsonAddPropertyFunctionalGroupDefinition = (sb, value) =>
            {
                sb.WriteStartObject();
                JsonAddKVPStrings(sb, "Definitions", value.Definitions);
                JsonAddKVPDoubles(sb, "Properties", value.Properties);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<Common.FunctionalGroupDefinition>> JsonAddPropertyFunctionalGroupDefinitionArray = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartArray();
                value.ToList().ForEach(v => JsonAddPropertyFunctionalGroupDefinition(sb, v));
                sb.WriteEndArray();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, Common.FunctionalGroupDefinitions> JsonAddPropertyFunctionalGroupDefinitions = (sb, name, value) =>
                {
                    sb.WritePropertyName(name);
                    sb.WriteStartObject();
                    JsonAddPropertyFunctionalGroupDefinitionArray(sb, "Data", value.Data);
                    JsonAddPropertyStringArray(sb, "Definitions", value.Definitions);
                    JsonAddPropertyStringArray(sb, "Properties", value.Properties);
                    sb.WriteEndObject();
                };

            Action<Newtonsoft.Json.JsonTextWriter, string, Common.ScenarioParameter> JsonAddPropertyScenarioParameter = (sb, name, value) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartObject();
                JsonAddPropertyString(sb, "ParamString", value.ParamString);
                JsonAddPropertyDouble(sb, "ParamDouble1", value.ParamDouble1);
                JsonAddPropertyDouble(sb, "ParamDouble2", value.ParamDouble2);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<KeyValuePair<string, Common.ScenarioParameter>>> JsonAddKVPScenarioParameter = (sb, name, kvps) =>
            {
                sb.WritePropertyName(name);
                sb.WriteStartObject();
                kvps.ToList().ForEach(kvp => JsonAddPropertyScenarioParameter(sb, kvp.Key, kvp.Value));
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, Common.ScenarioParameters> JsonAddScenarioParameter = (sb, value) =>
            {
                sb.WriteStartObject();
                JsonAddPropertyString(sb, "Label", value.Label);
                JsonAddPropertyInt(sb, "SimulationNumber", value.SimulationNumber);
                JsonAddKVPScenarioParameter(sb, "Parameters", value.Parameters);
                sb.WriteEndObject();
            };

            Action<Newtonsoft.Json.JsonTextWriter, string, IEnumerable<Common.ScenarioParameters>> JsonAddScenarioParameters = (sb, name, value) =>
                {
                    sb.WritePropertyName(name);
                    sb.WriteStartArray();
                    value.ToList().ForEach(v => JsonAddScenarioParameter(sb, v));
                    sb.WriteEndArray();
                };

            Action<Newtonsoft.Json.JsonTextWriter, string, Common.EcologicalParameters> JsonAddEcologicalParameters = (sb, name, ecologicalParameters) =>
                {
                    sb.WritePropertyName(name);
                    sb.WriteStartObject();
                    JsonAddKVPDoubles(sb, "Parameters", ecologicalParameters.Parameters);
                    JsonAddPropertyStringArray(sb, "TimeUnits", ecologicalParameters.TimeUnits);
                    sb.WriteEndObject();
                };

            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();
                JsonAddPropertyString(writer, "GlobalModelTimeStepUnit", configuration.GlobalModelTimeStepUnit);
                JsonAddPropertyInt(writer, "NumTimeSteps", configuration.NumTimeSteps);
                JsonAddPropertyInt(writer, "BurninTimeSteps", configuration.BurninTimeSteps);
                JsonAddPropertyInt(writer, "ImpactTimeSteps", configuration.ImpactTimeSteps);
                JsonAddPropertyInt(writer, "RecoveryTimeSteps", configuration.RecoveryTimeSteps);
                JsonAddPropertyBoolean(writer, "RunCellsInParallel", configuration.RunCellsInParallel);
                JsonAddPropertyBoolean(writer, "RunSimulationsInParallel", configuration.RunSimulationsInParallel);
                JsonAddPropertyString(writer, "RunRealm", configuration.RunRealm);
                JsonAddPropertyBoolean(writer, "DrawRandomly", configuration.DrawRandomly);
                JsonAddPropertyDouble(writer, "ExtinctionThreshold", configuration.ExtinctionThreshold);
                JsonAddPropertyInt(writer, "MaxNumberOfCohorts", configuration.MaxNumberOfCohorts);
                JsonAddPropertyBoolean(writer, "DispersalOnly", configuration.DispersalOnly);
                JsonAddPropertyString(writer, "DispersalOnlyType", configuration.DispersalOnlyType);
                JsonAddPropertyDouble(writer, "PlanktonDispersalThreshold", configuration.PlanktonDispersalThreshold);
                JsonAddPropertyFunctionalGroupDefinitions(writer, "CohortFunctionalGroupDefinitions", configuration.CohortFunctionalGroupDefinitions);
                JsonAddPropertyFunctionalGroupDefinitions(writer, "StockFunctionalGroupDefinitions", configuration.StockFunctionalGroupDefinitions);
                JsonAddPropertyIntArray(writer, "ImpactCellIndices", configuration.ImpactCellIndices);
                JsonAddPropertyBoolean(writer, "ImpactAll", configuration.ImpactAll);
                JsonAddScenarioParameters(writer, "ScenarioParameters", configuration.ScenarioParameters);
                JsonAddPropertyInt(writer, "ScenarioIndex", configuration.ScenarioIndex);
                JsonAddPropertyInt(writer, "Simulation", configuration.Simulation);
                JsonAddEcologicalParameters(writer, "EcologicalParameters", configuration.EcologicalParameters);
                JsonAddPropertyStringArray(writer, "FileNames", configuration.FileNames);

                writer.WriteEndObject();
            }
        }

        public static Madingley.Common.Configuration Deserialize(TextReader sr)
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

            Func<Newtonsoft.Json.JsonTextReader, Common.FunctionalGroupDefinition> JsonReadFunctionalGroupDefinition = (reader) =>
            {
                var ret = new Common.FunctionalGroupDefinition();

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
                        case "Definitions": ret.Definitions = JsonReadKVPStrings(reader); break;
                        case "Properties": ret.Properties = JsonReadKVPDoubles(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<Common.FunctionalGroupDefinition>> JsonReadFunctionalGroupDefinitionArray = (reader) =>
            {
                var ret = new List<Common.FunctionalGroupDefinition>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                    ret.Add(JsonReadFunctionalGroupDefinition(reader));
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Common.FunctionalGroupDefinitions> JsonReadFunctionalGroupDefinitions = (reader) =>
                {
                    var ret = new Common.FunctionalGroupDefinitions();

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
                            case "Data": ret.Data = JsonReadFunctionalGroupDefinitionArray(reader); break;
                            case "Definitions": ret.Definitions = JsonReadStringArray(reader); break;
                            case "Properties": ret.Properties = JsonReadStringArray(reader); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }

                    return ret;
                };

            Func<Newtonsoft.Json.JsonTextReader, Common.ScenarioParameter> JsonReadScenarioParameter = (reader) =>
            {
                var ret = new Common.ScenarioParameter();

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
                        case "ParamString": ret.ParamString = JsonReadString(reader); break;
                        case "ParamDouble1": ret.ParamDouble1 = JsonReadDouble(reader); break;
                        case "ParamDouble2": ret.ParamDouble2 = JsonReadDouble(reader); break;
                        default: throw new Exception(string.Format("Unexpected property: {0}", property));
                    }
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, IDictionary<string, Common.ScenarioParameter>> JsonReadKVPScenarioParameter = (reader) =>
            {
                var ret = new Dictionary<string, Common.ScenarioParameter>();

                Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartObject);

                while (reader.Read())
                {
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject) break;

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName);
                    Debug.Assert(reader.ValueType == typeof(string));

                    var key = Convert.ToString(reader.Value);
                    reader.Read();
                    var value = JsonReadScenarioParameter(reader);

                    ret.Add(key, value);
                }

                return ret;
            };

            Func<Newtonsoft.Json.JsonTextReader, Common.ScenarioParameters> JsonReadScenarioParameters = (reader) =>
                {
                    var ret = new Common.ScenarioParameters();

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
                            case "Label": ret.Label = JsonReadString(reader); break;
                            case "SimulationNumber": ret.SimulationNumber = JsonReadInt(reader); break;
                            case "Parameters": ret.Parameters = JsonReadKVPScenarioParameter(reader); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }

                    return ret;
                };

            Func<Newtonsoft.Json.JsonTextReader, IEnumerable<Common.ScenarioParameters>> JsonReadScenarioParametersArray = (reader) =>
                {
                    var ret = new List<Common.ScenarioParameters>();

                    Debug.Assert(reader.TokenType == Newtonsoft.Json.JsonToken.StartArray);

                    while (reader.Read())
                    {
                        if (reader.TokenType == Newtonsoft.Json.JsonToken.EndArray) break;

                        ret.Add(JsonReadScenarioParameters(reader));
                    }

                    return ret;
                };

            Func<Newtonsoft.Json.JsonTextReader, Common.EcologicalParameters> JsonReadEcologicalParameters = (reader) =>
                {
                    var ret = new Common.EcologicalParameters();

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
                            case "Parameters": ret.Parameters = JsonReadKVPDoubles(reader); break;
                            case "TimeUnits": ret.TimeUnits = JsonReadStringArray(reader); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }

                    return ret;
                };

            var configuration = new Madingley.Common.Configuration();

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
                            case "GlobalModelTimeStepUnit": configuration.GlobalModelTimeStepUnit = JsonReadString(reader); break;
                            case "NumTimeSteps": configuration.NumTimeSteps = JsonReadInt(reader); break;
                            case "BurninTimeSteps": configuration.BurninTimeSteps = JsonReadInt(reader); break;
                            case "ImpactTimeSteps": configuration.ImpactTimeSteps = JsonReadInt(reader); break;
                            case "RecoveryTimeSteps": configuration.RecoveryTimeSteps = JsonReadInt(reader); break;
                            case "RunCellsInParallel": configuration.RunCellsInParallel = JsonReadBool(reader); break;
                            case "RunSimulationsInParallel": configuration.RunSimulationsInParallel = JsonReadBool(reader); break;
                            case "RunRealm": configuration.RunRealm = JsonReadString(reader); break;
                            case "DrawRandomly": configuration.DrawRandomly = JsonReadBool(reader); break;
                            case "ExtinctionThreshold": configuration.ExtinctionThreshold = JsonReadDouble(reader); break;
                            case "MaxNumberOfCohorts": configuration.MaxNumberOfCohorts = JsonReadInt(reader); break;
                            case "DispersalOnly": configuration.DispersalOnly = JsonReadBool(reader); break;
                            case "DispersalOnlyType": configuration.DispersalOnlyType = JsonReadString(reader); break;
                            case "PlanktonDispersalThreshold": configuration.PlanktonDispersalThreshold = JsonReadDouble(reader); break;
                            case "CohortFunctionalGroupDefinitions": configuration.CohortFunctionalGroupDefinitions = JsonReadFunctionalGroupDefinitions(reader); break;
                            case "StockFunctionalGroupDefinitions": configuration.StockFunctionalGroupDefinitions = JsonReadFunctionalGroupDefinitions(reader); break;
                            case "ImpactCellIndices": configuration.ImpactCellIndices = JsonReadIntArray(reader); break;
                            case "ImpactAll": configuration.ImpactAll = JsonReadBool(reader); break;
                            case "ScenarioParameters": configuration.ScenarioParameters = JsonReadScenarioParametersArray(reader).ToList(); break;
                            case "ScenarioIndex": configuration.ScenarioIndex = JsonReadInt(reader); break;
                            case "Simulation": configuration.Simulation = JsonReadInt(reader); break;
                            case "EcologicalParameters": configuration.EcologicalParameters = JsonReadEcologicalParameters(reader); break;
                            case "FileNames": configuration.FileNames = JsonReadStringArray(reader).ToList(); break;
                            default: throw new Exception(string.Format("Unexpected property: {0}", property));
                        }
                    }
                }
            }

            return configuration;
        }
    }
}
