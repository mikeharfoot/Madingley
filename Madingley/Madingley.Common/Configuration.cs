using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class Configuration
    {
        public string GlobalModelTimeStepUnit { get; set; }

        public int NumTimeSteps { get; set; }

        public int BurninTimeSteps { get; set; }

        public int ImpactTimeSteps { get; set; }

        public int RecoveryTimeSteps { get; set; }

        public bool RunCellsInParallel { get; set; }

        public bool RunSimulationsInParallel { get; set; }

        public string RunRealm { get; set; }

        public bool DrawRandomly { get; set; }

        public double ExtinctionThreshold { get; set; }

        public int MaxNumberOfCohorts { get; set; }

        public bool DispersalOnly { get; set; }

        public string DispersalOnlyType { get; set; }

        public double PlanktonDispersalThreshold { get; set; }

        public FunctionalGroupDefinitions CohortFunctionalGroupDefinitions { get; set; }

        public FunctionalGroupDefinitions StockFunctionalGroupDefinitions { get; set; }

        public IEnumerable<int> ImpactCellIndices { get; set; }

        public bool ImpactAll { get; set; }

        public ScenarioParameters[] ScenarioParameters { get; set; }

        public int ScenarioIndex { get; set; }

        public int Simulation { get; set; }

        public EcologicalParameters EcologicalParameters { get; set; }

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

        public Configuration(Configuration c)
        {
            this.GlobalModelTimeStepUnit = c.GlobalModelTimeStepUnit;
            this.NumTimeSteps = c.NumTimeSteps;
            this.BurninTimeSteps = c.BurninTimeSteps;
            this.ImpactTimeSteps = c.ImpactTimeSteps;
            this.RecoveryTimeSteps = c.RecoveryTimeSteps;
            this.RunCellsInParallel = c.RunCellsInParallel;
            this.RunSimulationsInParallel = c.RunSimulationsInParallel;
            this.RunRealm = c.RunRealm;
            this.DrawRandomly = c.DrawRandomly;
            this.ExtinctionThreshold = c.ExtinctionThreshold;
            this.MaxNumberOfCohorts = c.MaxNumberOfCohorts;
            this.DispersalOnly = c.DispersalOnly;
            this.DispersalOnlyType = c.DispersalOnlyType;
            this.PlanktonDispersalThreshold = c.PlanktonDispersalThreshold;
            this.MaxNumberOfCohorts = c.MaxNumberOfCohorts;
            this.CohortFunctionalGroupDefinitions = new FunctionalGroupDefinitions(c.CohortFunctionalGroupDefinitions);
            this.StockFunctionalGroupDefinitions = new FunctionalGroupDefinitions(c.StockFunctionalGroupDefinitions);
            this.ImpactCellIndices = c.ImpactCellIndices.ToArray();
            this.ImpactAll = c.ImpactAll;
            this.ScenarioParameters = c.ScenarioParameters.Select(s => new ScenarioParameters(s)).ToArray();
            this.ScenarioIndex = c.ScenarioIndex;
            this.Simulation = c.Simulation;
            this.EcologicalParameters = new EcologicalParameters(c.EcologicalParameters);
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as Configuration;
            if ((Object)y == null) return false;

            IEqualityComparer<double> compareFloats = new FixedDoubleComparer();

            return
                this.GlobalModelTimeStepUnit.Equals(y.GlobalModelTimeStepUnit) &&
                this.NumTimeSteps.Equals(y.NumTimeSteps) &&
                this.BurninTimeSteps.Equals(y.BurninTimeSteps) &&
                this.ImpactTimeSteps.Equals(y.ImpactTimeSteps) &&
                this.RecoveryTimeSteps.Equals(y.RecoveryTimeSteps) &&
                this.RunCellsInParallel.Equals(y.RunCellsInParallel) &&
                this.RunSimulationsInParallel.Equals(y.RunSimulationsInParallel) &&
                this.RunRealm.Equals(y.RunRealm) &&
                this.DrawRandomly.Equals(y.DrawRandomly) &&
                compareFloats.Equals(this.ExtinctionThreshold, y.ExtinctionThreshold) &&
                this.MaxNumberOfCohorts.Equals(y.MaxNumberOfCohorts) &&
                this.DispersalOnly.Equals(y.DispersalOnly) &&
                this.DispersalOnlyType.Equals(y.DispersalOnlyType) &&
                compareFloats.Equals(this.PlanktonDispersalThreshold, y.PlanktonDispersalThreshold) &&
                this.CohortFunctionalGroupDefinitions.Equals(y.CohortFunctionalGroupDefinitions) &&
                this.StockFunctionalGroupDefinitions.Equals(y.StockFunctionalGroupDefinitions) &&
                this.ImpactCellIndices.SequenceEqual(y.ImpactCellIndices) &&
                this.ImpactAll.Equals(y.ImpactAll) &&
                this.ScenarioParameters.SequenceEqual(y.ScenarioParameters, new ScenarioParametersComparer(compareFloats)) &&
                this.ScenarioIndex.Equals(y.ScenarioIndex) &&
                this.Simulation.Equals(y.Simulation) &&
                this.EcologicalParameters.Equals(y.EcologicalParameters);
        }

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
