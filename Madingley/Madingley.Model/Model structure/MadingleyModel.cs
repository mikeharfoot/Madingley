using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;
using Timing;


using System.IO;

namespace Madingley
{   
    
    /// <summary>
    /// Thread-local variables for tracking extinction and production of cohorts
    /// </summary>
    /// <todo>Needs a little tidying and checking of access levels</todo>
    public class ThreadLockedParallelVariables 
    { 
        /// <summary>
        /// Thread-local variable to track the extinction of cohorts
        /// </summary>
        public int Extinctions;

        /// <summary>
        /// Thread-local variable to track the production of cohorts
        /// </summary>
        public int Productions;

        /// <summary>
        /// Variable to track the number of cohorts combined
        /// </summary>
        public int Combinations { get; set; }

        /// <summary>
        /// Thread-locked variable to track the cohort ID to assign to newly produced cohorts
        /// </summary>
        public Int64 NextCohortIDThreadLocked;

    }



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
        /// An instance of the cross grid cell ecology class
        /// </summary>
        private EcologyCrossGridCell MadingleyEcologyCrossGridCell;

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
        /// The number of time steps to run before any human impacts are simulated
        /// </summary>
        private uint NumBurninSteps;

        /// <summary>
        /// For temporary impacts, the number of time steps to apply the impact for
        /// </summary>
        private uint NumImpactSteps;

        /// <summary>
        /// For temporary impacts, the number of time steps to apply the impact for
        /// </summary>
        private uint NumRecoverySteps;

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
        /// The threshold abundance below which cohorts will automatically become extinct
        /// </summary>
        private double _ExtinctionThreshold;
        /// <summary>
        /// Get the extinction threshold for this model
        /// </summary>
        public double ExtinctionThreshold
        {  get { return _ExtinctionThreshold; } }

        //Values to define when cohorts can be merged
        /// <summary>
        /// The proportional difference in adult, juvenile and current body masses that cohorts must fall within in order to be considered for merging
        /// </summary>
        private double MergeDifference;
        
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
//        /// <summary>
//        /// An array of instances of the output class to deal with grid cell outputs
//        /// </summary>
//        private OutputCell[] CellOutputs;

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

//        /// <summary>
//        /// An instance of OutputModelState to output the state of all
//        /// cohorts and stocks in the model at a particular time
//        /// </summary>
//        private OutputModelState WriteModelState;

//        /// <summary>
//        /// An instance of OutputModelState to output the state of all
//        /// cohorts and stocks in the model at a particular time
//        /// </summary>
//        private InputModelState ReadModelState;

//        /// <summary>
//        /// An instance of the output class to deal with global outputs
//        /// </summary>
//        private OutputGlobal GlobalOutputs;

//        /// <summary>
//        /// An instance of the output class to deal with gridded outputs
//        /// </summary>
//        private OutputGrid GridOutputs;

        /// <summary>
        /// The suffix to be applied to files output by this model instance
        /// </summary>
        string OutputFilesSuffix;

        /// <summary>
        /// A sorted list of strings from the initialisation file
        /// </summary>
        SortedList<string, string> InitialisationFileStrings = new SortedList<string, string>();


        /// <summary>
        /// A sorted list of strings for environmental data units
        /// </summary>
        SortedList<string, string> EnvironmentalDataUnits = new SortedList<string, string>();

        /// <summary>
        /// The scenario of human NPP extraction to use
        /// </summary>
        /// <value>The first item is the scenario type
        /// The second item is an associated magnitude</value>
        public Madingley.Common.ScenarioParameter HumanNPPScenario { get; private set; }

        /// <summary>
        /// The scenario of temperature change to use
        /// </summary>
        public Madingley.Common.ScenarioParameter TemperatureScenario { get; private set; }

        /// <summary>
        /// The scenario of direct animal harvesting to use
        /// </summary>
        public Madingley.Common.ScenarioParameter HarvestingScenario { get; private set; }
        

        // A variable to increment for the purposes of giving each cohort a unique ID
        private Int64 NextCohortID;
     
        /// <summary>
        /// Variable to track the number of cohorts that have dispersed. Doesn't need to be thread-local because all threads have converged prior to running cross-grid-cell processes
        /// </summary>
        public uint Dispersals;

        // An instance of the climate change impacts class
        ClimateChange ClimateChangeSimulator;

        // An instance of the direct harvesting impacts class
        Harvesting HarvestingSimulator;

#if true
        public MadingleyModelInitialisation mmi { get; set; }

        public Madingley.Common.ModelState CreateModelStateData(uint timeStep)
        {
            var modelStateData = Converters.ConvertModelStateData(
                timeStep,
                this.GlobalDiagnosticVariables,
                this.EcosystemModelGrid.GetGridCell,
                this._CellList,
                this.NextCohortID);

            return modelStateData;
        }

        /// <summary>
        /// Initializes the ecosystem model.
        /// </summary>
        /// <param name="modelState">Existing model state, or null.</param> 
        /// <param name="configuration">Configuration.</param>
        /// <param name="environment">Environment.</param>
        public MadingleyModel(
            Madingley.Common.ModelState modelState,
            Madingley.Common.Configuration configuration,
            Madingley.Common.Environment environment)
        {
            var initialisation = Converters.ConvertInitialisation(modelState, configuration, environment);
            var scenarioParameters = configuration.ScenarioParameters;
            Converters.ConvertEcologicalParameters(configuration.EcologicalParameters);
            var scenarioIndex = configuration.ScenarioIndex;
            var outputFilesSuffix = "";
            var simulation = configuration.Simulation;

            this.mmi = initialisation;
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
#endif

            // Assign the properties for this model run
#if true
            AssignModelRunProperties(initialisation, scenarioParameters, scenarioIndex, outputFilesSuffix, modelState);
#else
            AssignModelRunProperties(initialisation, scenarioParameters, scenarioIndex, outputFilesSuffix);
#endif

            // Set up list of global diagnostics
            SetUpGlobalDiagnosticsList();

            // Set up the model grid
            SetUpModelGrid(initialisation, scenarioParameters, scenarioIndex, simulation);

            // Set up model outputs
#if true
        }

        public IEnumerable<Tuple<Madingley.Common.RunState, ReturnType>> Initialise<ReturnType>(uint startTimeStep, Madingley.Common.IOutput output, IProgress<double> progress, System.Threading.CancellationToken cancellation)
        {
            var initialisation = this.mmi;
            var globalModelTimeStepUnit = initialisation.GlobalModelTimeStepUnit;
#else
            SetUpOutputs(initialisation, simulation, scenarioIndex);

            // Make the initial outputs
            InitialOutputs(outputFilesSuffix, initialisation, CurrentMonth);
#endif

            // Instance the array of process trackers
            ProcessTrackers = new ProcessTracker[_CellList.Count];

            // Set up process trackers for each grid cell
            for (int i = 0; i < _CellList.Count; i++)
            {
#if true
                var trackers = output.ProcessTracker.Count() > i ? output.ProcessTracker[i] : new Madingley.Common.IProcessTracker[] { };
                this.ProcessTrackers[i] = new ProcessTracker(trackers);
#else
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
#endif
            }
            
            // Set up a cross cell process tracker
#if true
            TrackCrossCellProcesses = new CrossCellProcessTracker(output.CrossCellProcessTracker.Count() > 0);
#else
            TrackCrossCellProcesses = new CrossCellProcessTracker(initialisation.TrackCrossCellProcesses, "DispersalData", initialisation.OutputPath, outputFilesSuffix);
#endif

            //Set up a global process tracker
            if (SpecificLocations) initialisation.TrackGlobalProcesses = false;

#if true
            TrackGlobalProcesses = new GlobalProcessTracker(output.GlobalProcessTracker);
#else
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
#endif

            if (SpecificLocations) initialisation.RunRealm = "";

            // Record the initial cohorts in the process trackers
            RecordInitialCohorts();

            // Initialise the class for cross-grid-cell ecology
            MadingleyEcologyCrossGridCell = new EcologyCrossGridCell();

            // Initialise the time step timer
            TimeStepTimer = new StopWatch();
            EcologyTimer = new StopWatch();
            OutputTimer = new StopWatch();

            // Set the global model time step unit
            _GlobalModelTimeStepUnit = globalModelTimeStepUnit;


            // Initialise the climate change impacts class
            ClimateChangeSimulator = new ClimateChange();

            // Initialise the harvesting impacts class
            HarvestingSimulator = new Harvesting(EcosystemModelGrid.Lats, EcosystemModelGrid.Lons, (float)EcosystemModelGrid.LatCellSize);
#if false
        }
#endif

#if true
            var originalNumTimeSteps = NumTimeSteps;

