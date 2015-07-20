using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;
using Timing;

using Microsoft.Research.Science.Data;

using System.IO;

namespace Madingley
{
#if true
    using Madingley.Output;
#endif

    /// <summary>
    /// The ecosystem model
    /// </summary>
    public class MadingleyModel
    {
        /// <summary>
        /// An instance of the cohort functional group definitions for this model
        /// </summary>
        private FunctionalGroupDefinitions CohortFunctionalGroupDefinitions;
        /// <summary>
        /// An instance of the stock functional group definitions for this model
        /// </summary>
        private FunctionalGroupDefinitions StockFunctionalGroupDefinitions;

        /// <summary>
        /// A list of environmental data layers
        /// </summary>
#if true
        private SortedList<string, double[]>[] EnviroStack;
#else
        private SortedList<string, EnviroData> EnviroStack = new SortedList<string, EnviroData>();
#endif

        /// <summary>
        /// An instance of ModelGrid to hold the grid to be used in this model
        /// </summary>
        private ModelGrid EcosystemModelGrid;

        /// <summary>
        /// The lowest latitude for the model grid
        /// </summary>
        private float BottomLatitude;
        /// <summary>
        /// The upper latitude for the model grid
        /// </summary>
        private float TopLatitude;
        /// <summary>
        /// The left-most longitude for the model grid
        /// </summary>
        private float LeftmostLongitude;
        /// <summary>
        /// The right-most longitude for the model grid
        /// </summary>
        private float RightmostLongitude;

        /// <summary>
        /// The size of the grid cells in degrees
        /// </summary>
        private float CellSize;

        /// <summary>
        /// The number of time steps in the model run
        /// </summary>
        private uint NumTimeSteps;

        /// <summary>
        /// The timesteps for which model state should be output 
        /// </summary>
        private List<uint> OutputModelStateTimestep;

        /// <summary>
        /// The current time step
        /// </summary>
        public uint CurrentTimeStep;

        /// <summary>
        /// The current month: 1=Jan; 2=Feb; 3=Mar etc.
        /// </summary>
        public uint CurrentMonth;

        /// <summary>
        /// Whether to use randomisation in the model run, i.e. cohorts will be seeeded with random masses and cohorts will act in a random order
        /// Default is true
        /// </summary>
        public Boolean DrawRandomly = true;

        /// <summary>
        /// The time step units for this model
        /// </summary>
        private string _GlobalModelTimeStepUnit;
        /// <summary>
        /// Get or set the time step units for this model
        /// </summary>
        public string GlobalModelTimeStepUnit
        {
            get { return _GlobalModelTimeStepUnit; }
            set { _GlobalModelTimeStepUnit = value; }
        }

        /// <summary>
        /// Pairs of longitude and latitude indices for all active cells in the model grid
        /// </summary>
        private List<uint[]> _CellList;

        /// <summary>
        /// A list of global diagnostics for this model run
        /// </summary>
        public SortedList<string, double> GlobalDiagnosticVariables;

        /// <summary>
        /// Whether the model will run in parallel (default  is false)
        /// </summary>
        private Boolean RunGridCellsInParallel = false;

        /// <summary>
        /// Whether the model will be run for specific locations, instead of for the whole model grid
        /// </summary>
        public Boolean SpecificLocations;

        /// <summary>
        /// An instance of StopWatch to time individual time steps
        /// </summary>
        private StopWatch TimeStepTimer;
        private StopWatch EcologyTimer;
        private StopWatch OutputTimer;
        /// <summary>
        /// An array of instances of the output class to deal with grid cell outputs
        /// </summary>
        private OutputCell[] CellOutputs;

        /// <summary>
        /// An array of indices of process trackers for each grid cell
        /// </summary>
        private ProcessTracker[] ProcessTrackers;

        /// <summary>
        /// An instance of a cross-cell process tracker
        /// </summary>
        private CrossCellProcessTracker TrackCrossCellProcesses;

        /// <summary>
        /// An instance of a global process tracker to track global data across the model grid
        /// </summary>
        private GlobalProcessTracker TrackGlobalProcesses;

        /// <summary>
        /// An instance of OutputModelState to output the state of all
        /// cohorts and stocks in the model at a particular time
        /// </summary>
        private OutputModelState WriteModelState;

        /// <summary>
        /// An instance of the output class to deal with global outputs
        /// </summary>
        private OutputGlobal GlobalOutputs;

