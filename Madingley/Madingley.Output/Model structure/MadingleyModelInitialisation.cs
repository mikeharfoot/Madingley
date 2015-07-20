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
        /// The size of cells to be used in the model grid
        /// </summary>
        private double _CellSize;
        /// <summary>
        /// Get and set the size of cells to be used in the model grid
        /// </summary>
        public double CellSize
        {
            get { return _CellSize; }
            set { _CellSize = value; }
        }

        /// <summary>
        /// The lowest extent of the model grid in degrees
        /// </summary>
        private float _BottomLatitude;
        /// <summary>
        /// Get and set the lowest extent of the model grid in degrees
        /// </summary>
        public float BottomLatitude
        {
            get { return _BottomLatitude; }
            set { _BottomLatitude = value; }
        }

        /// <summary>
        /// The uppermost extent of the model grid in degrees
        /// </summary>
        private float _TopLatitude;
        /// <summary>
        /// Get and set the uppermost extent of the model grid in degrees
        /// </summary>
        public float TopLatitude
        {
            get { return _TopLatitude; }
            set { _TopLatitude = value; }
        }

        /// <summary>
        /// The leftmost extent of the model grid in degrees
        /// </summary>
        private float _LeftmostLongitude;
        /// <summary>
        /// Get and set the leftmost extent of the model grid in degrees
        /// </summary>
        public float LeftmostLongitude
        {
            get { return _LeftmostLongitude; }
            set { _LeftmostLongitude = value; }
        }

        /// <summary>
        /// The rightmost extent of the model grid in degrees
        /// </summary>
        private float _RightmostLongitude;
        /// <summary>
        /// Get and set the rightmost extent of the model grid in degrees
        /// </summary>
        public float RightmostLongitude
        {
            get { return _RightmostLongitude; }
            set { _RightmostLongitude = value; }
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

#if true
        public SortedList<string, double[]>[] EnviroStack { get; set; }
#else
        /// <summary>
        /// The environmental layers for use in the model
        /// </summary>
        private SortedList<string, EnviroData> _EnviroStack = new SortedList<string, EnviroData>();
        /// <summary>
        /// Get and set the environmental layers for use in the model
        /// </summary>
        public SortedList<string, EnviroData> EnviroStack
        {
            get { return _EnviroStack; }
            set { _EnviroStack = value; }
        }
#endif

        /// <summary>
        /// The full path for the output files for a set of simulations
        /// </summary>
        private string _OutputPath;
        /// <summary>
        /// Get and set the full path for the output files for a set of simulations
        /// </summary>
        public string OutputPath
        {
            get { return _OutputPath; }
            set { _OutputPath = value; }
        }

        /// <summary>
        /// Whether to output detailed diagnostics for the ecological processes
        /// </summary>
        private Boolean _TrackProcesses = false;
        /// <summary>
        /// Get and set whether to output detailed diagnostics for the ecological processes
        /// </summary>
        public Boolean TrackProcesses
        {
            get { return _TrackProcesses; }
            set { _TrackProcesses = value; }
        }

        /// <summary>
        /// Whether to output detailed diagnostics for the cross cell ecological processes
        /// </summary>
        private Boolean _TrackCrossCellProcesses = false;
        /// <summary>
        /// Get and set whether to output detailed diagnostics for the cross cell ecological processes
        /// </summary>
        public Boolean TrackCrossCellProcesses
        {
            get { return _TrackCrossCellProcesses; }
            set { _TrackCrossCellProcesses = value; }
        }

        /// <summary>
        /// Whether to output detailed diagnostics for the ecological processes
        /// </summary>
        private Boolean _TrackGlobalProcesses = false;
        /// <summary>
        /// Get and set whether to output detailed diagnostics for the ecological processes
        /// </summary>
        public Boolean TrackGlobalProcesses
        {
            get { return _TrackGlobalProcesses; }
            set { _TrackGlobalProcesses = value; }
        }

        /// <summary>
        /// The paths and filenames for the diagnostics for the ecological processes
        /// </summary>
        private SortedList<string, string> _ProcessTrackingOutputs = new SortedList<string, string>();
        /// <summary>
        /// Get and set the paths and filenames for the diagnostics for the ecological processes
        /// </summary>
        public SortedList<string, string> ProcessTrackingOutputs
        {
            get { return _ProcessTrackingOutputs; }
            set { _ProcessTrackingOutputs = value; }
        }

        /// <summary>
        /// An instance of the mass bin handler for the current model run
        /// </summary>
        private MassBinsHandler _ModelMassBins;
        /// <summary>
        /// Get the instance of the mass bin handler for the current model run
        /// </summary>
#if true
        public MassBinsHandler ModelMassBins
        {
            get { return _ModelMassBins; }
            set { _ModelMassBins = value; }
        }
#else
        public MassBinsHandler ModelMassBins
        { get { return _ModelMassBins; } }
#endif

        /// <summary>
        /// Whether to display live outputs using Dataset Viewer during the model runs
        /// </summary>
        private Boolean _LiveOutputs;

        /// <summary>
        /// Get and set whether to display live outputs using Dataset Viewer during the model runs
        /// </summary>
        public Boolean LiveOutputs
        {
            get { return _LiveOutputs; }
            set { _LiveOutputs = value; }
        }

        /// <summary>
        /// Whether or not to track trophic level biomass and flow information specific to the marine realm
        /// </summary>
        private Boolean _TrackMarineSpecifics;
        /// <summary>
        /// Get and set whether or not to track trophic level biomass and flow information specific to the marine realm
        /// </summary>
        public Boolean TrackMarineSpecifics
        {
            get { return _TrackMarineSpecifics; }
            set { _TrackMarineSpecifics = value; }
        }

        /// <summary>
        /// Whether to output ecosystem-level functional metrics
        /// </summary>
        private Boolean _OutputMetrics;
        /// <summary>
        /// Get and set whether to output ecosystem-level functional metrics
        /// </summary>
        public Boolean OutputMetrics
        {
            get { return _OutputMetrics; }
            set { _OutputMetrics = value; }
        }

        private List<uint> _OutputStateTimestep = new List<uint>();

        public List<uint> OutputStateTimestep
        {
            get { return _OutputStateTimestep; }
            set { _OutputStateTimestep = value; }
        }

        /// <summary>
        /// Pairs of longitude and latitude indices for all active cells in the model grid
        /// </summary>
        private List<uint[]> _CellList;
        public List<uint[]> CellList
        {
            get { return _CellList; }
            set { _CellList = value; }
        }

        //Indicates if specific locations have been specified
        private Boolean _SpecificLocations = false;

        public Boolean SpecificLocations
        {
            get { return _SpecificLocations; }
            set { _SpecificLocations = value; }
        }

#if true
        public MadingleyModelInitialisation()
        {
        }

        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="simulationInitialisationFilename">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="definitionsFilename">Definitions file</param>
        /// <param name="outputsFilename">Outputs filename</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        /// <param name="inputPath">Path to input files</param>
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
            // Write to console
            Console.WriteLine("Initializing model...\n");

            // Initialize the mass bins to be used during the model run
            _ModelMassBins = new MassBinsHandler();

            // Read the intialisation files and copy them to the output directory
#if true
            ReadAndCopyInitialisationFiles(simulationInitialisationFilename, definitionsFilename, outputsFilename, outputPath, inputPath);
#else
            ReadAndCopyInitialisationFiles(simulationInitialisationFilename, definitionsFilename, outputsFilename, outputPath);

            // Copy parameter values to an output file
            //Don't do this now as the parameter values are read in from file and this file is copied to the output directory
            //CopyParameterValues(outputPath);
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
        /// <todo>Need to adjust this file to deal with incorrect inputs, extra columns etc by throwing an error</todo>
        /// <todo>Also need to strip leading spaces</todo>
        public void ReadAndCopyInitialisationFiles(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath, string inputPath)
        {
            // Construct file name
            var DefinitionsFileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, definitionsFilename) + "&openMode=readOnly";
            var OutputsFileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, outputsFilename) + "&openMode=readOnly";

            // Read in the definitions data
            DataSet InternalData = DataSet.Open(DefinitionsFileString);

            // Get the names of parameters in the initialization file
            var VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            var VarValues = InternalData.Variables[0].GetData();
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

            // Read in the definitions data
            InternalData = DataSet.Open(DefinitionsFileString);

            // Get the names of parameters in the initialization file
            VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            VarValues = InternalData.Variables[0].GetData();
