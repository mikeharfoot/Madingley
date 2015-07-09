using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Research.Science.Data;


namespace Madingley
{
    /// <summary>
    /// Initialization information for Madingley model simulations
    /// </summary>
    public class MadingleyModelInitialisation
    {
        /// <summary>
        /// String identifying time step units to be used by the simulations
        /// </summary>
        private string _GlobalModelTimeStepUnit;
        /// <summary>
        /// Get and set the string identifying time step units to be used by the simulations
        /// </summary>
        public string GlobalModelTimeStepUnit
        {
            get { return _GlobalModelTimeStepUnit; }
            set { _GlobalModelTimeStepUnit = value; }
        }

        /// <summary>
        /// The number of time steps to be run in the simulations
        /// </summary>
        private uint _NumTimeSteps;
        /// <summary>
        /// Get and set the number of time steps to be run in the simulations
        /// </summary>
        public uint NumTimeSteps
        {
            get { return _NumTimeSteps; }
            set { _NumTimeSteps = value; }
        }

        /// <summary>
        /// The number of time steps to run the model for before any impacts are applied
        /// </summary>
        private uint _BurninTimeSteps;
        /// <summary>
        /// Get and set the number of time steps to run the model for before any impacts are applied
        /// </summary>
        public uint BurninTimeSteps
        {
            get { return _BurninTimeSteps; }
            set { _BurninTimeSteps = value; }
        }

        /// <summary>
        /// For scenarios with temporary impacts, the number of time steps to apply the impact for
        /// </summary>
        private uint _ImpactTimeSteps;

        /// <summary>
        /// Get and set the number of time steps to apply the impact for, for scenarios with temporary impacts
        /// </summary>
        public uint ImpactTimeSteps
        {
            get { return _ImpactTimeSteps; }
            set { _ImpactTimeSteps = value; }
        }


        /// <summary>
        /// For scenarios with temporary impacts, the number of time steps to apply the impact for
        /// </summary>
        private uint _RecoveryTimeSteps;

        /// <summary>
        /// Get and set the number of time steps to apply the impact for, for scenarios with temporary impacts
        /// </summary>
        public uint RecoveryTimeSteps
        {
            get { return _RecoveryTimeSteps; }
            set { _RecoveryTimeSteps = value; }
        }

        /// <summary>
        /// Whether to run the model for different grid cells in parallel
        /// </summary>
        private Boolean _RunCellsInParallel = false;
        /// <summary>
        /// Get and set whether to run the model for different grid cells in parallel
        /// </summary>
        public Boolean RunCellsInParallel
        {
            get { return _RunCellsInParallel; }
            set { _RunCellsInParallel = value; }
        }

        /// <summary>
        /// Whether to run the model for different simulations in parallel
        /// </summary>
        private Boolean _RunSimulationsInParallel = false;
        /// <summary>
        /// Get and set whether to run the model for different grid cells in parallel
        /// </summary>
        public Boolean RunSimulationsInParallel
        {
            get { return _RunSimulationsInParallel; }
            set { _RunSimulationsInParallel = value; }
        }

        /// <summary>
        /// Which realm to run the model for
        /// </summary>
        private string _RunRealm;
        /// <summary>
        /// Get and set which realm to run the model for
        /// </summary>
        public string RunRealm
        {
            get { return _RunRealm; }
            set { _RunRealm = value; }
        }


        /// <summary>
        /// Whether to draw cohort properties randomly when seeding them, and whether cohorts will undergo ecological processes in a random order
        /// </summary>
        /// <remarks>Value should be set in initialization file, but default value is true</remarks>
        private Boolean _DrawRandomly = true;
        /// <summary>
        /// Get and set whether to draw cohort properties randomly when seeding them, and whether cohorts will undergo ecological processes in a random order
        /// </summary>
        public Boolean DrawRandomly
        {
            get { return _DrawRandomly; }
            set { _DrawRandomly = value; }
        }

        /// <summary>
        /// The threshold abundance below which cohorts will be made extinct
        /// </summary>
        private double _ExtinctionThreshold;
        /// <summary>
        /// Get and set the threshold abundance below which cohorts will be made extinct
        /// </summary>
        public double ExtinctionThreshold
        {
            get { return _ExtinctionThreshold; }
            set { _ExtinctionThreshold = value; }
        }

        /// <summary>
        /// The maximum number of cohorts to be in the model, per grid cell, when it is running
        /// </summary>
        private int _MaxNumberOfCohorts;

        /// <summary>
        ///  Get and set the maximum number of cohorts per grid cell
        /// </summary>
        public int MaxNumberOfCohorts
        {
            get { return _MaxNumberOfCohorts; }
            set { _MaxNumberOfCohorts = value; }
        }