            NumTimeSteps = (uint)Utilities.ConvertTimeUnits("year", _GlobalModelTimeStepUnit);

            CurrentTimeStep = startTimeStep > 0 ? startTimeStep - 1 : 0;
            CurrentMonth = Utilities.GetCurrentMonth(CurrentTimeStep, _GlobalModelTimeStepUnit);

            for (var timeStep = startTimeStep; timeStep < originalNumTimeSteps; timeStep += NumTimeSteps)
            {
                var year = (int)(timeStep / NumTimeSteps);

                RunMadingley(this.mmi, timeStep, originalNumTimeSteps, output, progress, cancellation);

                var endYearModelStateData = this.CreateModelStateData(timeStep + NumTimeSteps);

                if (cancellation.IsCancellationRequested)
                {
                    output.EndRun();
                    Console.WriteLine("**** Cancelled!");
                }
                cancellation.ThrowIfCancellationRequested();
                progress.Report((float)(timeStep + NumTimeSteps) / (float)originalNumTimeSteps);

                var ey = output.EndYear(year);

                var state = new Madingley.Common.RunState(endYearModelStateData, output);

                yield return Tuple.Create(state, (ReturnType)ey);
            }

            output.EndRun();
        }

        /// <summary>
        /// Run the global ecosystem model
        /// </summary>
        /// <param name="initialisation">The initialization details for the current set of model simulations</param>
        /// <param name="startTimeStep">Which time step to start from</param>
        /// <param name="originalNumTimeSteps">The original number of time steps before reducing to a year</param>
        /// <param name="output">Output interface implementation</param>
        /// <param name="progress">Progress reporting callback</param>
        /// <param name="cancellation">Cancellation token to stop run</param>
        public void RunMadingley(MadingleyModelInitialisation initialisation, uint startTimeStep, uint originalNumTimeSteps, Madingley.Common.IOutput output, IProgress<double> progress, System.Threading.CancellationToken cancellation)
#else
        /// <summary>
        /// Run the global ecosystem model
        /// </summary>
        /// <param name="initialisation">The initialization details for the current set of model simulations</param>
        public void RunMadingley(MadingleyModelInitialisation initialisation)
#endif
        {            
            // Write out model run details to the console
            Console.WriteLine("Running model");
            Console.WriteLine("Number of time steps is: {0}", NumTimeSteps);
            Console.WriteLine(" ");
            Console.WriteLine(" ");

             // Run the model
#if true
            for (UInt32 hh = startTimeStep; hh < NumTimeSteps + startTimeStep; hh += 1)
#else
             for (UInt32 hh = 0; hh < NumTimeSteps; hh += 1)
#endif
            {
#if true
                if (cancellation.IsCancellationRequested)
                {
                    output.EndRun();
                    Console.WriteLine("**** Cancelled!");
                }
                cancellation.ThrowIfCancellationRequested();
                progress.Report((float)hh / (float)originalNumTimeSteps);
#endif

                 Console.WriteLine("Running time step {0}...",hh + 1);

#if true
                 DebugPrintModel("before", hh);
#endif

                 // Start the timer
                 TimeStepTimer.Start();

                 // Get current time step and month
                 CurrentTimeStep = hh;
                 CurrentMonth = Utilities.GetCurrentMonth(hh,_GlobalModelTimeStepUnit);

#if true
                 this.TrackGlobalProcesses.TimeStep = this.CurrentTimeStep;

                 this.TrackCrossCellProcesses.GridCellDispersals = null;
#endif

                 // Initialise cross grid cell ecology
                 MadingleyEcologyCrossGridCell.InitializeCrossGridCellEcology(_GlobalModelTimeStepUnit, DrawRandomly, initialisation);

                 EcologyTimer.Start();

                 // Loop over grid cells and run biological processes
                 if (RunGridCellsInParallel)
                 {
                     // Run cells in parallel
#if true
                     RunCellsInParallel(initialisation, output, progress, cancellation);
#else
                     RunCellsInParallel(initialisation);
#endif
                 }
                 else
                 {
                     // Run cells in sequence
#if true
                     RunCellsSequentially(initialisation, output, progress, cancellation);
#else
                     RunCellsSequentially(initialisation);
#endif
                 }

                 

                 EcologyTimer.Stop();
                 Console.WriteLine("Within grid ecology took: {0}" ,EcologyTimer.GetElapsedTimeSecs());
                 // Run the garbage collector. Note that it works in the background so may take a little while
                 // Needs to be done to ensure cohorts are deleted properly
                 GC.Collect();

#if true
                 if (cancellation.IsCancellationRequested)
                 {
                     output.EndRun();
                     Console.WriteLine("**** Cancelled!");
                 }
                 cancellation.ThrowIfCancellationRequested();
#else
                 if (TrackGlobalProcesses.TrackProcesses)
                 {
                     for (uint ii = 0; ii < StockFunctionalGroupDefinitions.GetNumberOfFunctionalGroups(); ii++)
                     {
                         TrackGlobalProcesses.StoreNPPGrid(hh, ii);
                         TrackGlobalProcesses.StoreHANPPGrid(hh, ii);
                     }
                 }
#endif


                 EcologyTimer.Start();

                 // Run cross grid cell ecology
                 RunCrossGridCellEcology(ref Dispersals, initialisation.DispersalOnly, initialisation);

                 EcologyTimer.Stop();
                 Console.WriteLine("Across grid ecology took: {0}", EcologyTimer.GetElapsedTimeSecs());

                 // Run the garbage collector. Note that it works in the background so may take a little while
                 // Needs to be done here to ensure cohorts are deleted properly
                 GC.Collect();
                 
                 // Stop the timer
                 TimeStepTimer.Stop();

                 OutputTimer.Start();

#if true
                 if (cancellation.IsCancellationRequested)
                 {
                     output.EndRun();
                     Console.WriteLine("**** Cancelled!");
                 }
                 cancellation.ThrowIfCancellationRequested();

                var endTimestepModelStateData = this.CreateModelStateData(hh + 1);

                output.EndTimestep((int)this.CurrentTimeStep, endTimestepModelStateData);

                if (this.TrackCrossCellProcesses.GridCellDispersals != null)
                {
                    foreach (var o in output.CrossCellProcessTracker)
                    {
                        o.RecordDispersals((int)this.CurrentTimeStep, this.TrackCrossCellProcesses.GridCellDispersals, (int)this.Dispersals);
                    }
                }
                this.TrackCrossCellProcesses.GridCellDispersals = null;

                output.SaveTimestep((int)this.CurrentTimeStep);

                this.Dispersals = 0;
#else
                 // Write the global outputs for this time step
                 GlobalOutputs.TimeStepOutputs(EcosystemModelGrid, CurrentTimeStep, CurrentMonth, TimeStepTimer,CohortFunctionalGroupDefinitions,
                     StockFunctionalGroupDefinitions,_CellList,GlobalDiagnosticVariables, initialisation);

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
                         if(ProcessTrackers[i].TrackProcesses) ProcessTrackers[i].WriteTimeStepTrophicFlows(CurrentTimeStep, EcosystemModelGrid.NumLatCells, EcosystemModelGrid.NumLonCells, initialisation,
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
                     WriteModelState.OutputCurrentModelState( EcosystemModelGrid, _CellList, hh);
                     WriteModelState.OutputCurrentModelState(EcosystemModelGrid,CohortFunctionalGroupDefinitions, _CellList, CurrentTimeStep, initialisation.MaxNumberOfCohorts,"ModelState");
                     

                     OutputTimer.Stop();
                     // Write the results of dispersal to the console
                     Console.ForegroundColor = ConsoleColor.Green;
                     Console.WriteLine("Writing model state took: {0}", OutputTimer.GetElapsedTimeSecs());
                     Console.ForegroundColor = ConsoleColor.White;

                     /*ReadModelState = new InputModelState(
                         initialisation.OutputPath,
                         "ModelState0");*/

                 }


#endif

#if true
                DebugPrintModel("after", hh);
#endif
            }

#if false
             if (TrackGlobalProcesses.TrackProcesses) TrackGlobalProcesses.CloseNPPFile();

            // Loop over cells and close process trackers
             for (int i = 0; i < _CellList.Count; i++)
             {
                 if (ProcessTrackers[i].TrackProcesses) ProcessTrackers[i].CloseStreams(SpecificLocations);
             }

            // Write the final global outputs
            GlobalOutputs.FinalOutputs();

            WriteModelState.CloseStreams();

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

            
#endif
        }

        /// <summary>
        /// A method to run the main ecosystem model loop in parallel (latitudinal strips)
        /// </summary>
        /// <param name="cellIndex">The index of the current cell in the list of all cells to run the model for</param>
        /// <param name="partial">A threadlockedparallelvariable that is used to pass global diagnostic information back with locking or race conditions</param>
        /// <param name="dispersalOnly">Whether to run dispersal only (i.e. to turn all other ecological processes off</param>
        /// <param name="initialisation">The Madingley Model intialisation</param>
        /// <remarks>Note that variables and instances of classes that are written to within this method MUST be local within this method to prevent 
        /// race issues and multiple threads attempting to write to the same variable when running the program in parallel</remarks>
        public void RunCell(int cellIndex, ThreadLockedParallelVariables partial, Boolean dispersalOnly, 
            MadingleyModelInitialisation initialisation)
        {
            // Apply any climate change impacts
            ClimateChangeSimulator.ApplyTemperatureScenario(
                EcosystemModelGrid.GetCellEnvironment(_CellList[cellIndex][0], _CellList[cellIndex][1]),
                TemperatureScenario,CurrentTimeStep,CurrentMonth,NumBurninSteps,NumImpactSteps,
                ((initialisation.ImpactCellIndices.Contains((uint)cellIndex) || initialisation.ImpactAll)));

            // Create a temporary internal copy of the grid cell cohorts
            GridCellCohortHandler WorkingGridCellCohorts = EcosystemModelGrid.GetGridCellCohorts(_CellList[cellIndex][0], _CellList[cellIndex][1]);

            // Create a temporary internal copy of the grid cell stocks
            GridCellStockHandler WorkingGridCellStocks = EcosystemModelGrid.GetGridCellStocks(_CellList[cellIndex][0], _CellList[cellIndex][1]);

            // Run stock ecology
            RunWithinCellStockEcology(_CellList[cellIndex][0], _CellList[cellIndex][1], WorkingGridCellStocks, cellIndex,
                initialisation);

            // Run within cell ecology if we are not doing dispersal only
            if (dispersalOnly)
            {
                // Run cohort ecology
                RunWithinCellDispersalOnly(_CellList[cellIndex][0], _CellList[cellIndex][1], partial, WorkingGridCellCohorts, WorkingGridCellStocks);
            }
            else
            {
                // Run cohort ecology
                RunWithinCellCohortEcology(_CellList[cellIndex][0], _CellList[cellIndex][1], partial, WorkingGridCellCohorts, WorkingGridCellStocks, InitialisationFileStrings["OutputDetail"], cellIndex, initialisation);

            }

            // Apply any direct harvesting impacts
            HarvestingSimulator.RemoveHarvestedIndividuals(WorkingGridCellCohorts, HarvestingScenario, CurrentTimeStep, NumBurninSteps,
                NumImpactSteps,NumTimeSteps, EcosystemModelGrid.GetCellEnvironment(_CellList[cellIndex][0], _CellList[cellIndex][1]),
                (initialisation.ImpactCellIndices.Contains((uint)cellIndex) || initialisation.ImpactAll), _GlobalModelTimeStepUnit, CohortFunctionalGroupDefinitions);

            // For runs with specific locations and where track processes has been specified, write out mass flows data and reset the mass flow tracker 
            // for the next time step
            if (SpecificLocations && ProcessTrackers[cellIndex].TrackProcesses)
            {
                ProcessTrackers[cellIndex].EndTimeStepPredationTracking(CurrentTimeStep);
                ProcessTrackers[cellIndex].EndTimeStepHerbvioryTracking(CurrentTimeStep);
            }

        }