        /// <summary>
        /// An instance of the output class to deal with gridded outputs
        /// </summary>
        private OutputGrid GridOutputs;

        /// <summary>
        /// The suffix to be applied to files output by this model instance
        /// </summary>
        string OutputFilesSuffix;

        /// <summary>
        /// A sorted list of strings from the initialisation file
        /// </summary>
        SortedList<string, string> InitialisationFileStrings = new SortedList<string, string>();

        /// <summary>
        /// Variable to track the number of cohorts that have dispersed. Doesn't need to be thread-local because all threads have converged prior to running cross-grid-cell processes
        /// </summary>
        public uint Dispersals;

#if true
        public MadingleyModelInitialisation ModelInitialisation { get; set; }

        /// <summary>
        /// Initializes the ecosystem model
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
        /// <param name="simulation">The index of the simulation being run</param>
        /// <param name="modelState">Existing model state or null</param>
        public MadingleyModel(
            MadingleyModelInitialisation initialisation,
            string outputFilesSuffix,
            int simulation,
            Madingley.Common.ModelState modelState)
        {
            this.ModelInitialisation = initialisation;

            var scenarioIndex = 0;
            var globalModelTimeStepUnit = initialisation.GlobalModelTimeStepUnit;

            this.GlobalDiagnosticVariables = new SortedList<string, double>(modelState.GlobalDiagnosticVariables);
            var gridCells = Converters.ConvertGridCells(modelState.GridCells);

            // Assign the properties for this model run
            AssignModelRunProperties(initialisation, outputFilesSuffix);

#else
        /// <summary>
        /// Initializes the ecosystem model
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario being run</param>
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
        /// <param name="globalModelTimeStepUnit">The time step unit used in the model</param>
        /// <param name="simulation">The index of the simulation being run</param>
        public MadingleyModel(MadingleyModelInitialisation initialisation, ScenarioParameterInitialisation scenarioParameters, int scenarioIndex,
            string outputFilesSuffix, string globalModelTimeStepUnit, int simulation)
        {         
            // Assign the properties for this model run
            AssignModelRunProperties(initialisation, scenarioParameters, scenarioIndex, outputFilesSuffix);
#endif

            // Set up the model grid
#if true
            this._CellList = initialisation.CellList.ToList();
            SetUpModelGrid(initialisation);
            EcosystemModelGrid.SetGridCells(gridCells, this._CellList);
#else
            SetUpModelGrid(initialisation, scenarioParameters, scenarioIndex, simulation);
#endif

            // Set up model outputs
            SetUpOutputs(initialisation, simulation, scenarioIndex);

            // Make the initial outputs
            InitialOutputs(outputFilesSuffix, initialisation, CurrentMonth);

            // Instance the array of process trackers
            ProcessTrackers = new ProcessTracker[_CellList.Count];

            // Temporary variables
            Boolean varExists;

            // Set up process trackers for each grid cell
            for (int i = 0; i < _CellList.Count; i++)
            {
                ProcessTrackers[i] = new ProcessTracker(NumTimeSteps,
                EcosystemModelGrid.Lats, EcosystemModelGrid.Lons,
                _CellList,
                initialisation.ProcessTrackingOutputs,
                initialisation.TrackProcesses,
                CohortFunctionalGroupDefinitions,
                EcosystemModelGrid.GlobalMissingValue,
                outputFilesSuffix,
                initialisation.OutputPath, initialisation.ModelMassBins,
                SpecificLocations, i, initialisation,
                EcosystemModelGrid.GetEnviroLayer("Realm", 0, _CellList[i][0], _CellList[i][1], out varExists) == 2.0,
                EcosystemModelGrid.LatCellSize,
                EcosystemModelGrid.LonCellSize);
            }

            // Set up a cross cell process tracker
            TrackCrossCellProcesses = new CrossCellProcessTracker(initialisation.TrackCrossCellProcesses, "DispersalData", initialisation.OutputPath, outputFilesSuffix);

            //Set up a global process tracker
            if (SpecificLocations) initialisation.TrackGlobalProcesses = false;

            TrackGlobalProcesses = new GlobalProcessTracker(NumTimeSteps,
                EcosystemModelGrid.Lats, EcosystemModelGrid.Lons,
                _CellList,
                initialisation.ProcessTrackingOutputs,
                initialisation.TrackGlobalProcesses,
                CohortFunctionalGroupDefinitions,
                StockFunctionalGroupDefinitions,
                EcosystemModelGrid.GlobalMissingValue,
                outputFilesSuffix,
                initialisation.OutputPath, initialisation.ModelMassBins,
                SpecificLocations, initialisation,
                EcosystemModelGrid.LatCellSize,
                EcosystemModelGrid.LonCellSize);

            //Set-up the instance of OutputModelState
            WriteModelState = new OutputModelState(initialisation, outputFilesSuffix, simulation);

            if (SpecificLocations) initialisation.RunRealm = "";

            // Record the initial cohorts in the process trackers
            RecordInitialCohorts();

            // Initialise the time step timer
            TimeStepTimer = new StopWatch();
            EcologyTimer = new StopWatch();
            OutputTimer = new StopWatch();

            // Set the global model time step unit
            _GlobalModelTimeStepUnit = globalModelTimeStepUnit;
        }

#if true
        public void EndTimestep(
            int currentTimestep,
            Madingley.Common.ModelState modelState)
        {
            this.CurrentTimeStep = (uint)currentTimestep;
            this.CurrentMonth = Utilities.GetCurrentMonth(this.CurrentTimeStep, _GlobalModelTimeStepUnit);
            this.GlobalDiagnosticVariables = new SortedList<string, double>(modelState.GlobalDiagnosticVariables);
            var gridCells = Converters.ConvertGridCells(modelState.GridCells);

            this.EcosystemModelGrid.SetGridCells(gridCells, this._CellList);
        }

        public void RecordDispersals(
            IList<Madingley.Common.GridCellDispersal> dispersalData,
            uint numberOfDispersals)
        {
            if (dispersalData.Count() > 0)
            {
                Converters.CopyCrossCellProcessTrackerData(
                    this.TrackCrossCellProcesses,
                    dispersalData,
                    this.CurrentTimeStep,
                    this.EcosystemModelGrid);
            }

            this.Dispersals = numberOfDispersals;
        }

        public void SaveTimestep(int currentTimestep)
        {
            UInt32 hh = this.CurrentTimeStep;

            MadingleyModelInitialisation initialisation = this.ModelInitialisation;

            // Temporary variables
            Boolean varExists;

            // For runs with specific locations and where track processes has been specified, write out mass flows data and reset the mass flow tracker 
            // for the next time step
            if (SpecificLocations)
            {
                for (var cellIndex = 0; cellIndex < _CellList.Count; cellIndex++)
                {
                    if (ProcessTrackers[cellIndex].TrackProcesses)
                    {
                        ProcessTrackers[cellIndex].EndTimeStepPredationTracking(CurrentTimeStep);
                        ProcessTrackers[cellIndex].EndTimeStepHerbvioryTracking(CurrentTimeStep);
                    }
                }
            }

            if (TrackGlobalProcesses.TrackProcesses)
            {
                for (uint ii = 0; ii < StockFunctionalGroupDefinitions.GetNumberOfFunctionalGroups(); ii++)
                {
                    TrackGlobalProcesses.StoreNPPGrid(hh, ii);
                    TrackGlobalProcesses.StoreHANPPGrid(hh, ii);
                }
            }
#endif
            OutputTimer.Start();

            // Write the global outputs for this time step
            GlobalOutputs.TimeStepOutputs(EcosystemModelGrid, CurrentTimeStep, CurrentMonth, TimeStepTimer, CohortFunctionalGroupDefinitions,
                StockFunctionalGroupDefinitions, _CellList, GlobalDiagnosticVariables, initialisation);

            OutputTimer.Stop();
            Console.WriteLine("Global Outputs took: {0}", OutputTimer.GetElapsedTimeSecs());

            OutputTimer.Start();

            if (SpecificLocations)
            {
                // Loop over grid cells and write (a) time step outputs, and (b) trophic flow data (if process tracking is on)
                for (int i = 0; i < _CellList.Count; i++)
                {
                    // Write out the grid cell outputs for this time step
                    CellOutputs[i].TimeStepOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                        _CellList, i, GlobalDiagnosticVariables, TimeStepTimer, NumTimeSteps, CurrentTimeStep, initialisation, CurrentMonth, EcosystemModelGrid.GetEnviroLayer("Realm", 0, _CellList[i][0], _CellList[i][1], out varExists) == 2.0);

                    // Write out trophic flow data for this time step
                    if (ProcessTrackers[i].TrackProcesses) ProcessTrackers[i].WriteTimeStepTrophicFlows(CurrentTimeStep, EcosystemModelGrid.NumLatCells, EcosystemModelGrid.NumLonCells, initialisation,
                         EcosystemModelGrid.GetEnviroLayer("Realm", 0, _CellList[i][0], _CellList[i][1], out varExists) == 2.0);

                }
            }
            else
            {
                // Write out grid outputs for this time step
                GridOutputs.TimeStepOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, _CellList,
                    CurrentTimeStep, initialisation);
            }