        /// <summary>
        /// Whether to run only dispersal (i.e. turn all other ecological processes off, and set dispersal probability to one temporarily)
        /// </summary>
        private Boolean _DispersalOnly = false;
        /// <summary>
        /// Get and set whether to run dispersal only
        /// </summary>
        public Boolean DispersalOnly
        {
            get { return _DispersalOnly; }
            set { _DispersalOnly = value; }
        }

        /// <summary>
        /// The weight threshold (grams) below which marine organisms that are not obligate zooplankton will be dispersed planktonically
        /// </summary>
        private double _PlanktonDispersalThreshold;
        /// <summary>
        /// Get and set the weight threshold (grams) below which marine organisms that are not obligate zooplankton will be dispersed planktonically
        /// </summary>
        public double PlanktonDispersalThreshold
        {
            get { return _PlanktonDispersalThreshold; }
            set { _PlanktonDispersalThreshold = value; }
        }

        /// <summary>
        /// Information from the initialization file
        /// </summary>
        private SortedList<string, string> _InitialisationFileStrings = new SortedList<string, string>();
        /// <summary>
        /// Get and set information from the initialization file
        /// </summary>
        public SortedList<string, string> InitialisationFileStrings
        {
            get { return _InitialisationFileStrings; }
            set { _InitialisationFileStrings = value; }
        }

        /// <summary>
        /// The functional group definitions of cohorts in the model
        /// </summary>
        private FunctionalGroupDefinitions _CohortFunctionalGroupDefinitions;
        /// <summary>
        /// Get and set the functional group definitions of cohorts in the model
        /// </summary>
        public FunctionalGroupDefinitions CohortFunctionalGroupDefinitions
        {
            get { return _CohortFunctionalGroupDefinitions; }
            set { _CohortFunctionalGroupDefinitions = value; }
        }

        /// <summary>
        /// The functional group definitions of stocks in the model
        /// </summary>
        private FunctionalGroupDefinitions _StockFunctionalGroupDefinitions;
        /// <summary>
        /// Get and set the functional group definitions of stocks in the model
        /// </summary>
        public FunctionalGroupDefinitions StockFunctionalGroupDefinitions
        {
            get { return _StockFunctionalGroupDefinitions; }
            set { _StockFunctionalGroupDefinitions = value; }
        }

        private List<uint> _ImpactCellIndices = new List<uint>();

        public List<uint> ImpactCellIndices
        {
            get { return _ImpactCellIndices; }
            set { _ImpactCellIndices = value; }
        }

        private Boolean _ImpactAll = false;