#if true
        public void AssignModelRunProperties(MadingleyModelInitialisation initialisation, 
            IList<Madingley.Common.ScenarioParameters> scenarioParameters, int scenarioIndex,
            string outputFilesSuffix, Madingley.Common.ModelState modelState)
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
            NumBurninSteps = initialisation.BurninTimeSteps;
            NumImpactSteps = initialisation.ImpactTimeSteps;
            NumRecoverySteps = initialisation.RecoveryTimeSteps;
            CellSize = (float)initialisation.CellSize;
            _CellList = initialisation.CellList;
            BottomLatitude = initialisation.BottomLatitude;
            TopLatitude = initialisation.TopLatitude;
            LeftmostLongitude = initialisation.LeftmostLongitude;
            RightmostLongitude = initialisation.RightmostLongitude;
            RunGridCellsInParallel = initialisation.RunCellsInParallel;
            DrawRandomly = initialisation.DrawRandomly;
            _ExtinctionThreshold = initialisation.ExtinctionThreshold;
            MergeDifference = initialisation.MergeDifference;
            InitialisationFileStrings = initialisation.InitialisationFileStrings;
            CohortFunctionalGroupDefinitions = initialisation.CohortFunctionalGroupDefinitions;
            StockFunctionalGroupDefinitions = initialisation.StockFunctionalGroupDefinitions;
            EnviroStack = initialisation.EnviroStack;
            HumanNPPScenario = scenarioParameters.ElementAt(scenarioIndex).Parameters["npp"];
            TemperatureScenario = scenarioParameters.ElementAt(scenarioIndex).Parameters["temperature"];
            HarvestingScenario = scenarioParameters.ElementAt(scenarioIndex).Parameters["harvesting"];
            OutputFilesSuffix = outputFilesSuffix;
            EnvironmentalDataUnits = initialisation.Units;
            OutputModelStateTimestep = initialisation.OutputStateTimestep;
            SpecificLocations = initialisation.SpecificLocations;

            // Initialise the cohort ID to zero
            NextCohortID = 0;

#if true
            if (modelState != null)
            {
                this.NextCohortID = modelState.NextCohortID;
            }
#endif
        }

        /// <summary>
        /// Sets up the list of global diagnostic variables
        /// </summary>
        public void SetUpGlobalDiagnosticsList()
        {
            //Instantiate the global diagnostic variables
            GlobalDiagnosticVariables = new SortedList<string, double>();
            // Add global diagnostic variables
            GlobalDiagnosticVariables.Add("NumberOfCohortsExtinct", 0.0);
            GlobalDiagnosticVariables.Add("NumberOfCohortsProduced", 0.0);
            GlobalDiagnosticVariables.Add("NumberOfCohortsCombined", 0.0);
            GlobalDiagnosticVariables.Add("NumberOfCohortsInModel", 0.0);
            GlobalDiagnosticVariables.Add("NumberOfStocksInModel", 0.0);

        }

#if false
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

                // Spawn a dataset viewer instance for each cell to display live model results
                if (initialisation.LiveOutputs)
                {
                    for (int i = 0; i < _CellList.Count; i++)
                    {
                        CellOutputs[i].SpawnDatasetViewer(NumTimeSteps);
                    }
                }
            
            }
            else
            {
                GridOutputs = new OutputGrid(InitialisationFileStrings["OutputDetail"], initialisation);

                // Spawn dataset viewer to display live grid results
                if (initialisation.LiveOutputs)
                {
                    GridOutputs.SpawnDatasetViewer();
                }
            }

            
        }
