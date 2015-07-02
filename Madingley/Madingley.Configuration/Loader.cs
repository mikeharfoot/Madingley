using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Configuration
{
    /// <summary>
    /// Initialization information for Madingley model simulations
    /// </summary>
    public static class Loader
    {
        public static string simulationInitialisationFile = "SimulationControlParameters.csv";
        public static string definitionsFilename = "FileLocationParameters.csv";
        public static string outputsFilename = "";
        public static string outputPath = "";

        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="inputPath">The path to folder which contains the inputs</param>
        public static Madingley.Common.Configuration Load(string inputPath)
        {
            var i = new MadingleyModelInitialisation(simulationInitialisationFile, definitionsFilename, outputsFilename, outputPath, inputPath);

            var s = new ScenarioParameterInitialisation("Scenarios.csv", "", inputPath);

            return
                new Madingley.Common.Configuration(
                    i.GlobalModelTimeStepUnit,
                    (int)i.NumTimeSteps,
                    (int)i.BurninTimeSteps,
                    (int)i.ImpactTimeSteps,
                    (int)i.RecoveryTimeSteps,
                    i.RunCellsInParallel,
                    i.RunSimulationsInParallel,
                    i.RunRealm,
                    i.DrawRandomly,
                    i.ExtinctionThreshold,
                    i.MaxNumberOfCohorts,
                    i.DispersalOnly,
                    i.InitialisationFileStrings["DispersalOnlyType"],
                    i.PlanktonDispersalThreshold,
                    ConvertFunctionalGroupDefinitions(i.CohortFunctionalGroupDefinitions),
                    ConvertFunctionalGroupDefinitions(i.StockFunctionalGroupDefinitions),
                    i.ImpactCellIndices.Select(ii => (int)ii),
                    i.ImpactAll,
                    s.scenarioParameters.Select(a => new Madingley.Common.ScenarioParameters(a.Item1, a.Item2, a.Item3)),
                    0,
                    0,
                    new Madingley.Common.EcologicalParameters(EcologicalParameters.Parameters, EcologicalParameters.TimeUnits));
        }

        public static Madingley.Common.FunctionalGroupDefinitions ConvertFunctionalGroupDefinitions(FunctionalGroupDefinitions g)
        {
            var length = g.TraitLookupFromIndex.Max(p => p.Value.Length);

            var data = new Madingley.Common.FunctionalGroupDefinition[length];

            for (var i = 0; i < length; i++)
            {
                var definitions = g.TraitLookupFromIndex.ToDictionary(kv => kv.Key, kv => kv.Value[i]);
                var properties = g.FunctionalGroupProperties.ToDictionary(kv => kv.Key, kv => kv.Value[i]);

                data[i] = new Madingley.Common.FunctionalGroupDefinition(definitions, properties);
            }

            var definitionNames = g.TraitLookupFromIndex.Select(kv => kv.Key).Distinct();

            var propertyNames = g.FunctionalGroupProperties.Select(kv => kv.Key).Distinct();

            return new Madingley.Common.FunctionalGroupDefinitions(data, definitionNames, propertyNames);
        }
    }
}
