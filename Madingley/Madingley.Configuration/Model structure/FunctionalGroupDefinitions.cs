using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Research.Science.Data;

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
        /// <param name="outputPath">The path to the output folder, in which to copy the functional group definitions file</param>
        /// <param name="inputPath">The path to folder which contains the inputs</param>
        public static FunctionalGroupDefinitions Load(string fileName, string inputPath)
        {
            // Construct the URI for the functional group definition file
            string FileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, "Ecological Definition Files", fileName) + "&openMode=readOnly";

            var InternalData = (DataSet)null;

#if false
            // Construct the URI for the functional group definition file
            string FileString = "msds:csv?file=input/Model setup/ecological definition files/" + fileName + "&openMode=readOnly";

            // Copy the Function group definitions file to the output directory
            System.IO.File.Copy("input/Model setup/ecological definition files/" + fileName, outputPath + fileName, true);
#endif

            // Read in the data
            InternalData = DataSet.Open(FileString);

            // Initialise the lists
            var TraitLookupFromIndex = new SortedDictionary<string, string[]>();
            var FunctionalGroupProperties = new SortedList<string, double[]>();

            // Loop over columns in the functional group definitions file
            foreach (Variable v in InternalData.Variables)
            {
                // Get the column header
                string TraitName = v.Name.Split('_')[1].ToLower();
                // Get the values in this column
                var TempValues = v.GetData();

                // For functional group definitions
                if (System.Text.RegularExpressions.Regex.IsMatch(v.Name, "DEFINITION_"))
                {
                    // Declare a sorted dictionary to hold the index values for each unique trait value
                    SortedDictionary<string, int[]> TraitIndexValuesList = new SortedDictionary<string, int[]>();
                    // Create a string array with the values of this trait
                    string[] TempString = new string[TempValues.Length];
                    for (int nn = 0; nn < TempValues.Length; nn++)
                    {
                        TempString[nn] = TempValues.GetValue(nn).ToString().ToLower();
                    }
                    // Add the trait values to the trait-value lookup list
                    TraitLookupFromIndex.Add(TraitName, TempString);

                    // Get the unique values for this trait
                    var DistinctValues = TempString.Distinct().ToArray();
                    //Loop over the unique values for this trait and list all the functional group indices with the value
                    foreach (string DistinctTraitValue in DistinctValues.ToArray())
                    {
                        List<int> FunctionalGroupIndex = new List<int>();
                        //Loop over the string array associated with this trait and add the index values of matching string to a list
                        for (int kk = 0; kk < TempString.Length; kk++)
                        {
                            if (TempString[kk].Equals(DistinctTraitValue))
                            {
                                FunctionalGroupIndex.Add(kk);
                            }
                        }
                        //Add the unique trait value and the functional group indices to the temporary list
                        TraitIndexValuesList.Add(DistinctTraitValue, FunctionalGroupIndex.ToArray());
                    }
                }
                // For functional group properties
                else if (System.Text.RegularExpressions.Regex.IsMatch(v.Name, "PROPERTY_"))
                {
                    // Get the values for this property
                    double[] TempDouble = new double[TempValues.Length];
                    for (int nn = 0; nn < TempValues.Length; nn++)
                    {
                        TempDouble[nn] = Convert.ToDouble(TempValues.GetValue(nn));
                    }
                    // Add the values to the list of functional group properties
                    FunctionalGroupProperties.Add(TraitName, TempDouble);
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(v.Name, "NOTES_"))
                {
                    // Ignore
                }
                // Otherwise, throw an error
                else
                {
                    Debug.Fail("All functional group data must be prefixed by DEFINITTION OR PROPERTY");
                }


            }

            var length = TraitLookupFromIndex.Max(p => p.Value.Length);

            var data = new FunctionalGroupDefinition[length];

            for (var i = 0; i < length; i++)
            {
                var definitions = TraitLookupFromIndex.ToDictionary(kv => kv.Key, kv => kv.Value[i]);
                var properties = FunctionalGroupProperties.ToDictionary(kv => kv.Key, kv => kv.Value[i]);

                data[i] = new FunctionalGroupDefinition(definitions, properties);
            }

            var definitionNames = TraitLookupFromIndex.Select(kv => kv.Key).Distinct();

            var propertyNames = FunctionalGroupProperties.Select(kv => kv.Key).Distinct();

            return new FunctionalGroupDefinitions(data, definitionNames, propertyNames);
        }
    }
}
