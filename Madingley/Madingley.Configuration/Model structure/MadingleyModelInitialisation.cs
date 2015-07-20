using System;
using System.Collections.Generic;
using Microsoft.Research.Science.Data;

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

            var SimulationFileString = "msds:csv?file=" + simulationInitialisationFileName + "&openMode=readOnly";
            var DefinitionsFileString = "msds:csv?file=" + definitionsFileName + "&openMode=readOnly";

            var i = new Madingley.Common.Configuration();

            i.FileNames.Add(simulationInitialisationFileName);
            i.FileNames.Add(definitionsFileName);

            // Read in the simulation data
            DataSet InternalData = DataSet.Open(SimulationFileString);

            // Get the names of parameters in the initialization file
            var VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            var VarValues = InternalData.Variables[0].GetData();

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "timestep units":
                        i.GlobalModelTimeStepUnit = VarValues.GetValue(row).ToString();
                        break;
                    case "length of simulation (years)":
                        i.NumTimeSteps = (int)Utilities.ConvertTimeUnits("year", i.GlobalModelTimeStepUnit) * Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "burn-in (years)":
                        i.BurninTimeSteps = (int)Utilities.ConvertTimeUnits("year", i.GlobalModelTimeStepUnit) * Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "impact duration (years)":
                        i.ImpactTimeSteps = (int)Utilities.ConvertTimeUnits("year", i.GlobalModelTimeStepUnit) * Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "recovery duration (years)":
                        i.RecoveryTimeSteps = (int)Utilities.ConvertTimeUnits("year", i.GlobalModelTimeStepUnit) * Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "number timesteps":
                        i.NumTimeSteps = Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "run cells in parallel":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                i.RunCellsInParallel = true;
                                break;
                            case "no":
                                i.RunCellsInParallel = false;
                                break;
                        }
                        break;
                    case "run simulations in parallel":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                i.RunSimulationsInParallel = true;
                                break;
                            case "no":
                                i.RunSimulationsInParallel = false;
                                break;
                        }
                        break;
                    case "run single realm":
                        i.RunRealm = VarValues.GetValue(row).ToString().ToLower();
                        break;
                    case "draw randomly":

                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                i.DrawRandomly = true;
                                break;
                            case "no":
                                i.DrawRandomly = false;
                                break;
                        }
                        break;
                    case "extinction threshold":
                        i.ExtinctionThreshold = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "maximum number of cohorts":
                        i.MaxNumberOfCohorts = Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "impact cell index":
                        if (VarValues.GetValue(row).ToString() != "")
                        {
                            var impactCellIndices = new List<int>();

                            string[] temp = VarValues.GetValue(row).ToString().Split(new char[] { ';' });
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

                            i.ImpactCellIndices = impactCellIndices;
                        }
                        break;
                    case "dispersal only":
                        if (VarValues.GetValue(row).ToString() == "yes")
                            i.DispersalOnly = true;
                        else i.DispersalOnly = false;
                        break;
                    case "dispersal only type":
                        i.DispersalOnlyType = VarValues.GetValue(row).ToString();
                        break;
                    case "plankton size threshold":
                        i.PlanktonDispersalThreshold = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                }
            }

            InternalData.Dispose();

            // Read in the definitions data
            InternalData = DataSet.Open(DefinitionsFileString);

            // Get the names of parameters in the initialization file
            VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            VarValues = InternalData.Variables[0].GetData();

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "cohort functional group definitions file":
                        {
                            Console.WriteLine("Reading functional group definitions...\n");
                            // Open a the specified csv file and set up the cohort functional group definitions
                            var functionalDefinitionsFileName = VarValues.GetValue(row).ToString();
                            var fileName = System.IO.Path.Combine(inputPath, "Ecological Definition Files", functionalDefinitionsFileName);
                            i.FileNames.Add(fileName);

                            i.CohortFunctionalGroupDefinitions = FunctionalGroupDefinitionsSerialization.Load(fileName);
                        }
                        break;
                    case "stock functional group definitions file":
                        {
                            // Open a the specified csv file and set up the stock functional group definitions
                            var functionalDefinitionsFileName = VarValues.GetValue(row).ToString();
                            var fileName = System.IO.Path.Combine(inputPath, "Ecological Definition Files", functionalDefinitionsFileName);
                            i.FileNames.Add(fileName);

                            i.StockFunctionalGroupDefinitions = FunctionalGroupDefinitionsSerialization.Load(fileName);
                        }
                        break;
                    case "ecological parameters file":
                        {
                            var parametersFileName = VarValues.GetValue(row).ToString();
                            var fileName = System.IO.Path.Combine(inputPath, "Ecological Definition Files", parametersFileName);
                            i.FileNames.Add(fileName);

                            i.EcologicalParameters = EcologicalParameters.Load(fileName);
                        }
                        break;
                }
            }

            InternalData.Dispose();

            return i;
        }
    }
}