#endif

        /// <summary>
        /// Sets up the model grid within a Madingley model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario that this model is to run</param>
        /// <param name="simulation">Simulation number</param>
        public void SetUpModelGrid(MadingleyModelInitialisation initialisation,
            IList<Madingley.Common.ScenarioParameters> scenarioParameters, int scenarioIndex, int simulation)
        {
            // If the intialisation file contains a column pointing to another file of specific locations, and if this column is not blank then read the 
            // file indicated
            if (SpecificLocations)
            {
                // Set up the model grid using these locations
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations,RunGridCellsInParallel);

            }
            else
            {
#if false
                _CellList = new List<uint[]>();
                //Switched order so we create cell list first then initialise cells using list rather than grid.

                uint NumLatCells = (uint)((TopLatitude - BottomLatitude) / CellSize);
                uint NumLonCells = (uint)((RightmostLongitude - LeftmostLongitude) / CellSize);

                // Loop over all cells in the model
                for (uint ii = 0; ii < NumLatCells; ii += 1)
                {
                    for (uint jj = 0; jj < NumLonCells; jj += 1)
                    {
                        // Define a vector to hold the pair of latitude and longitude indices for this grid cell
                        uint[] cellIndices = new uint[2];

                        // Add the latitude and longitude indices to this vector
                        cellIndices[0] = ii;
                        cellIndices[1] = jj;

                        // Add the vector to the list of all active grid cells
                        _CellList.Add(cellIndices);

                    }
                }
#endif

                EcologyTimer = new StopWatch();
                EcologyTimer.Start();

                // Set up a full model grid (i.e. not for specific locations)
                // Set up the model grid using these locations
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations, RunGridCellsInParallel);

                EcologyTimer.Stop();
                Console.WriteLine("Time to initialise cells: {0}", EcologyTimer.GetElapsedTimeSecs());

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Madingley Model memory usage post grid cell seed: {0}", GC.GetTotalMemory(true) / 1E9, " (G Bytes)\n");
                Console.ForegroundColor = ConsoleColor.White;

            }

            // When the last simulation for the current scenario
            // if ((scenarioParameters.scenarioSimulationsNumber.Count == 1) && (scenarioIndex == scenarioParameters.scenarioSimulationsNumber[scenarioIndex] - 1)) EnviroStack.Clear();
            // Seed stocks and cohorts in the grid cells
            // If input state from output from a previous simulation
            if (initialisation.InputState)
            {
                EcosystemModelGrid.SeedGridCellStocksAndCohorts(_CellList, initialisation.ModelStates[simulation], CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions);

#if true
                GlobalDiagnosticVariables = initialisation.InputGlobalDiagnosticVariables;
#endif

                //remove cohorts that do not contain any biomass
                foreach (uint[] CellPair in _CellList)
                {

                    GridCellCohortHandler workingGridCellCohorts = EcosystemModelGrid.GetGridCellCohorts(CellPair[0], CellPair[1]);

                    for (int kk = 0; kk < CohortFunctionalGroupDefinitions.GetNumberOfFunctionalGroups(); kk++)
                    {
                        // Loop through each cohort in the functional group
                        for (int ll = (workingGridCellCohorts[kk].Count - 1); ll >= 0; ll--)
                        {
                            // If cohort abundance is less than the extinction threshold then add to the list for extinction
                            if (workingGridCellCohorts[kk][ll].CohortAbundance.CompareTo(0) <= 0 || workingGridCellCohorts[kk][ll].IndividualBodyMass.CompareTo(0.0) == 0)
                            {
                                // Remove the extinct cohort from the list of cohorts
                                workingGridCellCohorts[kk].RemoveAt(ll);
                            }
                        }

                    }
                }


            }
            else
            {
                EcosystemModelGrid.SeedGridCellStocksAndCohorts(_CellList, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, ref NextCohortID, InitialisationFileStrings["OutputDetail"] == "high", DrawRandomly,
                    initialisation.DispersalOnly, InitialisationFileStrings["DispersalOnlyType"], RunGridCellsInParallel);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Madingley Model memory usage pre Collect: {0}", Math.Round(GC.GetTotalMemory(true) / 1E9, 2), " (GBytes)");
            Console.ForegroundColor = ConsoleColor.White;
            GC.Collect();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Madingley Model memory usage post Collect: {0}", Math.Round(GC.GetTotalMemory(true) / 1E9, 5), " (GBytes)\n");
            Console.ForegroundColor = ConsoleColor.White;

        }

