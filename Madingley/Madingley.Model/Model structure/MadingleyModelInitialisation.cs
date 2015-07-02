using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;


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
        /// For scenarios with instantaneous impacts, the time step in which to apply the impact
        /// </summary>
        private uint _InstantaneousTimeStep;

        /// <summary>
        /// Get and set the time step in which to apply the impact, for scenarios with instantaneous impacts
        /// </summary>
        public uint InstantaneousTimeStep
        {
            get { return _InstantaneousTimeStep; }
            set { _InstantaneousTimeStep = value; }
        }


        /// <summary>
        /// For scenarios with instantaneous impacts, the number of time steps to apply the impact for
        /// </summary>
        private uint _NumInstantaneousTimeStep;

        /// <summary>
        /// Get and set the number of time steps to apply the impact for, for scenarios with instantaneous impacts
        /// </summary>
        public uint NumInstantaneousTimeStep
        {
            get { return _NumInstantaneousTimeStep; }
            set { _NumInstantaneousTimeStep = value; }
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
        /// Whether to run the model for different grid cells in parallel
        /// </summary>
        private Boolean _RunInParallel = false;
        /// <summary>
        /// Get and set whether to run the model for different grid cells in parallel
        /// </summary>
        public Boolean RunInParallel
        {
            get { return _RunInParallel; }
            set { _RunInParallel = value; }
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
        /// The threshold difference between cohorts, within which they will be merged
        /// </summary>
        private double _MergeDifference;
        /// <summary>
        /// Get and set the threshold difference between cohorts, within which they will be merged
        /// </summary>
        public double MergeDifference
        {
            get { return _MergeDifference; }
            set { _MergeDifference = value; }
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
        /// The string values for the units of each environmental data layer
        /// </summary>
        private SortedList<string, string> _Units = new SortedList<string, string>();
        /// <summary>
        /// Get and set the unit strings
        /// </summary>
        public SortedList<string, string> Units
        {
            get { return _Units; }
            set { _Units = value; }
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


        private List<uint> _OutputStateTimestep = new List<uint>();

        public List<uint> OutputStateTimestep
        {
            get { return _OutputStateTimestep; }
            set { _OutputStateTimestep = value; }
        }

        //proportion of the model grid that is fragmented
        private float _FragmentProportion;

        /// <summary>
        /// Reads in and holds the state of the model as specified in the input file
        /// </summary>
        private List<InputModelState> _ModelStates;
        public List<InputModelState> ModelStates
        {
            get { return _ModelStates; }
            set { _ModelStates = value; }
        }

        private Boolean _InputState = false;

        public Boolean InputState
        {
            get { return _InputState; }
            set { _InputState = value; }
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
        public SortedList<string, double> InputGlobalDiagnosticVariables;
#endif

        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        public MadingleyModelInitialisation(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath)
        {
        }

    }



}