            OutputTimer.Stop();
            Console.WriteLine("Cell/Grid Outputs took: {0}", OutputTimer.GetElapsedTimeSecs());

            // Write the results of dispersal to the console
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Number of cohorts that dispersed this time step: {0}\n", Dispersals);
            Console.ForegroundColor = ConsoleColor.White;
            Dispersals = 0;

            if (OutputModelStateTimestep.Contains(hh))
            {
                OutputTimer.Start();
                Console.WriteLine("Outputting model state");

                //Writing to text based output
                WriteModelState.OutputCurrentModelState(EcosystemModelGrid, _CellList, hh);
                WriteModelState.OutputCurrentModelState(EcosystemModelGrid, CohortFunctionalGroupDefinitions, _CellList, CurrentTimeStep, initialisation.MaxNumberOfCohorts, "ModelState");

                OutputTimer.Stop();
                // Write the results of dispersal to the console
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Writing model state took: {0}", OutputTimer.GetElapsedTimeSecs());
                Console.ForegroundColor = ConsoleColor.White;

                /*ReadModelState = new InputModelState(
                    initialisation.OutputPath,
                    "ModelState0");*/

            }
        }

#if true
        public Object EndYear(int year)
        {
            var globalDataSet = this.GlobalOutputs.Clone();
            var cellDataSets = (DataSet[])null;
            var gridDataSet = (DataSet)null;
            var nppDataSet = (DataSet)null;

            if (this.CellOutputs != null)
            {
                cellDataSets =
                    this.CellOutputs.Select(
                        cell =>
                        {
                            return cell.Clone();
                        }).ToArray();
            }
            else
            {
                cellDataSets = new DataSet[] { };
                gridDataSet = this.GridOutputs.Clone();
                nppDataSet = this.TrackGlobalProcesses.TrackNPP.Clone();
            }

            var crossCellProcessOutputs =
                this.TrackCrossCellProcesses != null && this.TrackCrossCellProcesses.TrackCrossCellProcesses ? this.TrackCrossCellProcesses.TrackDispersal.Clone(year) : null;

            var o = new MadingleyModelOutputDataSets
                    {
                        Global = globalDataSet,
                        Cells = cellDataSets,
                        Dispersal = crossCellProcessOutputs,
                        Grid = gridDataSet,
                        NPP = nppDataSet
                    };

            return o;
        }

