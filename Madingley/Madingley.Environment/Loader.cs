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

            var unitsDictionary = mmi.Units.ToDictionary(kv => kv.Key, kv => kv.Value);

            var focusCells =
                m._CellList.Select(
                    (uint[] e) =>
                        Tuple.Create((int)e[0], (int)e[1]));

            var cellEnvironment =
                focusCells.Select(
                    cell =>
                    {
                        var e = m.EcosystemModelGrid.GetCellEnvironment((uint)cell.Item1, (uint)cell.Item2);

                        return e.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
                    });

            var environment =
                new Madingley.Common.Environment(
                    mmi.CellSize,
                    mmi.BottomLatitude,
                    mmi.TopLatitude,
                    mmi.LeftmostLongitude,
                    mmi.RightmostLongitude,
                    unitsDictionary,
                    m.SpecificLocations,
                    focusCells,
                    cellEnvironment);

            return environment;
        }
    }
}
