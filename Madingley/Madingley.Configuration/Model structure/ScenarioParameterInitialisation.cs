using System;
using System.Collections.Generic;
using System.IO;

using Madingley.Common;

namespace Madingley
{
    /// <summary>
    /// Reads the file specifying which scenarios will be run, and stores this information
    /// </summary>
    public static class ScenarioParameterInitialisation
    {
        /// <summary>
        /// Reads in scenario parameters from a specified file
        /// </summary>
        /// <param name="fileName">The name of the scenario parameters file, which must be in the 'Model setup' directory</param>
        public static IList<ScenarioParameters> Load(string fileName)
        {
            Console.WriteLine("Reading scenario parameters file...\n");

            var scenarioParameters = new List<ScenarioParameters>();

            // Read in the data
            using (var reader = new StreamReader(fileName))
            {
                // Discard the header
                var line = reader.ReadLine();
                var headers = line.Split(',');

                var labelIndex = Array.FindIndex(headers, item => item.ToLower() == "label");
                var nppIndex = Array.FindIndex(headers, item => item.ToLower() == "npp");
                var temperatureIndex = Array.FindIndex(headers, item => item.ToLower() == "temperature");
                var harvestingIndex = Array.FindIndex(headers, item => item.ToLower() == "harvesting");
                var simulationNumberIndex = Array.FindIndex(headers, item => item.ToLower() == "simulation number");

                while (!reader.EndOfStream)
                {
                    var parameters = new Dictionary<string, ScenarioParameter>();

                    line = reader.ReadLine();
                    // Split fields by commas
                    var fields = line.Split(new char[] { ',' }, headers.Length);

                    var pair = fields[nppIndex].Split(' ');

                    if (pair.Length > 2)
                    {
                        parameters.Add("npp", new ScenarioParameter(pair[0], Convert.ToDouble(pair[1]), Convert.ToDouble(pair[2])));
                    }
                    else
                    {
                        parameters.Add("npp", new ScenarioParameter(pair[0], Convert.ToDouble(pair[1]), -999));
                    }

                    pair = fields[temperatureIndex].Split(' ');

                    if (pair.Length > 2)
                    {
                        parameters.Add("temperature", new ScenarioParameter(pair[0], Convert.ToDouble(pair[1]), Convert.ToDouble(pair[2])));
                    }
                    else
                    {
                        parameters.Add("temperature", new ScenarioParameter(pair[0], Convert.ToDouble(pair[1]), -999));
                    }

                    pair = fields[harvestingIndex].Split(' ');
                    if (pair.Length > 2)
                    {
                        parameters.Add("harvesting", new ScenarioParameter(pair[0], Convert.ToDouble(pair[1]), Convert.ToDouble(pair[2])));
                    }
                    else
                    {
                        parameters.Add("harvesting", new ScenarioParameter(pair[0], Convert.ToDouble(pair[1]), -999));
                    }

                    var simulationNumber = Convert.ToInt32(fields[simulationNumberIndex]);

                    scenarioParameters.Add(new ScenarioParameters(fields[labelIndex], simulationNumber, parameters));
                }
            }

            return scenarioParameters;
        }
    }
}