        public void EndRun()
        {
            MadingleyModelInitialisation initialisation = this.ModelInitialisation;

            // Temporary
            Boolean varExists;
#endif
            if (TrackGlobalProcesses.TrackProcesses) TrackGlobalProcesses.CloseNPPFile();

            // Loop over cells and close process trackers
            for (int i = 0; i < _CellList.Count; i++)
            {
                if (ProcessTrackers[i].TrackProcesses) ProcessTrackers[i].CloseStreams(SpecificLocations);
            }

            // Write the final global outputs
            GlobalOutputs.FinalOutputs();

            if (SpecificLocations)
            {
                // Loop over grid cells and write the final grid cell outputs
                for (int i = 0; i < _CellList.Count; i++)
                {
                    CellOutputs[i].FinalOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                        _CellList, i, GlobalDiagnosticVariables, initialisation, CurrentMonth, EcosystemModelGrid.GetEnviroLayer("Realm", 0, _CellList[i][0], _CellList[i][1], out varExists) == 2.0);
                }
            }
            else
            {
                // Write the final grid outputs
                GridOutputs.FinalOutputs();
            }
        }

#if true
        /// <summary>
        /// Assigns the properties of the current model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
        public void AssignModelRunProperties(MadingleyModelInitialisation initialisation,
            string outputFilesSuffix)
#else
        /// <summary>
        /// Assigns the properties of the current model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario that this model is to run</param>
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
        public void AssignModelRunProperties(MadingleyModelInitialisation initialisation, 
            ScenarioParameterInitialisation scenarioParameters, int scenarioIndex,
            string outputFilesSuffix)
