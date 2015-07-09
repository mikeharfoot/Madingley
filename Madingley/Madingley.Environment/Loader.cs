using System;
using System.Collections.Generic;
using System.Linq;

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
            var mmi = new MadingleyModelInitialisation(simulationInitialisationFile, definitionsFilename, outputsFilename, outputPath, environmentDataRoot, inputPath);

            var m = new MadingleyModel(mmi);

            Func<uint[], Tuple<int, int>> convertCellIndices = e => Tuple.Create((int)e[0], (int)e[1]);

            Func<uint[], Dictionary<string, double[]>> convertCellEnvironment =
                cell =>
                {
                    var e = m.EcosystemModelGrid.GetCellEnvironment(cell[0], cell[1]);

                    return e.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
                };

            return
                new Madingley.Common.Environment(
                    mmi.CellSize,
                    mmi.BottomLatitude,
                    mmi.TopLatitude,
                    mmi.LeftmostLongitude,
                    mmi.RightmostLongitude,
                    mmi.Units,
                    m.SpecificLocations,
                    m._CellList.Select(convertCellIndices),
                    m._CellList.Select(convertCellEnvironment));
        }
    }
}