#endif

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "mass bin filename":
                        // Set up the mass bins as specified in the initialization file
#if true
                        _ModelMassBins.SetUpMassBins(VarValues.GetValue(row).ToString(), outputPath, inputPath);
#else
                        _ModelMassBins.SetUpMassBins(VarValues.GetValue(row).ToString(), outputPath);
#endif
                        break;
                }
            }

            InternalData.Dispose();

            // Read in the outputs data
            InternalData = DataSet.Open(OutputsFileString);

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
                    case "track processes":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                _TrackProcesses = true;
                                break;
                            case "no":
                                _TrackProcesses = false;
                                break;
                        }
                        break;
                    case "track cross cell processes":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                _TrackCrossCellProcesses = true;
                                break;
                            case "no":
                                _TrackCrossCellProcesses = false;
                                break;
                        }
                        break;
                    case "track global processes":
                        switch (VarValues.GetValue(row).ToString().ToLower())
                        {
                            case "yes":
                                _TrackGlobalProcesses = true;
                                break;
                            case "no":
                                _TrackGlobalProcesses = false;
                                break;
                        }
                        break;
                    case "new cohorts filename":
                        _ProcessTrackingOutputs.Add("NewCohortsOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "maturity filename":
                        _ProcessTrackingOutputs.Add("MaturityOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "biomasses eaten filename":
                        _ProcessTrackingOutputs.Add("BiomassesEatenOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "trophic flows filename":
                        _ProcessTrackingOutputs.Add("TrophicFlowsOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "growth filename":
                        _ProcessTrackingOutputs.Add("GrowthOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "metabolism filename":
                        _ProcessTrackingOutputs.Add("MetabolismOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "npp output filename":
                        _ProcessTrackingOutputs.Add("NPPOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "predation flows filename":
                        _ProcessTrackingOutputs.Add("PredationFlowsOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "herbivory flows filename":
                        _ProcessTrackingOutputs.Add("HerbivoryFlowsOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "mortality filename":
                        _ProcessTrackingOutputs.Add("MortalityOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "extinction filename":
                        _ProcessTrackingOutputs.Add("ExtinctionOutput", VarValues.GetValue(row).ToString());
                        break;
                    case "output detail":
                        _InitialisationFileStrings.Add("OutputDetail", VarValues.GetValue(row).ToString());
                        break;
                    case "live outputs":
                        if (VarValues.GetValue(row).ToString() == "yes")
                            _LiveOutputs = true;
                        else _LiveOutputs = false;
                        break;
                    case "track marine specifics":
                        if (VarValues.GetValue(row).ToString() == "yes")
                            _TrackMarineSpecifics = true;
                        else _TrackMarineSpecifics = false;
                        break;
                    case "output metrics":
                        if (VarValues.GetValue(row).ToString() == "yes")
                            _OutputMetrics = true;
                        else _OutputMetrics = false;
                        break;
                    case "output model state timesteps":

                        if (VarValues.GetValue(row).ToString() != "no")
                        {
                            string[] OutputStateTimesSteps = VarValues.GetValue(row).ToString().Split(new char[] { ';' });
                            foreach (string t in OutputStateTimesSteps)
                            {
                                if (t.Split(new char[] { '-' }).Length > 1)
                                {
                                    string[] range = t.Split(new char[] { '-' });
                                    for (uint i = Convert.ToUInt32(range[0]); i <= Convert.ToUInt32(range[1]); i++)
                                    {
                                        _OutputStateTimestep.Add(i);
                                    }
                                }
                                else
                                {
                                    _OutputStateTimestep.Add(Convert.ToUInt32(Convert.ToInt32(t)));
                                }
                            }
                        }

                        break;
                }
            }

            InternalData.Dispose();

        }
    }
}