#endif
        {
            // Assign the properties of this model run from the same properties in the specified model initialisation
            _GlobalModelTimeStepUnit = initialisation.GlobalModelTimeStepUnit;
            NumTimeSteps = initialisation.NumTimeSteps;
            CellSize = (float)initialisation.CellSize;
            _CellList = initialisation.CellList;
            BottomLatitude = initialisation.BottomLatitude;
            TopLatitude = initialisation.TopLatitude;
            LeftmostLongitude = initialisation.LeftmostLongitude;
            RightmostLongitude = initialisation.RightmostLongitude;
            InitialisationFileStrings = initialisation.InitialisationFileStrings;
            CohortFunctionalGroupDefinitions = initialisation.CohortFunctionalGroupDefinitions;
            StockFunctionalGroupDefinitions = initialisation.StockFunctionalGroupDefinitions;
            EnviroStack = initialisation.EnviroStack;
            OutputFilesSuffix = outputFilesSuffix;
            OutputModelStateTimestep = initialisation.OutputStateTimestep;
            SpecificLocations = initialisation.SpecificLocations;
        }

        /// <summary>
        /// Sets up the model outputs
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param>
        /// <param name="simulation">The index of the simulation being run</param>
        /// <param name="scenarioIndex">The index of the scenario being run</param>
        public void SetUpOutputs(MadingleyModelInitialisation initialisation, int simulation, int scenarioIndex)
        {
            // Initialise the global outputs
            GlobalOutputs = new OutputGlobal(InitialisationFileStrings["OutputDetail"], initialisation);

            // Create new outputs class instances (if the model is run for the whold model grid then select the grid view for the live output,
            // if the model is run for specific locations then use the graph view)
            if (SpecificLocations)
            {

                // Initialise the vector of outputs instances
                CellOutputs = new OutputCell[_CellList.Count];

                for (int i = 0; i < _CellList.Count; i++)
                {
                    CellOutputs[i] = new OutputCell(InitialisationFileStrings["OutputDetail"], initialisation, i);
                }

#if false
                // Spawn a dataset viewer instance for each cell to display live model results
                if (initialisation.LiveOutputs)
                {
                    for (int i = 0; i < _CellList.Count; i++)
                    {
                        CellOutputs[i].SpawnDatasetViewer(NumTimeSteps);
                    }
                }
#endif

            }
            else
            {
                GridOutputs = new OutputGrid(InitialisationFileStrings["OutputDetail"], initialisation);

#if false
                // Spawn dataset viewer to display live grid results
                if (initialisation.LiveOutputs)
                {
                    GridOutputs.SpawnDatasetViewer();
                }
#endif
            }
        }

#if true
        /// <summary>
        /// Sets up the model grid within a Madingley model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        public void SetUpModelGrid(MadingleyModelInitialisation initialisation)
#else
        /// <summary>
        /// Sets up the model grid within a Madingley model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario that this model is to run</param>
        public void SetUpModelGrid(MadingleyModelInitialisation initialisation,
            ScenarioParameterInitialisation scenarioParameters, int scenarioIndex, int simulation)
