using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Madingley.Common;

namespace Madingley
{
    /// <summary>
    /// Reads in and performs look-ups on functional group definitions
    /// </summary>
    /// <remarks>Mass bins values currently defined as middle of each mass bins</remarks>
    /// <todoM>Throw error if there are any blanks in csv file</todoM>
    public static class FunctionalGroupDefinitionsSerialization
    {
        /// <summary>
        /// Reads in the specified functional group definition file 
        /// </summary>
        /// <param name="fileName">The name of the functional group definition file to be read in</param>
        public static FunctionalGroupDefinitions Load(string fileName)
        {
            var data = new List<FunctionalGroupDefinition>();
            var definitionNames = new List<string>();
            var propertyNames = new List<string>();

            using (var reader = new StreamReader(fileName))
            {
                // Discard the header
                var line = reader.ReadLine();
                var headers = line.Split(',');
                definitionNames = headers.Where(header => Regex.IsMatch(header, "DEFINITION_")).Select(header => header.Split('_')[1].ToLower()).ToList();
                propertyNames = headers.Where(header => Regex.IsMatch(header, "PROPERTY_")).Select(header => header.Split('_')[1].ToLower()).ToList();
                var traitNames = headers.Select(header => header.Split('_')[1].ToLower()).ToArray();

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    // Split fields by commas
                    var fields = line.Split(new char[] { ',' }, headers.Length);

                    var definitions = new Dictionary<string, string>();
                    var properties = new Dictionary<string, double>();

                    for (var i = 0; i < fields.Length; i++)
                    {
                        // For functional group definitions
                        if (Regex.IsMatch(headers[i], "DEFINITION_"))
                        {
                            if (fields[i].Length > 0)
                            {
                                definitions[traitNames[i]] = fields[i].ToLower();
                            }
                        }
                        // For functional group properties
                        else if (Regex.IsMatch(headers[i], "PROPERTY_"))
                        {
                            if (fields[i].Length > 0)
                            {
                                properties[traitNames[i]] = Convert.ToDouble(fields[i]);
                            }
                        }
                        else if (Regex.IsMatch(headers[i], "NOTES_"))
                        {
                            // Ignore
                        }
                        // Otherwise, throw an error
                        else
                        {
                            Debug.Fail("All functional group data must be prefixed by DEFINITTION OR PROPERTY");
                        }
                    }

                    data.Add(new FunctionalGroupDefinition(definitions, properties));
                }
            }

            var filteredDefinitionNames = definitionNames.Where(trait => data.Exists(fgd => fgd.Definitions.ContainsKey(trait)));
            var filteredPropertyNames = propertyNames.Where(trait => data.Exists(fgd => fgd.Properties.ContainsKey(trait)));

            return new FunctionalGroupDefinitions(data, filteredDefinitionNames, filteredPropertyNames);
        }
    }
}
