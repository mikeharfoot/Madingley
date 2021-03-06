﻿
namespace Madingley.Environment
{
    public static class Loader
    {
        public static string simulationInitialisationFile = "SimulationControlParameters.csv";
        public static string definitionsFilename = "FileLocationParameters.csv";
        public static string outputsFilename = "";
        public static string outputPath = "";

        public static Madingley.Common.Environment Load(
            string environmentDataRoot,
            string inputPath)
        {
            var mmi = MadingleyModelInitialisation.Load(simulationInitialisationFile, definitionsFilename, environmentDataRoot, inputPath);

            MadingleyModel.Load(mmi);

            return mmi.Item1;
        }
    }
}