#endif
        {
            // If the intialisation file contains a column pointing to another file of specific locations, and if this column is not blank then read the 
            // file indicated
            if (SpecificLocations)
            {
                // Set up the model grid using these locations
#if true
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations, RunGridCellsInParallel);
#else
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations,RunGridCellsInParallel);
#endif

            }
            else
            {
                EcologyTimer = new StopWatch();
                EcologyTimer.Start();

                // Set up a full model grid (i.e. not for specific locations)
                // Set up the model grid using these locations
#if true
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations, RunGridCellsInParallel);
#else
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations, RunGridCellsInParallel);
#endif

                EcologyTimer.Stop();
                Console.WriteLine("Time to initialise cells: {0}", EcologyTimer.GetElapsedTimeSecs());

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Madingley Model memory usage post grid cell seed: {0}", GC.GetTotalMemory(true) / 1E9, " (G Bytes)\n");
                Console.ForegroundColor = ConsoleColor.White;

            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Madingley Model memory usage pre Collect: {0}", Math.Round(GC.GetTotalMemory(true) / 1E9, 2), " (GBytes)");
            Console.ForegroundColor = ConsoleColor.White;
            GC.Collect();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Madingley Model memory usage post Collect: {0}", Math.Round(GC.GetTotalMemory(true) / 1E9, 5), " (GBytes)\n");
            Console.ForegroundColor = ConsoleColor.White;

        }

        /// <summary>
        /// Generates the initial outputs for this model run
        /// </summary>
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
        /// <param name="initialisation">The Madingley Model initialisation</param>
        /// <param name="month">The current month in the model run</param>
        public void InitialOutputs(string outputFilesSuffix, MadingleyModelInitialisation initialisation, uint month)
        {
            // Set up global outputs for all model runs
            GlobalOutputs.SetupOutputs(NumTimeSteps, EcosystemModelGrid, OutputFilesSuffix);

            // Create initial global outputs
            GlobalOutputs.InitialOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, _CellList,
                GlobalDiagnosticVariables, initialisation);

            // Temporary
            Boolean varExists;

            if (SpecificLocations)
            {
                for (int i = 0; i < _CellList.Count; i++)
                {
                    // Set up grid cell outputs
                    CellOutputs[i].SetUpOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                        NumTimeSteps, OutputFilesSuffix, _CellList, i, EcosystemModelGrid.GetEnviroLayer("Realm", 0, _CellList[i][0], _CellList[i][1], out varExists) == 2.0);

                    // Create initial grid cell outputs
                    CellOutputs[i].InitialOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                        _CellList, i, GlobalDiagnosticVariables, NumTimeSteps, initialisation, month, EcosystemModelGrid.GetEnviroLayer("Realm", 0, _CellList[i][0], _CellList[i][1], out varExists) == 2.0);
                }
            }
            else
            {
                // Set up grid outputs
                GridOutputs.SetupOutputs(EcosystemModelGrid, OutputFilesSuffix, NumTimeSteps,
                    CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions);

                // Create initial grid outputs
                GridOutputs.InitialOutputs(EcosystemModelGrid, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, _CellList, initialisation);
            }

        }

        /// <summary>
        /// Make a record of the properties of the intial model cohorts in the new cohorts output file
        /// </summary>
        public void RecordInitialCohorts()
        {
            int i = 0;
            foreach (uint[] cell in _CellList)
            {
                if (ProcessTrackers[i].TrackProcesses)
                {

                    GridCellCohortHandler TempCohorts = EcosystemModelGrid.GetGridCellCohorts(cell[0], cell[1]);

                    for (int FunctionalGroup = 0; FunctionalGroup < TempCohorts.Count; FunctionalGroup++)
                    {
                        foreach (Cohort item in TempCohorts[FunctionalGroup])
                        {
                            ProcessTrackers[i].RecordNewCohort(cell[0], cell[1], 0, item.CohortAbundance, item.AdultMass, item.FunctionalGroupIndex,
                                new List<uint> { uint.MaxValue }, item.CohortID[0]);
                        }
                    }
                }
                i += 1;
            }
        }

#if true
        public ProcessTracker GetProcessTracker(int cellIndex)
        {
            return this.ProcessTrackers[cellIndex];
        }

        public GlobalProcessTracker GetGlobalProcessTracker()
        {
            return this.TrackGlobalProcesses;
        }

        public void Copy(MadingleyModel existing)
        {
            this.GlobalOutputs.Copy(existing.GlobalOutputs);

            if (this.CellOutputs != null &&
                existing.CellOutputs != null)
            {
                for (var i = 0; i < this.CellOutputs.Count(); i++)
                {
                    this.CellOutputs[i].Copy(existing.CellOutputs[i]);
                }
            }

            if (this.GridOutputs != null && existing.GridOutputs != null)
            {
                this.GridOutputs.Copy(existing.GridOutputs);
            }

            if (this.TrackGlobalProcesses != null && this.TrackGlobalProcesses.TrackNPP != null &&
                existing.TrackGlobalProcesses != null && existing.TrackGlobalProcesses.TrackNPP != null)
            {
                this.TrackGlobalProcesses.TrackNPP.Copy(existing.TrackGlobalProcesses.TrackNPP);
            }

            if (this.TrackCrossCellProcesses != null && this.TrackCrossCellProcesses.TrackDispersal != null &&
                existing.TrackCrossCellProcesses != null && existing.TrackCrossCellProcesses.TrackDispersal != null)
            {
                this.TrackCrossCellProcesses.TrackDispersal.Copy(existing.TrackCrossCellProcesses.TrackDispersal);
            }
        }
#endif
    }

}