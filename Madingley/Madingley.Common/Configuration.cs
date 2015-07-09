using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Initialization information for Madingley model simulations.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// String identifying time step units to be used by the simulations.
        /// </summary>
        public string GlobalModelTimeStepUnit { get; set; }

        /// <summary>
        /// Number of time steps to be run in the simulations.
        /// </summary>
        public int NumTimeSteps { get; set; }

        /// <summary>
        /// Number of time steps to run the model for before any impacts are applied.
        /// </summary>
        public int BurninTimeSteps { get; set; }

        /// <summary>
        /// For scenarios with temporary impacts, the number of time steps to apply the impact for.
        /// </summary>
        public int ImpactTimeSteps { get; set; }

        /// <summary>
        /// For scenarios with temporary impacts, the number of time steps to apply the impact for.
        /// </summary>
        public int RecoveryTimeSteps { get; set; }

        /// <summary>
        /// Whether to run the model for different grid cells in parallel?
        /// </summary>
        public bool RunCellsInParallel { get; set; }

        /// <summary>
        /// Whether to run the model for different simulations in parallel?
        /// </summary>
        public bool RunSimulationsInParallel { get; set; }

        /// <summary>
        /// Which realm to run the model for.
        /// </summary>
        public string RunRealm { get; set; }

        /// <summary>
        /// Whether to draw cohort properties randomly when seeding them, and whether cohorts will undergo ecological processes in a random order?
        /// </summary>
        /// <remarks>Value should be set in initialization file, but default value is true</remarks>
        public bool DrawRandomly { get; set; }

        /// <summary>
        /// The threshold abundance below which cohorts will be made extinct.
        /// </summary>
        public double ExtinctionThreshold { get; set; }

        /// <summary>
        /// The maximum number of cohorts to be in the model, per grid cell, when it is running.
        /// </summary>
        public int MaxNumberOfCohorts { get; set; }

        /// <summary>
        /// Whether to run only dispersal (i.e. turn all other ecological processes off, and set dispersal probability to one temporarily).
        /// </summary>
        public bool DispersalOnly { get; set; }

        /// <summary>
        /// If DispersalOnly, which dispersal type?
        /// </summary>
        public string DispersalOnlyType { get; set; }

        /// <summary>
        /// The weight threshold (grams) below which marine organisms that are not obligate zooplankton will be dispersed planktonically.
        /// </summary>
        public double PlanktonDispersalThreshold { get; set; }

        /// <summary>
        /// The functional group definitions of cohorts in the model.
        /// </summary>
        public FunctionalGroupDefinitions CohortFunctionalGroupDefinitions { get; set; }

        /// <summary>
        /// The functional group definitions of stocks in the model.
        /// </summary>
        public FunctionalGroupDefinitions StockFunctionalGroupDefinitions { get; set; }

        /// <summary>
        /// For scenarios with temporary impacts, the cells to apply impacts to (unless ImpactAll).
        /// </summary>
        public IEnumerable<int> ImpactCellIndices { get; set; }

        /// <summary>
        /// For scenarios with temporary impacts, apply impacts to all cells?
        /// </summary>
        public bool ImpactAll { get; set; }

        /// <summary>
        /// All available ScenarioParameters.
        /// </summary>
        public IList<ScenarioParameters> ScenarioParameters { get; set; }

        /// <summary>
        /// Which ScenarioParameter to use?
        /// </summary>
        public int ScenarioIndex { get; set; }

        /// <summary>
        /// Simulation number, only used for formatting output.
        /// </summary>
        public int Simulation { get; set; }

        /// <summary>
        /// EcologicalParameters to use.
        /// </summary>
        public EcologicalParameters EcologicalParameters { get; set; }

        /// <summary>
        /// Configuration constructor.
        /// </summary>
        /// <param name="globalModelTimeStepUnit">Time step units to be used.</param>
        /// <param name="numTimeSteps">Number of time steps to be run.</param>
        /// <param name="burninTimeSteps">Number of time steps before any impacts are applied.</param>
        /// <param name="impactTimeSteps">Number of time steps to apply the impact for.</param>
        /// <param name="recoveryTimeSteps">Number of time steps to apply the impact for.</param>
        /// <param name="runCellsInParallel">Whether to run the model for different grid cells in parallel?</param>
        /// <param name="runSimulationsInParallel">Whether to run the model for different simulations in parallel?</param>
        /// <param name="runRealm">Which realm to run the model for.</param>
        /// <param name="drawRandomly">Whether to draw cohort properties randomly when seeding them, and whether cohorts will undergo ecological processes in a random order.</param>
        /// <param name="extinctionThreshold">Threshold abundance below which cohorts will be made extinct.</param>
        /// <param name="maxNumberOfCohorts">Maximum number of cohorts to be in the model, per grid cell, when it is running.</param>
        /// <param name="dispersalOnly">Whether to run only dispersal (i.e. turn all other ecological processes off, and set dispersal probability to one temporarily).</param>
        /// <param name="dispersalOnlyType">If DispersalOnly, which dispersal type?</param>
        /// <param name="planktonDispersalThreshold">Weight threshold (grams) below which marine organisms that are not obligate zooplankton will be dispersed planktonically.</param>
        /// <param name="cohortFunctionalGroupDefinitions">Functional group definitions of cohorts in the model.</param>
        /// <param name="stockFunctionalGroupDefinitions">Functional group definitions of stocks in the model.</param>
        /// <param name="impactCellIndices">Cells to apply impacts to (unless ImpactAll).</param>
        /// <param name="impactAll">Apply impacts to all cells?</param>
        /// <param name="scenarioParameters">All available ScenarioParameters.</param>
        /// <param name="scenarioIndex">Which ScenarioParameter to use?</param>
        /// <param name="simulation">Simulation number, only used for formatting output.</param>
        /// <param name="ecologicalParameters">EcologicalParameters to use.</param>
        public Configuration(
            string globalModelTimeStepUnit,
            int numTimeSteps,
            int burninTimeSteps,
            int impactTimeSteps,
            int recoveryTimeSteps,
            bool runCellsInParallel,
            bool runSimulationsInParallel,
            string runRealm,
            bool drawRandomly,
            double extinctionThreshold,
            int maxNumberOfCohorts,
            bool dispersalOnly,
            string dispersalOnlyType,
            double planktonDispersalThreshold,
            FunctionalGroupDefinitions cohortFunctionalGroupDefinitions,
            FunctionalGroupDefinitions stockFunctionalGroupDefinitions,
            IEnumerable<int> impactCellIndices,
            bool impactAll,
            IEnumerable<ScenarioParameters> scenarioParameters,
            int scenarioIndex,
            int simulation,
            EcologicalParameters ecologicalParameters)
        {
            this.GlobalModelTimeStepUnit = globalModelTimeStepUnit;
            this.NumTimeSteps = numTimeSteps;
            this.BurninTimeSteps = burninTimeSteps;
            this.ImpactTimeSteps = impactTimeSteps;
            this.RecoveryTimeSteps = recoveryTimeSteps;
            this.RunCellsInParallel = runCellsInParallel;
            this.RunSimulationsInParallel = runSimulationsInParallel;
            this.RunRealm = runRealm;
            this.DrawRandomly = drawRandomly;
            this.ExtinctionThreshold = extinctionThreshold;
            this.MaxNumberOfCohorts = maxNumberOfCohorts;
            this.DispersalOnly = dispersalOnly;
            this.DispersalOnlyType = dispersalOnlyType;
            this.PlanktonDispersalThreshold = planktonDispersalThreshold;
            this.CohortFunctionalGroupDefinitions = new FunctionalGroupDefinitions(cohortFunctionalGroupDefinitions);
            this.StockFunctionalGroupDefinitions = new FunctionalGroupDefinitions(stockFunctionalGroupDefinitions);
            this.ImpactCellIndices = impactCellIndices.ToArray();
            this.ImpactAll = impactAll;
            this.ScenarioParameters = scenarioParameters.Select(s => new ScenarioParameters(s)).ToArray();
            this.ScenarioIndex = scenarioIndex;
            this.Simulation = simulation;
            this.EcologicalParameters = new EcologicalParameters(ecologicalParameters);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="configuration">Configuration to copy.</param>
        public Configuration(Configuration configuration)
        {
            this.GlobalModelTimeStepUnit = configuration.GlobalModelTimeStepUnit;
            this.NumTimeSteps = configuration.NumTimeSteps;
            this.BurninTimeSteps = configuration.BurninTimeSteps;
            this.ImpactTimeSteps = configuration.ImpactTimeSteps;
            this.RecoveryTimeSteps = configuration.RecoveryTimeSteps;
            this.RunCellsInParallel = configuration.RunCellsInParallel;
            this.RunSimulationsInParallel = configuration.RunSimulationsInParallel;
            this.RunRealm = configuration.RunRealm;
            this.DrawRandomly = configuration.DrawRandomly;
            this.ExtinctionThreshold = configuration.ExtinctionThreshold;
            this.MaxNumberOfCohorts = configuration.MaxNumberOfCohorts;
            this.DispersalOnly = configuration.DispersalOnly;
            this.DispersalOnlyType = configuration.DispersalOnlyType;
            this.PlanktonDispersalThreshold = configuration.PlanktonDispersalThreshold;
            this.MaxNumberOfCohorts = configuration.MaxNumberOfCohorts;
            this.CohortFunctionalGroupDefinitions = new FunctionalGroupDefinitions(configuration.CohortFunctionalGroupDefinitions);
            this.StockFunctionalGroupDefinitions = new FunctionalGroupDefinitions(configuration.StockFunctionalGroupDefinitions);
            this.ImpactCellIndices = configuration.ImpactCellIndices.ToArray();
            this.ImpactAll = configuration.ImpactAll;
            this.ScenarioParameters = configuration.ScenarioParameters.Select(s => new ScenarioParameters(s)).ToArray();
            this.ScenarioIndex = configuration.ScenarioIndex;
            this.Simulation = configuration.Simulation;
            this.EcologicalParameters = new EcologicalParameters(configuration.EcologicalParameters);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both Configurations and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var configurationObj = obj as Configuration;
            if ((Object)configurationObj == null) return false;

            IEqualityComparer<double> compareFloats = new FixedDoubleComparer();

            return
                this.GlobalModelTimeStepUnit.Equals(configurationObj.GlobalModelTimeStepUnit) &&
                this.NumTimeSteps.Equals(configurationObj.NumTimeSteps) &&
                this.BurninTimeSteps.Equals(configurationObj.BurninTimeSteps) &&
                this.ImpactTimeSteps.Equals(configurationObj.ImpactTimeSteps) &&
                this.RecoveryTimeSteps.Equals(configurationObj.RecoveryTimeSteps) &&
                this.RunCellsInParallel.Equals(configurationObj.RunCellsInParallel) &&
                this.RunSimulationsInParallel.Equals(configurationObj.RunSimulationsInParallel) &&
                this.RunRealm.Equals(configurationObj.RunRealm) &&
                this.DrawRandomly.Equals(configurationObj.DrawRandomly) &&
                compareFloats.Equals(this.ExtinctionThreshold, configurationObj.ExtinctionThreshold) &&
                this.MaxNumberOfCohorts.Equals(configurationObj.MaxNumberOfCohorts) &&
                this.DispersalOnly.Equals(configurationObj.DispersalOnly) &&
                this.DispersalOnlyType.Equals(configurationObj.DispersalOnlyType) &&
                compareFloats.Equals(this.PlanktonDispersalThreshold, configurationObj.PlanktonDispersalThreshold) &&
                this.CohortFunctionalGroupDefinitions.Equals(configurationObj.CohortFunctionalGroupDefinitions) &&
                this.StockFunctionalGroupDefinitions.Equals(configurationObj.StockFunctionalGroupDefinitions) &&
                this.ImpactCellIndices.SequenceEqual(configurationObj.ImpactCellIndices) &&
                this.ImpactAll.Equals(configurationObj.ImpactAll) &&
                this.ScenarioParameters.SequenceEqual(configurationObj.ScenarioParameters, new ScenarioParametersComparer(compareFloats)) &&
                this.ScenarioIndex.Equals(configurationObj.ScenarioIndex) &&
                this.Simulation.Equals(configurationObj.Simulation) &&
                this.EcologicalParameters.Equals(configurationObj.EcologicalParameters);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.GlobalModelTimeStepUnit.GetHashCode() ^
                this.NumTimeSteps.GetHashCode() ^
                this.BurninTimeSteps.GetHashCode() ^
                this.ImpactTimeSteps.GetHashCode() ^
                this.RecoveryTimeSteps.GetHashCode() ^
                this.RunCellsInParallel.GetHashCode() ^
                this.RunSimulationsInParallel.GetHashCode() ^
                this.RunRealm.GetHashCode() ^
                this.DrawRandomly.GetHashCode() ^
                this.ExtinctionThreshold.GetHashCode() ^
                this.MaxNumberOfCohorts.GetHashCode() ^
                this.DispersalOnly.GetHashCode() ^
                this.DispersalOnlyType.GetHashCode() ^
                this.PlanktonDispersalThreshold.GetHashCode() ^
                this.CohortFunctionalGroupDefinitions.GetHashCode() ^
                this.StockFunctionalGroupDefinitions.GetHashCode() ^
                this.ImpactCellIndices.GetHashCode() ^
                this.ImpactAll.GetHashCode() ^
                this.ScenarioParameters.GetHashCode() ^
                this.ScenarioIndex.GetHashCode() ^
                this.Simulation.GetHashCode() ^
                this.EcologicalParameters.GetHashCode();
        }
    }
}
