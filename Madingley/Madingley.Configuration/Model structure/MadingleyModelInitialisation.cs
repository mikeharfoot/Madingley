using System;
using System.Collections.Generic;
using System.IO;

namespace Madingley
{
    /// <summary>
    /// Initialization information for Madingley model simulations
    /// </summary>
    public static class MadingleyModelInitialisation
    {
        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="simulationInitialisationFilename">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="definitionsFilename">Definitions file name</param>
        /// <param name="inputPath">The path to folder which contains the inputs</param>
        public static Madingley.Common.Configuration Load(string simulationInitialisationFilename, string definitionsFilename, string inputPath)
        {
            // Read the intialisation files and copy them to the output directory

            // Construct file names
            var simulationInitialisationFileName = System.IO.Path.Combine(inputPath, simulationInitialisationFilename);
            var definitionsFileName = System.IO.Path.Combine(inputPath, definitionsFilename);

            var configuration = new Madingley.Common.Configuration();

            configuration.FileNames.Add(simulationInitialisationFileName);
            configuration.FileNames.Add(definitionsFileName);

            using (var reader = new StreamReader(simulationInitialisationFileName))
            {
                // Discard the header
                var line = reader.ReadLine();
                var headers = line.Split(new char[] { ',' }, 2);

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    // Split fields by commas
                    var fields = line.Split(new char[] { ',' }, 2);

                    // Switch based on the name of the parameter, and write the value to the appropriate field
                    switch (fields[0].ToLower())
                    {
                        case "timestep units":
                            configuration.GlobalModelTimeStepUnit = fields[1];
                            break;
                        case "length of simulation (years)":
                            configuration.NumTimeSteps = (int)Utilities.ConvertTimeUnits("year", configuration.GlobalModelTimeStepUnit) * Convert.ToInt32(fields[1]);
                            break;
                        case "burn-in (years)":
                            configuration.BurninTimeSteps = (int)Utilities.ConvertTimeUnits("year", configuration.GlobalModelTimeStepUnit) * Convert.ToInt32(fields[1]);
                            break;
                        case "impact duration (years)":
                            configuration.ImpactTimeSteps = (int)Utilities.ConvertTimeUnits("year", configuration.GlobalModelTimeStepUnit) * Convert.ToInt32(fields[1]);
                            break;
                        case "recovery duration (years)":
                            configuration.RecoveryTimeSteps = (int)Utilities.ConvertTimeUnits("year", configuration.GlobalModelTimeStepUnit) * Convert.ToInt32(fields[1]);
                            break;
                        case "number timesteps":
                            configuration.NumTimeSteps = Convert.ToInt32(fields[1]);
                            break;
                        case "run cells in parallel":
                            switch (fields[1].ToLower())
                            {
                                case "yes":
                                    configuration.RunCellsInParallel = true;
                                    break;
                                case "no":
                                    configuration.RunCellsInParallel = false;
                                    break;
                            }
                            break;
                        case "run simulations in parallel":
                            switch (fields[1].ToLower())
                            {
                                case "yes":
                                    configuration.RunSimulationsInParallel = true;
                                    break;
                                case "no":
                                    configuration.RunSimulationsInParallel = false;
                                    break;
                            }
                            break;
                        case "run single realm":
                            configuration.RunRealm = fields[1].ToLower();
                            break;
                        case "draw randomly":

                            switch (fields[1].ToLower())
                            {
                                case "yes":
                                    configuration.DrawRandomly = true;
                                    break;
                                case "no":
                                    configuration.DrawRandomly = false;
                                    break;
                            }
                            break;
                        case "extinction threshold":
                            configuration.ExtinctionThreshold = Convert.ToDouble(fields[1]);
                            break;
                        case "maximum number of cohorts":
                            configuration.MaxNumberOfCohorts = Convert.ToInt32(fields[1]);
                            break;
                        case "impact cell index":
                            if (fields[1] != "")
                            {
                                var impactCellIndices = new List<int>();

                                string[] temp = fields[1].Split(new char[] { ';' });
                                foreach (string t in temp)
                                {
                                    if (t.Split(new char[] { '-' }).Length > 1)
                                    {
                                        string[] range = t.Split(new char[] { '-' });
                                        for (int ii = Convert.ToInt32(range[0]); ii <= Convert.ToInt32(range[1]); ii++)
                                        {
                                            impactCellIndices.Add(ii);
                                        }
                                    }
                                    else
                                    {
                                        impactCellIndices.Add(Convert.ToInt32(t));
                                    }

                                }

                                configuration.ImpactCellIndices = impactCellIndices;
                            }
                            break;
                        case "dispersal only":
                            if (fields[1] == "yes")
                                configuration.DispersalOnly = true;
                            else configuration.DispersalOnly = false;
                            break;
                        case "dispersal only type":
                            configuration.DispersalOnlyType = fields[1];
                            break;
                        case "plankton size threshold":
                            configuration.PlanktonDispersalThreshold = Convert.ToDouble(fields[1]);
                            break;
                    }
                }
            }

            // Read in the definitions data
            using (var reader = new StreamReader(definitionsFileName))
            {
                // Discard the header
                var line = reader.ReadLine();
                var headers = line.Split(new char[] { ',' }, 2);

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    // Split fields by commas
                    var fields = line.Split(new char[] { ',' }, 2);

                    // Switch based on the name of the parameter, and write the value to the appropriate field
                    switch (fields[0].ToLower())
                    {
                        case "cohort functional group definitions file":
                            {
                                Console.WriteLine("Reading functional group definitions...\n");
                                // Open a the specified csv file and set up the cohort functional group definitions
                                var functionalDefinitionsFileName = fields[1];
                                var fileName = System.IO.Path.Combine(inputPath, "Ecological Definition Files", functionalDefinitionsFileName);
                                configuration.FileNames.Add(fileName);

                                configuration.CohortFunctionalGroupDefinitions = FunctionalGroupDefinitionsSerialization.Load(fileName);
                            }
                            break;
                        case "stock functional group definitions file":
                            {
                                // Open a the specified csv file and set up the stock functional group definitions
                                var functionalDefinitionsFileName = fields[1];
                                var fileName = System.IO.Path.Combine(inputPath, "Ecological Definition Files", functionalDefinitionsFileName);
                                configuration.FileNames.Add(fileName);

                                configuration.StockFunctionalGroupDefinitions = FunctionalGroupDefinitionsSerialization.Load(fileName);
                            }
                            break;
                        case "ecological parameters file":
                            {
                                var parametersFileName = fields[1];
                                var fileName = System.IO.Path.Combine(inputPath, "Ecological Definition Files", parametersFileName);
                                configuration.FileNames.Add(fileName);

                                configuration.EcologicalParameters = EcologicalParameters.Load(fileName);
                            }
                            break;
                    }
                }
            }

            return configuration;
        }
    }
}