#if false
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
            GlobalOutputs.InitialOutputs(EcosystemModelGrid,CohortFunctionalGroupDefinitions,StockFunctionalGroupDefinitions,_CellList,
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
#endif

        /// <summary>
        /// Run processes for cells in parallel
        /// </summary>
#if true
        public void RunCellsInParallel(MadingleyModelInitialisation initialisation, Madingley.Common.IOutput output, IProgress<double> progress, System.Threading.CancellationToken cancellation)
#else
        public void RunCellsInParallel(MadingleyModelInitialisation initialisation)
#endif
        {

            // Create temporary variables to hold extinctions and productions in the parallel loop;
            int extinctions = 0, productions = 0, combinations = 0;

            if (initialisation.RunRealm == "")
            {
                // Run a parallel loop over rows
                Parallel.For(0, _CellList.Count, () => new ThreadLockedParallelVariables { Extinctions = 0, Productions = 0, Combinations = 0, NextCohortIDThreadLocked = NextCohortID }, (ii, loop, threadTrackedDiagnostics) =>
                {
                    RunCell(ii, threadTrackedDiagnostics, initialisation.DispersalOnly, initialisation);
                    return threadTrackedDiagnostics;
                },
                 (threadTrackedDiagnostics) =>
                 {
                     Interlocked.Add(ref extinctions, threadTrackedDiagnostics.Extinctions);
                     Interlocked.Add(ref productions, threadTrackedDiagnostics.Productions);
                     Interlocked.Add(ref combinations, threadTrackedDiagnostics.Combinations);
                     Interlocked.Exchange(ref NextCohortID, threadTrackedDiagnostics.NextCohortIDThreadLocked);
                 }
                 );


            }
            else
            {

                if (initialisation.RunRealm == "marine")
                {

                    // Run a parallel loop over rows
                    Parallel.For(0, _CellList.Count, () => new ThreadLockedParallelVariables { Extinctions = 0, Productions = 0, Combinations = 0, NextCohortIDThreadLocked = NextCohortID }, (ii, loop, threadTrackedDiagnostics) =>
                    {
                        if (EcosystemModelGrid.GetCellEnvironment(_CellList[ii][0], _CellList[ii][1])["Realm"][0] == 2.0) RunCell(ii, threadTrackedDiagnostics, initialisation.DispersalOnly, initialisation);
                        return threadTrackedDiagnostics;
                    },
                     (threadTrackedDiagnostics) =>
                     {
                         Interlocked.Add(ref extinctions, threadTrackedDiagnostics.Extinctions);
                         Interlocked.Add(ref productions, threadTrackedDiagnostics.Productions);
                         Interlocked.Add(ref combinations, threadTrackedDiagnostics.Combinations);
                         Interlocked.Exchange(ref NextCohortID, threadTrackedDiagnostics.NextCohortIDThreadLocked);
                     }
                     );
                }
                else if (initialisation.RunRealm == "terrestrial")
                {

                    // Run a parallel loop over rows
                    Parallel.For(0, _CellList.Count, () => new ThreadLockedParallelVariables { Extinctions = 0, Productions = 0, Combinations = 0, NextCohortIDThreadLocked = NextCohortID }, (ii, loop, threadTrackedDiagnostics) =>
                    {
                        if (EcosystemModelGrid.GetCellEnvironment(_CellList[ii][0], _CellList[ii][1])["Realm"][0] == 1.0) RunCell(ii, threadTrackedDiagnostics, initialisation.DispersalOnly, initialisation);
                        return threadTrackedDiagnostics;
                    },
                     (threadTrackedDiagnostics) =>
                     {
                         Interlocked.Add(ref extinctions, threadTrackedDiagnostics.Extinctions);
                         Interlocked.Add(ref productions, threadTrackedDiagnostics.Productions);
                         Interlocked.Add(ref combinations, threadTrackedDiagnostics.Combinations);
                         Interlocked.Exchange(ref NextCohortID, threadTrackedDiagnostics.NextCohortIDThreadLocked);
                     }
                     );
                }
                else
                {
                    Console.WriteLine("Run single realm needs to be 'marine', 'terrestrial', or blank");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("\n");

            // Take the results from the thread local variables and apply to the global diagnostic variables
            GlobalDiagnosticVariables["NumberOfCohortsExtinct"] = extinctions - combinations;
            GlobalDiagnosticVariables["NumberOfCohortsProduced"] = productions;
            GlobalDiagnosticVariables["NumberOfCohortsInModel"] = GlobalDiagnosticVariables["NumberOfCohortsInModel"] + productions - extinctions;
            GlobalDiagnosticVariables["NumberOfCohortsCombined"] = combinations;
        }

        /// <summary>
        /// Run processes for cells sequentially
        /// </summary>
#if true
        public void RunCellsSequentially(MadingleyModelInitialisation initialisation, Madingley.Common.IOutput output, IProgress<double> progress, System.Threading.CancellationToken cancellation)
#else
        public void RunCellsSequentially(MadingleyModelInitialisation initialisation)
#endif
        {
            // Instantiate a class to hold thread locked global diagnostic variables
            ThreadLockedParallelVariables singleThreadDiagnostics = new ThreadLockedParallelVariables { Extinctions = 0, Productions = 0, NextCohortIDThreadLocked = NextCohortID };

            if (initialisation.RunRealm == "")
            {

                for (int ii = 0; ii < _CellList.Count; ii++)
                {
#if true
                    if (cancellation.IsCancellationRequested)
                    {
                        output.EndRun();
                        Console.WriteLine("**** Cancelled!");
                    }
                    cancellation.ThrowIfCancellationRequested();
#endif
                    RunCell(ii, singleThreadDiagnostics, initialisation.DispersalOnly, initialisation);
                }
            }

            else
            {

                if (initialisation.RunRealm == "marine")
                {
                    for (int ii = 0; ii < _CellList.Count; ii++)
                    {
#if true
                        if (cancellation.IsCancellationRequested)
                        {
                            output.EndRun();
                            Console.WriteLine("**** Cancelled!");
                        }
                        cancellation.ThrowIfCancellationRequested();
#endif
                        if (EcosystemModelGrid.GetCellEnvironment(_CellList[ii][0], _CellList[ii][1])["Realm"][0] == 2.0) RunCell(ii, singleThreadDiagnostics, initialisation.DispersalOnly, initialisation);
                    }
                }
                else if (initialisation.RunRealm == "terrestrial")
                {
                    for (int ii = 0; ii < _CellList.Count; ii++)
                    {
#if true
                        if (cancellation.IsCancellationRequested)
                        {
                            output.EndRun();
                            Console.WriteLine("**** Cancelled!");
                        }
                        cancellation.ThrowIfCancellationRequested();
#endif
                        if (EcosystemModelGrid.GetCellEnvironment(_CellList[ii][0], _CellList[ii][1])["Realm"][0] == 1.0) RunCell(ii, singleThreadDiagnostics, initialisation.DispersalOnly, initialisation);
                    }
                }
                else
                {
                    Console.WriteLine("Run Single Realm needs to be 'marine', 'terrestrial', or blank");
                    Console.ReadKey();
                }
            }

            // Update the variable tracking cohort unique IDs
            NextCohortID = singleThreadDiagnostics.NextCohortIDThreadLocked;

            // Take the results from the thread local variables and apply to the global diagnostic variables
            GlobalDiagnosticVariables["NumberOfCohortsExtinct"] = singleThreadDiagnostics.Extinctions - singleThreadDiagnostics.Combinations;
            GlobalDiagnosticVariables["NumberOfCohortsProduced"] = singleThreadDiagnostics.Productions;
            GlobalDiagnosticVariables["NumberOfCohortsInModel"] = GlobalDiagnosticVariables["NumberOfCohortsInModel"] + singleThreadDiagnostics.Productions - singleThreadDiagnostics.Extinctions;
            GlobalDiagnosticVariables["NumberOfCohortsCombined"] = singleThreadDiagnostics.Combinations;
        }

        /// <summary>
        /// Run ecological processes for stocks in a specified grid cell
        /// </summary>
        /// <param name="latCellIndex">The latitudinal index of the cell to run stock ecology for</param>
        /// <param name="lonCellIndex">The longitudinal index of the cell to run stock ecology for</param>
        /// <param name="workingGridCellStocks">A copy of the cohorts in the current grid cell</param>
        /// <param name="cellIndex">The index of the current cell in the list of all cells to run the model for</param>
        /// <param name="initialisation">The Madingley Model initialisation</param>
        private void RunWithinCellStockEcology(uint latCellIndex, uint lonCellIndex, 
            GridCellStockHandler workingGridCellStocks, int cellIndex, MadingleyModelInitialisation initialisation)
        {
            // Create a local instance of the stock ecology class
            EcologyStock MadingleyEcologyStock = new EcologyStock();
            
            // Initialise stock ecology
            MadingleyEcologyStock.InitializeEcology();

            //The location of the acting stock
            int[] ActingStock = new int[2];

            // Get the list of functional group indices for autotroph stocks
            int[] AutotrophStockFunctionalGroups = StockFunctionalGroupDefinitions.GetFunctionalGroupIndex("Heterotroph/Autotroph", "Autotroph", false).
                ToArray();

            // Loop over autotroph functional groups
            foreach (int FunctionalGroup in AutotrophStockFunctionalGroups)
            {
                for (int ll = 0; ll < workingGridCellStocks[FunctionalGroup].Count; ll++)
                {
                    // Get the position of the acting stock
                    ActingStock[0] = FunctionalGroup;
                    ActingStock[1] = ll;

                    // Run stock ecology
                    MadingleyEcologyStock.RunWithinCellEcology(workingGridCellStocks, ActingStock, EcosystemModelGrid.GetCellEnvironment(
                        latCellIndex, lonCellIndex), EnvironmentalDataUnits, HumanNPPScenario, StockFunctionalGroupDefinitions,
                        CurrentTimeStep, NumBurninSteps, NumImpactSteps, initialisation.RecoveryTimeSteps, initialisation.InstantaneousTimeStep, initialisation.NumInstantaneousTimeStep, _GlobalModelTimeStepUnit, ProcessTrackers[cellIndex].TrackProcesses, ProcessTrackers[cellIndex],
                        TrackGlobalProcesses, CurrentMonth,
                        InitialisationFileStrings["OutputDetail"],SpecificLocations,((initialisation.ImpactCellIndices.Contains((uint)cellIndex) || (initialisation.ImpactAll))));

                }
            }

        }

        private void RunWithinCellCohortEcology(uint latCellIndex, uint lonCellIndex, ThreadLockedParallelVariables partial, 
            GridCellCohortHandler workingGridCellCohorts, GridCellStockHandler workingGridCellStocks,string outputDetail, int cellIndex, MadingleyModelInitialisation initialisation)
        {


            // Local instances of classes
            EcologyCohort MadingleyEcologyCohort = new EcologyCohort();
            Activity CohortActivity = new Activity();
            CohortMerge CohortMerger = new CohortMerge(DrawRandomly);

            // A list of the original cohorts inside a particular grid cell
            int[] OriginalGridCellCohortsNumbers;
            // A vector to hold the order in which cohorts will act
            uint[] RandomCohortOrder;
            // A jagged array to keep track of cohorts that are being worked on
            uint[][] CohortIndices;
            // The location of the acting cohort
            int[] ActingCohort = new int[2];
            // Temporary local variables
            int EcosystemModelParallelTempval1;
            int EcosystemModelParallelTempval2;
            // variable to track cohort number
            uint TotalCohortNumber = 0;

            // Fill in the array with the number of cohorts per functional group before ecological processes are run
            OriginalGridCellCohortsNumbers = new int[workingGridCellCohorts.Count];

            for (int i = 0; i < workingGridCellCohorts.Count; i++)
            {
                OriginalGridCellCohortsNumbers[i] = workingGridCellCohorts[i].Count;
            }

            // Initialize ecology for stocks and cohorts
            MadingleyEcologyCohort.InitializeEcology(EcosystemModelGrid.GetCellEnvironment(latCellIndex, lonCellIndex)["Cell Area"][0],
                _GlobalModelTimeStepUnit, DrawRandomly);

            // Create a jagged array indexed by functional groups to hold cohort indices
            CohortIndices = new uint[CohortFunctionalGroupDefinitions.GetNumberOfFunctionalGroups()][];

            // Loop over functional groups
            for (int ll = 0; ll < CohortFunctionalGroupDefinitions.GetNumberOfFunctionalGroups(); ll++)
            {
                // Dimension the number of columns in each row of the jagged array to equal number of gridCellCohorts in each functional group
                if (workingGridCellCohorts[ll] == null)
                {
                    CohortIndices[ll] = new uint[0];
                }
                else
                {
                    CohortIndices[ll] = new uint[workingGridCellCohorts[ll].Count()];
                }
                // Loop over gridCellCohorts in the functional group
                for (int kk = 0; kk < CohortIndices[ll].Count(); kk++)
                {
                    // Fill jagged array with indices for each cohort
                    CohortIndices[ll][kk] = TotalCohortNumber;
                    TotalCohortNumber += 1;
                }

            }

            if (DrawRandomly)
            {
                // Randomly order the cohort indices
                RandomCohortOrder = Utilities.RandomlyOrderedIndices(TotalCohortNumber);
            }
            else
            {
                RandomCohortOrder = Utilities.NonRandomlyOrderedCohorts(TotalCohortNumber, CurrentTimeStep);
            }

            // Diagnostic biological variables don't need to be reset every cohort, but rather every grid cell
            EcosystemModelParallelTempval2 = 0;

            // Initialise eating formulations
            MadingleyEcologyCohort.EatingFormulations["Basic eating"].InitializeEcologicalProcess(workingGridCellCohorts, workingGridCellStocks,
                CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, "revised predation");
            MadingleyEcologyCohort.EatingFormulations["Basic eating"].InitializeEcologicalProcess(workingGridCellCohorts, workingGridCellStocks
                , CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, "revised herbivory");

            // Loop over randomly ordered gridCellCohorts to implement biological functions
            for (int ll = 0; ll < RandomCohortOrder.Length; ll++)
            {

                // Locate the randomly chosen cohort within the array of lists of gridCellCohorts in the grid cell
                ActingCohort = Utilities.FindJaggedArrayIndex(RandomCohortOrder[ll], CohortIndices, TotalCohortNumber);

                // Perform all biological functions except dispersal (which is cross grid cell)
                if (workingGridCellCohorts[ActingCohort].CohortAbundance.CompareTo(_ExtinctionThreshold) > 0)
                {
                    // Calculate number of cohorts in this functional group in this grid cell before running ecology
                    EcosystemModelParallelTempval1 = workingGridCellCohorts[ActingCohort[0]].Count;

                    CohortActivity.AssignProportionTimeActive(workingGridCellCohorts[ActingCohort], EcosystemModelGrid.GetCellEnvironment(latCellIndex, lonCellIndex), CohortFunctionalGroupDefinitions, CurrentTimeStep, CurrentMonth);

                    // Run ecology
                    MadingleyEcologyCohort.RunWithinCellEcology(workingGridCellCohorts, workingGridCellStocks,
                        ActingCohort, EcosystemModelGrid.GetCellEnvironment(latCellIndex, lonCellIndex),
                        EcosystemModelGrid.GetCellDeltas(latCellIndex, lonCellIndex),
                        CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, CurrentTimeStep,
                        ProcessTrackers[cellIndex], ref partial, SpecificLocations,outputDetail, CurrentMonth, initialisation);

                    // Update the properties of the acting cohort
                    MadingleyEcologyCohort.UpdateEcology(workingGridCellCohorts, workingGridCellStocks, ActingCohort,
                        EcosystemModelGrid.GetCellEnvironment(latCellIndex, lonCellIndex), EcosystemModelGrid.GetCellDeltas(
                        latCellIndex, lonCellIndex), CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, CurrentTimeStep,
                        ProcessTrackers[cellIndex]);

                    // Add newly produced cohorts to the tracking variable
                    EcosystemModelParallelTempval2 += workingGridCellCohorts[ActingCohort[0]].Count - EcosystemModelParallelTempval1;
                    

                    // Check that the mass of individuals in this cohort is still >= 0 after running ecology
                    Debug.Assert(workingGridCellCohorts[ActingCohort].IndividualBodyMass >= 0.0, "Biomass < 0 for this cohort");
                }

                // Check that the mass of individuals in this cohort is still >= 0 after running ecology
                Debug.Assert(workingGridCellCohorts[ActingCohort].IndividualBodyMass >= 0.0, "Biomass < 0 for this cohort");
            }


            // Update diagnostics of productions
            partial.Productions += EcosystemModelParallelTempval2;

            RunExtinction(latCellIndex, lonCellIndex, partial, workingGridCellCohorts, cellIndex);



            // Merge cohorts, if necessary
            if (workingGridCellCohorts.GetNumberOfCohorts() > initialisation.MaxNumberOfCohorts)
            {
                partial.Combinations = CohortMerger.MergeToReachThresholdFast(workingGridCellCohorts, workingGridCellCohorts.GetNumberOfCohorts(), initialisation.MaxNumberOfCohorts);

                //Run extinction a second time to remove those cohorts that have been set to zero abundance when merging
                RunExtinction(latCellIndex, lonCellIndex, partial, workingGridCellCohorts, cellIndex);
            }
            else
                partial.Combinations = 0;
            
            // Write out the updated cohort numbers after all ecological processes have occured
            EcosystemModelGrid.SetGridCellCohorts(workingGridCellCohorts, latCellIndex, lonCellIndex);
        }

        /// <summary>
        /// Carries out extinction on cohorts that have an abundance below a defined extinction threshold
        /// </summary>
        private void RunExtinction(uint latCellIndex, uint lonCellIndex, ThreadLockedParallelVariables partial,
            GridCellCohortHandler workingGridCellCohorts, int cellIndex)
        {
            bool VarExists;

            // Loop over cohorts and remove any whose abundance is below the extinction threshold
            for (int kk = 0; kk < CohortFunctionalGroupDefinitions.GetNumberOfFunctionalGroups(); kk++)
            {
                // Create a list to hold the cohorts to remove
                List<int> CohortIndicesToRemove = new List<int>();

                // Loop through each cohort in the functional group
                for (int ll = 0; ll < workingGridCellCohorts[kk].Count; ll++)
                {
                    // If cohort abundance is less than the extinction threshold then add to the list for extinction
                    if (workingGridCellCohorts[kk][ll].CohortAbundance.CompareTo(_ExtinctionThreshold) <= 0 || workingGridCellCohorts[kk][ll].IndividualBodyMass.CompareTo(0.0) == 0)
                    {
                        CohortIndicesToRemove.Add(ll);

                        partial.Extinctions += 1;

                        // If track processes is set and output detail is set to high and the cohort being made extinct has never been merged,
                        // then output its mortality profile
                        if (ProcessTrackers[cellIndex].TrackProcesses && (InitialisationFileStrings["OutputDetail"] == "high") && (workingGridCellCohorts[kk][ll].CohortID.Count == 1))
                        {
                            ProcessTrackers[cellIndex].OutputMortalityProfile(workingGridCellCohorts[kk][ll].CohortID[0]);
                        }
                    }
                }

                // Code to add the biomass to the biomass pool and dispose of the cohort
                for (int ll = (CohortIndicesToRemove.Count - 1); ll >= 0; ll--)
                {
                    // Add biomass of the extinct cohort to the organic matter pool
                    EcosystemModelGrid.SetEnviroLayer("Organic Pool", 0, EcosystemModelGrid.GetEnviroLayer("Organic Pool", 0, latCellIndex, lonCellIndex, out VarExists) +
                        (workingGridCellCohorts[kk][CohortIndicesToRemove[ll]].IndividualBodyMass + workingGridCellCohorts[kk][CohortIndicesToRemove[ll]].IndividualReproductivePotentialMass) * workingGridCellCohorts[kk][CohortIndicesToRemove[ll]].CohortAbundance, latCellIndex, lonCellIndex);
                    Debug.Assert(EcosystemModelGrid.GetEnviroLayer("Organic Pool", 0, latCellIndex, lonCellIndex, out VarExists) > 0, "Organic pool < 0");

                    if (ProcessTrackers[cellIndex].TrackProcesses && SpecificLocations == true)
                        ProcessTrackers[cellIndex].RecordExtinction(latCellIndex, lonCellIndex, CurrentTimeStep, workingGridCellCohorts[kk][CohortIndicesToRemove[ll]].Merged, workingGridCellCohorts[kk][CohortIndicesToRemove[ll]].CohortID);

                    // Remove the extinct cohort from the list of cohorts
                    workingGridCellCohorts[kk].RemoveAt(CohortIndicesToRemove[ll]);


                }

            }

        }

        private void RunWithinCellDispersalOnly(uint latCellIndex, uint lonCellIndex, ThreadLockedParallelVariables partial,
                   GridCellCohortHandler workingGridCellCohorts, GridCellStockHandler workingGridCellStocks)
        {
            // Merge cohorts. Requires cohorts to be identical, for testing purposes (remember that they don't grow etc)
            // SHOULD ONLY BE RUN FOR RESPONSIVE DISPERSAL TESTING
            //partial.Combinations = Merger.MergeForResponsiveDispersalOnly(workingGridCellCohorts);

            // Loop over cohorts and remove any whose abundance is below the extinction threshold
            for (int kk = 0; kk < CohortFunctionalGroupDefinitions.GetNumberOfFunctionalGroups(); kk++)
            {
                // Create a list to hold the cohorts to remove
                List<int> CohortIndicesToRemove = new List<int>();

                // Loop through each cohort in the functional group
                for (int ll = 0; ll < workingGridCellCohorts[kk].Count; ll++)
                {
                    // If cohort abundance is less than the extinction threshold then add to the list for extinction
                    if (workingGridCellCohorts[kk][ll].CohortAbundance <= _ExtinctionThreshold)
                    {
                        CohortIndicesToRemove.Add(ll);

                        partial.Extinctions += 1;
                    }
                }

                // Note that we don't keep track of the organic biomass pool if running dispersal only, since there are cohorts with strange biomasses
                for (int ll = (CohortIndicesToRemove.Count - 1); ll >= 0; ll--)
                {
                    // Remove the extinct cohort from the list of cohorts
                    workingGridCellCohorts[kk].RemoveAt(CohortIndicesToRemove[ll]);
                }

            }


            // Write out the updated cohort numbers after all ecological processes have occured
            EcosystemModelGrid.SetGridCellCohorts(workingGridCellCohorts, latCellIndex, lonCellIndex);
        }


        /// <summary>
        /// Run ecological processes that operate across grid cells
        /// </summary>
        public void RunCrossGridCellEcology(ref uint dispersals, bool dispersalOnly, MadingleyModelInitialisation modelInitialisation)
        {
            // If we are running specific locations, then we do not run dispersal
            if (SpecificLocations != true)
            {
                if (RunGridCellsInParallel)
                {
                    // Loop through each grid cell, and run dispersal for each. Note that because cells essentially run independently, we do not need to thread-lock variables 
                    // as they do not exchange information until they are all completed (they basically just build up delta arrays of cohorts to move). However, if cells
                    // should start to exchange information for dispersal, or all contribute to a single centralised variable, then thread-locked parallel variables would
                    // have to be used.
                     Parallel.For(0, _CellList.Count, ii =>
                    {
                        EcologyCrossGridCell TempMadingleyEcologyCrossGridCell = new EcologyCrossGridCell();
                        // Initialise cross grid cell ecology
                        TempMadingleyEcologyCrossGridCell.InitializeCrossGridCellEcology(_GlobalModelTimeStepUnit, DrawRandomly, modelInitialisation);

                        //Initialise the delta for dispersal lists for this grid cell
                        EcosystemModelGrid.DeltaFunctionalGroupDispersalArray[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();
                        EcosystemModelGrid.DeltaCohortNumberDispersalArray[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();
                        EcosystemModelGrid.DeltaCellToDisperseToArray[_CellList[ii][0], _CellList[ii][1]] = new List<uint[]>();

                        EcosystemModelGrid.DeltaCellExitDirection[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();
                        EcosystemModelGrid.DeltaCellEntryDirection[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();

                        // We have looped through individal cells and calculated ecological processes for each. Now do this for cross grid cell processes
                        TempMadingleyEcologyCrossGridCell.RunCrossGridCellEcology(_CellList[ii], EcosystemModelGrid, dispersalOnly,
                            CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, CurrentMonth);
                    });
                }
                else
                {
                    // Loop through each grid cell, and run dispersal for each.
                    // Note that currently dispersal is not parallelised, although it could be (though care would need to be taken to ensure that necessary variables are thread-locked
                    for (int ii = 0; ii < _CellList.Count; ii++)
                    {
                        //Initialise the delta for dispersal lists for this grid cell
                        EcosystemModelGrid.DeltaFunctionalGroupDispersalArray[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();
                        EcosystemModelGrid.DeltaCohortNumberDispersalArray[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();
                        EcosystemModelGrid.DeltaCellToDisperseToArray[_CellList[ii][0], _CellList[ii][1]] = new List<uint[]>();

                        EcosystemModelGrid.DeltaCellExitDirection[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();
                        EcosystemModelGrid.DeltaCellEntryDirection[_CellList[ii][0], _CellList[ii][1]] = new List<uint>();

                        // We have looped through individal cells and calculated ecological processes for each. Now do this for cross grid cell processes
                        MadingleyEcologyCrossGridCell.RunCrossGridCellEcology(_CellList[ii], EcosystemModelGrid, dispersalOnly,
                            CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions, CurrentMonth);
                    }
                }
                // Apply the changes in the delta arrays from dispersal
                MadingleyEcologyCrossGridCell.UpdateCrossGridCellEcology(EcosystemModelGrid, ref dispersals, TrackCrossCellProcesses, CurrentTimeStep);
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
        public void DebugPrintModel(string filePrefix, uint timeStep)
        {
            if (System.IO.Directory.Exists(filePrefix))
            {
                var fileName = string.Format("{0}/{1}.txt", filePrefix, timeStep);

                using (var f = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var w = new StreamWriter(f))
                    {
                        var sb = new StringBuilder();

                        sb.AppendLine(String.Format("{{"));

                        JsonAddObject(sb, "MadingleyModel", ToString(this));

                        sb.AppendLine(String.Format("}}"));

                        w.Write(sb.ToString());
                    }
                }
            }
        }

        public string EnviroStackToString()
        {
            var sb = new StringBuilder();

            /*
            this.EnviroStack.ToList().ForEach(
                trait =>
                    sb.AppendLine(String.Format("{0},", KeyValueListListToString(trait)))
            );
            */
            sb.AppendLine("<< ENVIRON >>");

            return sb.ToString();
        }

        public string CellListToString()
        {
            var sb = new StringBuilder();

            this._CellList.ToList().ForEach(
                trait =>
                    sb.AppendLine(String.Format("[{0},{1}],", trait[0], trait[1]))
            );

            return sb.ToString();
        }

        public static string TToString<T>(T t)
        {
            return t.ToString();
        }

        public static string ToString(MadingleyModel m)
        {
            var sb = new StringBuilder();

            JsonAddObject(sb, "CohortFunctionalGroupDefinitions", FunctionalGroupDefinitions.ToString(m.CohortFunctionalGroupDefinitions));
            JsonAddObject(sb, "StockFunctionalGroupDefinitions", FunctionalGroupDefinitions.ToString(m.StockFunctionalGroupDefinitions));
            JsonAddArray(sb, "EnviroStack", m.EnviroStackToString());
            JsonAddObject(sb, "EcosystemModelGrid", ModelGrid.ToString(m.EcosystemModelGrid));
            //JsonAddObject(sb, "MadingleyEcologyCrossGridCell", EcologyCrossGridCell.ToString(m.MadingleyEcologyCrossGridCell));
            JsonAddProperty(sb, "BottomLatitude", m.BottomLatitude);
            JsonAddProperty(sb, "TopLatitude", m.TopLatitude);
            JsonAddProperty(sb, "LeftmostLongitude", m.LeftmostLongitude);
            JsonAddProperty(sb, "RightmostLongitude", m.RightmostLongitude);
            JsonAddProperty(sb, "CellSize", m.CellSize);
            JsonAddProperty(sb, "NumBurninSteps", m.NumBurninSteps);
            JsonAddProperty(sb, "NumImpactSteps", m.NumImpactSteps);
            JsonAddProperty(sb, "NumRecoverySteps", m.NumRecoverySteps);
            JsonAddProperty(sb, "CurrentTimeStep", m.CurrentTimeStep);
            JsonAddProperty(sb, "CurrentMonth", m.CurrentMonth);
            JsonAddProperty(sb, "DrawRandomly", m.DrawRandomly);
            JsonAddProperty(sb, "ExtinctionThreshold", m.ExtinctionThreshold);
            JsonAddProperty(sb, "MergeDifference", m.MergeDifference);
            JsonAddProperty(sb, "GlobalModelTimeStepUnit", m.GlobalModelTimeStepUnit);
            JsonAddArray(sb, "_CellList", m.CellListToString());
            JsonAddArray(sb, "GlobalDiagnosticVariables", KeyValueListToString(m.GlobalDiagnosticVariables, TToString<double>));
            JsonAddProperty(sb, "RunGridCellsInParallel", m.RunGridCellsInParallel);
            JsonAddProperty(sb, "SpecificLocations", m.SpecificLocations);
            JsonAddArray(sb, "InitialisationFileStrings", KeyValueListToString(m.InitialisationFileStrings, TToString<string>));
            JsonAddArray(sb, "EnvironmentalDataUnits", KeyValueListToString(m.EnvironmentalDataUnits, TToString<string>));
            JsonAddProperty(sb, "HumanNPPScenario", m.HumanNPPScenario);
            JsonAddProperty(sb, "TemperatureScenario", m.TemperatureScenario);
            JsonAddProperty(sb, "HarvestingScenario", m.HarvestingScenario);
            JsonAddProperty(sb, "NextCohortID", m.NextCohortID);
            JsonAddProperty(sb, "Dispersals", m.Dispersals);

            return sb.ToString();
        }

        public static void JsonAddProperty(StringBuilder sb, string name, double value)
        {
            sb.AppendLine(String.Format("{0}: {1:G17},", name, value));
        }

        public static void JsonAddProperty<TValue>(StringBuilder sb, string name, TValue value)
        {
            sb.AppendLine(String.Format("{0}: {1},", name, value));
        }

        public static void JsonAddArray(StringBuilder sb, string name, string value)
        {
            var lines = value.TrimEnd().Split('\n');

            if (lines.Count() == 0)
            {
                sb.AppendLine(String.Format("{0}: [],", name));
            }
            else if (lines.Count() == 1)
            {
                sb.AppendLine(String.Format("{0}: [{1}],", name, lines[0].TrimEnd()));
            }
            else
            {
                sb.AppendLine(String.Format("{0}: [", name));

                lines.ToList().ForEach(line => sb.AppendLine(String.Format("  {0}", line.TrimEnd())));

                sb.AppendLine(String.Format("],"));
            }
        }

        public static void JsonAddObject(StringBuilder sb, string name, string value)
        {
            var lines = value.TrimEnd().Split('\n');

            if (lines.Count() == 0)
            {
                sb.AppendLine(String.Format("{0}: {{}},", name));
            }
            else if (lines.Count() == 1)
            {
                sb.AppendLine(String.Format("{0}: {{{1}}},", name, lines[0].TrimEnd()));
            }
            else
            {
                sb.AppendLine(String.Format("{0}: {{", name));

                lines.ToList().ForEach(line => sb.AppendLine(String.Format("  {0}", line.TrimEnd())));

                sb.AppendLine(String.Format("}},"));
            }
        }

        public static void JsonAddArrayValue(StringBuilder sb, string value)
        {
            var lines = value.TrimEnd().Split('\n');

            if (lines.Count() == 0)
            {
                sb.AppendLine(String.Format("[],"));
            }
            else if (lines.Count() == 1)
            {
                sb.AppendLine(String.Format("[{0}],", lines[0].TrimEnd()));
            }
            else
            {
                sb.AppendLine(String.Format("["));

                lines.ToList().ForEach(line => sb.AppendLine(String.Format("  {0}", line.TrimEnd())));

                sb.AppendLine(String.Format("],"));
            }
        }

        public static void JsonAddObjectValue(StringBuilder sb, string value)
        {
            var lines = value.TrimEnd().Split('\n');

            if (lines.Count() == 0)
            {
                sb.AppendLine(String.Format("{{}},"));
            }
            else if (lines.Count() == 1)
            {
                sb.AppendLine(String.Format("{{{0}}},", lines[0].TrimEnd()));
            }
            else
            {
                sb.AppendLine(String.Format("{{"));

                lines.ToList().ForEach(line => sb.AppendLine(String.Format("  {0}", line.TrimEnd())));

                sb.AppendLine(String.Format("}},"));
            }
        }

        public static string KeyValueListListToString<TValue>(IEnumerable<KeyValuePair<string, TValue[]>> pairs, Func<TValue, string> formatter)
        {
            var sb = new StringBuilder();

            pairs.ToList().ForEach(
                trait =>
                    {
                        var vs = trait.Value.Select(v => formatter.Invoke(v));
                        var s = string.Join(",", vs);

                        JsonAddArray(sb, trait.Key, s);
                    }
            );

            return sb.ToString();
        }

        public static string KeyValueListToString<TValue>(IEnumerable<KeyValuePair<string, TValue>> pairs, Func<TValue, string> formatter)
        {
            var sb = new StringBuilder();

            pairs.ToList().ForEach(
                trait => JsonAddProperty(sb, trait.Key, formatter.Invoke(trait.Value))
            );

            return sb.ToString();
        }

        public static string IndexLookupFromTraitToString(IEnumerable<KeyValuePair<string, SortedDictionary<string, int[]>>> dict)
        {
            var sb = new StringBuilder();

            dict.ToList().ForEach(
                trait =>
                    JsonAddArray(sb, trait.Key, KeyValueListListToString<int>(trait.Value, TToString<int>))
            );

            return sb.ToString();
        }

        public static string Array2DToString<TValue>(TValue[,] array, Func<TValue, string> formatter)
        {
            var sb = new StringBuilder();

            for (var ii = 0; ii < array.GetLength(0); ii++)
            {
                for (var jj = 0; jj < array.GetLength(1); jj++)
                {
                    if (array[ii, jj] != null)
                    {
                        sb.AppendFormat("{{ {0} }},", formatter.Invoke(array[ii, jj]));
                    }
                }
            }

            return sb.ToString();
        }

#endif
    }

}