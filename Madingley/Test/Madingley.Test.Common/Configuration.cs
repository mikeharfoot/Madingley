using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class Configuration
    {
        public static Madingley.Common.ScenarioParameters[] RandomScenarioParameters(Random rnd, int length1, int length2)
        {
            return Enumerable.Range(0, length2).Select(i => ScenarioParameters.RandomScenarioParameters(rnd, length1)).ToArray();
        }

        public static Madingley.Common.Configuration RandomConfiguration(Random rnd)
        {
            var convertTimeSteps = 12;
            var yearCount = rnd.Next(0, (int)System.UInt16.MaxValue);

            var globalModelTimeStepUnit = "Month";
            var numTimeSteps = yearCount * convertTimeSteps;
            var burninTimeSteps = rnd.Next(0, yearCount) * convertTimeSteps;
            var impactTimeSteps = rnd.Next(0, yearCount) * convertTimeSteps;
            var recoveryTimeSteps = rnd.Next(0, yearCount) * convertTimeSteps;
            var runCellsInParallel = Common.RandomBool(rnd);
            var runSimulationsInParallel = Common.RandomBool(rnd);
            var runRealm = Common.RandomString(rnd, 6).ToLower();
            var drawRandomly = Common.RandomBool(rnd);
            var extinctionThreshold = Common.RandomFloat(rnd);
            var maxNumberOfCohorts = rnd.Next();
            var dispersalOnly = Common.RandomBool(rnd);
            var dispersalOnlyType = Common.RandomString(rnd, 9);
            var planktonDispersalThreshold = Common.RandomFloat(rnd);
            var cohortFunctionalGroupDefinitions = FunctionalGroupDefinitions.RandomFunctionalGroupDefinitions(rnd);
            var stockFunctionalGroupDefinitions = FunctionalGroupDefinitions.RandomFunctionalGroupDefinitions(rnd);
            var impactCellIndices = Common.RandomIntArray(rnd, 12);
            var impactAll = false; // TODO rndBool()
            var scenarioParameters = RandomScenarioParameters(rnd, 13, 14);
            var scenarioIndex = 0;
            var simulation = 0;
            var ecologicalParameters = EcologicalParameters.RandomEcologicalParameters(rnd);
            var fileNames = new string[] {
                @"Model setup\SimulationControlParameters.csv",
                @"Model setup\FileLocationParameters.csv",
                @"Model setup\Ecological Definition Files\CohortFunctionalGroupDefinitions.csv",
                @"Model setup\Ecological Definition Files\StockFunctionalGroupDefinitions.csv",
                @"Model setup\Ecological Definition Files\EcologicalParameters.csv",
                @"Model setup\Initial Model State Setup\Scenarios.csv"
            };

            return new Madingley.Common.Configuration(
                globalModelTimeStepUnit,
                numTimeSteps,
                burninTimeSteps,
                impactTimeSteps,
                recoveryTimeSteps,
                runCellsInParallel,
                runSimulationsInParallel,
                runRealm,
                drawRandomly,
                extinctionThreshold,
                maxNumberOfCohorts,
                dispersalOnly,
                dispersalOnlyType,
                planktonDispersalThreshold,
                cohortFunctionalGroupDefinitions,
                stockFunctionalGroupDefinitions,
                impactCellIndices,
                impactAll,
                scenarioParameters,
                scenarioIndex,
                simulation,
                ecologicalParameters,
                fileNames);
        }

        public static void CreateDirectories(string directory)
        {
            Common.DeleteDirectory(directory);

            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(Path.Combine(directory, "Ecological Definition Files"));
            Directory.CreateDirectory(Path.Combine(directory, "Environmental Data Layer List"));
            Directory.CreateDirectory(Path.Combine(directory, "Initial Model State Setup"));
        }

        public static void Save(string directory, Madingley.Common.Configuration c)
        {
            CreateDirectories(directory);

            var convertTimeSteps = 12;

            var parameters = new Tuple<string, string>[]
                {
                    Tuple.Create("Parameter", "Value"),
                    Tuple.Create("Timestep Units", c.GlobalModelTimeStepUnit),
                    Tuple.Create("Length of simulation (years)", (c.NumTimeSteps / convertTimeSteps).ToString()),
                    Tuple.Create("Burn-in (years)", (c.BurninTimeSteps / convertTimeSteps).ToString()),
                    Tuple.Create("Impact duration (years)", (c.ImpactTimeSteps / convertTimeSteps).ToString()),
                    Tuple.Create("Recovery duration (years)", (c.RecoveryTimeSteps / convertTimeSteps).ToString()),
                    Tuple.Create("Plankton size threshold", c.PlanktonDispersalThreshold.ToString()),
                    Tuple.Create("Draw Randomly", c.DrawRandomly ? "yes" : "no"),
                    Tuple.Create("Extinction Threshold", c.ExtinctionThreshold.ToString()),
                    Tuple.Create("Maximum Number Of Cohorts", c.MaxNumberOfCohorts.ToString()),
                    Tuple.Create("Run Cells In Parallel", c.RunCellsInParallel ? "yes" : "no"),
                    Tuple.Create("Run Simulations In Parallel", c.RunSimulationsInParallel ? "yes" : "no"),
                    Tuple.Create("Run Single Realm", c.RunRealm),
                    Tuple.Create("Impact Cell Index", System.String.Join(";", c.ImpactCellIndices)),
                    // Tuple.Create("ImpactAll", if c.ImpactAll then "yes" else "no"),
                    Tuple.Create("Dispersal only", c.DispersalOnly ? "yes" : "no"),
                    Tuple.Create("Dispersal only type", c.DispersalOnlyType)
                };

            using (var writer = new StreamWriter(Path.Combine(directory, "SimulationControlParameters.csv")))
            {
                parameters.ToList().ForEach(kv => writer.WriteLine(String.Format("{0},{1}", kv.Item1, kv.Item2)));
            }

            var fileLocationParameters = new Tuple<string, string>[]
                {
                    Tuple.Create("Parameter", "Value"),
                    Tuple.Create("Mass Bin Filename", "MassBinDefinitions.csv"),
                    Tuple.Create("Environmental Data File", "EnvironmentalDataLayers.csv"),
                    Tuple.Create("Cohort Functional Group Definitions File", "CohortFunctionalGroupDefinitions.csv"),
                    Tuple.Create("Stock Functional Group Definitions File", "StockFunctionalGroupDefinitions.csv"),
                    Tuple.Create("Ecological parameters file","EcologicalParameters.csv")
                };

            using (var writer = new StreamWriter(Path.Combine(directory, "FileLocationParameters.csv")))
            {
                fileLocationParameters.ToList().ForEach(kv => writer.WriteLine(String.Format("{0},{1}", kv.Item1, kv.Item2)));
            }

            EcologicalParameters.Save(c.EcologicalParameters, Path.Combine(directory, "Ecological Definition Files", "EcologicalParameters.csv"));

            FunctionalGroupDefinitions.Save(c.CohortFunctionalGroupDefinitions, Path.Combine(directory, "Ecological Definition Files", "CohortFunctionalGroupDefinitions.csv"));
            FunctionalGroupDefinitions.Save(c.StockFunctionalGroupDefinitions, Path.Combine(directory, "Ecological Definition Files", "StockFunctionalGroupDefinitions.csv"));

            ScenarioParameters.SaveScenarios(c.ScenarioParameters, Path.Combine(directory, "Initial Model State Setup", "Scenarios.csv"));
        }
    }
}