        public Boolean ImpactAll
        {
            get { return _ImpactAll; }
            set { _ImpactAll = value; }
        }

#if true
        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="simulationInitialisationFilename">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="definitionsFilename">Definitions file name</param>
        /// <param name="outputsFilename">Outputs file name</param>
        /// <param name="outputPath">Path to output files</param>
        /// <param name="inputPath">The path to folder which contains the inputs</param>
        public MadingleyModelInitialisation(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath, string inputPath)
#else
        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        public MadingleyModelInitialisation(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath)
#endif
        {
            // Read the intialisation files and copy them to the output directory
#if true
            ReadAndCopyInitialisationFiles(simulationInitialisationFilename, definitionsFilename, outputsFilename, outputPath, inputPath);
#else
            ReadAndCopyInitialisationFiles(simulationInitialisationFilename, definitionsFilename, outputsFilename, outputPath);
#endif
        }

#if true
        /// <summary>
        /// Reads in all initialisation files and copies them to the output directory for future reference
        /// </summary>
        /// <param name="simulationInitialisationFilename">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="definitionsFilename">Definitions file name</param>
        /// <param name="outputsFilename">Outputs file name</param>
        /// <param name="outputPath">Path to output files</param>
        /// <param name="inputPath">The path to folder which contains the inputs</param>
        public void ReadAndCopyInitialisationFiles(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath, string inputPath)
        {
            // Construct file names
            var SimulationFileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, simulationInitialisationFilename) + "&openMode=readOnly";
            var DefinitionsFileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, definitionsFilename) + "&openMode=readOnly";
#else
        /// <summary>
        /// Reads in all initialisation files and copies them to the output directory for future reference
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        /// <todo>Need to adjust this file to deal with incorrect inputs, extra columns etc by throwing an error</todo>
        /// <todo>Also need to strip leading spaces</todo>
        public void ReadAndCopyInitialisationFiles(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath)
        {
            // Construct file names
            string SimulationFileString = "msds:csv?file=input/Model setup/" + simulationInitialisationFilename + "&openMode=readOnly";
            string DefinitionsFileString = "msds:csv?file=input/Model setup/" + definitionsFilename + "&openMode=readOnly";
            string OutputsFileString = "msds:csv?file=input/Model setup/" + outputsFilename + "&openMode=readOnly";

            // Copy the initialisation files to the output directory
            System.IO.File.Copy("input/Model setup/" + simulationInitialisationFilename, outputPath + simulationInitialisationFilename, true);
            System.IO.File.Copy("input/Model setup/" + definitionsFilename, outputPath + definitionsFilename, true);
            System.IO.File.Copy("input/Model setup/" + outputsFilename, outputPath + outputsFilename, true);
#endif

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
                        _GlobalModelTimeStepUnit = VarValues.GetValue(row).ToString();
                        break;
                    case "length of simulation (years)":
                        _NumTimeSteps = (uint)Utilities.ConvertTimeUnits("year", _GlobalModelTimeStepUnit) * Convert.ToUInt32(VarValues.GetValue(row));
                        break;
                    case "burn-in (years)":
                        _BurninTimeSteps = (uint)Utilities.ConvertTimeUnits("year", _GlobalModelTimeStepUnit) * Convert.ToUInt32(VarValues.GetValue(row));
                        break;
                    case "impact duration (years)":
                        _ImpactTimeSteps = (uint)Utilities.ConvertTimeUnits("year", _GlobalModelTimeStepUnit) * Convert.ToUInt32(VarValues.GetValue(row));
                        break;
                    case "recovery duration (years)":
                        _RecoveryTimeSteps = (uint)Utilities.ConvertTimeUnits("year", _GlobalModelTimeStepUnit) * Convert.ToUInt32(VarValues.GetValue(row));
                        break;
                    case "number timesteps":
                        _NumTimeSteps = Convert.ToUInt32(VarValues.GetValue(row));
                        break;
                    case "run cells in parallel":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                _RunCellsInParallel = true;
                                break;
                            case "no":
                                _RunCellsInParallel = false;
                                break;
                        }
                        break;
                    case "run simulations in parallel":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                _RunSimulationsInParallel = true;
                                break;
                            case "no":
                                _RunSimulationsInParallel = false;
                                break;
                        }
                        break;
                    case "run single realm":
                        _RunRealm = VarValues.GetValue(row).ToString().ToLower();
                        break;
                    case "draw randomly":

                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                _DrawRandomly = true;
                                break;
                            case "no":
                                _DrawRandomly = false;
                                break;
                        }
                        break;
                    case "extinction threshold":
                        _ExtinctionThreshold = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "maximum number of cohorts":
                        _MaxNumberOfCohorts = Convert.ToInt32(VarValues.GetValue(row));
                        break;
                    case "impact cell index":
                        if (VarValues.GetValue(row).ToString() != "")
                        {
                            string[] temp = VarValues.GetValue(row).ToString().Split(new char[] { ';' });
                            foreach (string t in temp)
                            {
                                if (t.Split(new char[] { '-' }).Length > 1)
                                {
                                    string[] range = t.Split(new char[] { '-' });
                                    for (uint i = Convert.ToUInt32(range[0]); i <= Convert.ToUInt32(range[1]); i++)
                                    {
                                        _ImpactCellIndices.Add(i);
                                    }
                                }
                                else
                                {
                                    _ImpactCellIndices.Add(Convert.ToUInt32(Convert.ToInt32(t)));
                                }

                            }
                        }
                        break;
                    case "dispersal only":
                        if (VarValues.GetValue(row).ToString() == "yes")
                            _DispersalOnly = true;
                        else _DispersalOnly = false;
                        break;
                    case "dispersal only type":
                        _InitialisationFileStrings.Add("DispersalOnlyType", VarValues.GetValue(row).ToString());
                        break;
                    case "plankton size threshold":
                        _PlanktonDispersalThreshold = Convert.ToDouble(VarValues.GetValue(row));
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
                        Console.WriteLine("Reading functional group definitions...\n");
                        _InitialisationFileStrings.Add("CohortFunctional", VarValues.GetValue(row).ToString());
                        // Open a the specified csv file and set up the cohort functional group definitions
#if true
                        _CohortFunctionalGroupDefinitions = new FunctionalGroupDefinitions(VarValues.GetValue(row).ToString(), outputPath, inputPath);
#else
                        _CohortFunctionalGroupDefinitions = new FunctionalGroupDefinitions(VarValues.GetValue(row).ToString(), outputPath);
#endif
                        break;
                    case "stock functional group definitions file":
                        _InitialisationFileStrings.Add("StockFunctional", VarValues.GetValue(row).ToString());
                        // Open a the specified csv file and set up the stock functional group definitions
#if true
                        _StockFunctionalGroupDefinitions = new FunctionalGroupDefinitions(VarValues.GetValue(row).ToString(), outputPath, inputPath);
#else
                        _StockFunctionalGroupDefinitions = new FunctionalGroupDefinitions(VarValues.GetValue(row).ToString(), outputPath);
#endif
                        break;
                    case "ecological parameters file":
#if true
                        EcologicalParameters.ReadEcologicalParameters(VarValues.GetValue(row).ToString(), outputPath, inputPath);
#else
                        EcologicalParameters.ReadEcologicalParameters(VarValues.GetValue(row).ToString(), outputPath);
#endif
                        break;
                }
            }

            InternalData.Dispose();

        }
    }



}
