
namespace Madingley.Configuration
{
    /// <summary>
    /// Initialization information for Madingley model simulations
    /// </summary>
    public static class Loader
    {
        public static string simulationInitialisationFile = "SimulationControlParameters.csv";
        public static string definitionsFilename = "FileLocationParameters.csv";
        public static string scenariosFilename = "Scenarios.csv";

        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="inputPath">The path to folder which contains the inputs</param>
        public static Madingley.Common.Configuration Load(string inputPath)
        {
            var configuration = MadingleyModelInitialisation.Load(simulationInitialisationFile, definitionsFilename, inputPath);

            configuration.ScenarioParameters = ScenarioParameterInitialisation.Load(scenariosFilename, inputPath);

            return configuration;
        }
    }
}
