using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class ScenarioParameters
    {
        public static Madingley.Common.ScenarioParameter RandomScenarioParameter(Random rnd)
        {
            return new Madingley.Common.ScenarioParameter(Common.RandomString(rnd, 5), rnd.NextDouble(), rnd.NextDouble());
        }

        public static Madingley.Common.ScenarioParameters RandomScenarioParameters(Random rnd, int length)
        {
            var validScenarios = new string[] { "npp", "temperature", "harvesting" };

            var rndTriplesMap = validScenarios.Select(scenario => Tuple.Create(scenario, RandomScenarioParameter(rnd)));

            var rndTriples = rndTriplesMap.ToDictionary(l => l.Item1, l => l.Item2);

            return new Madingley.Common.ScenarioParameters(Common.RandomString(rnd, 7), rnd.Next(0, (int)(System.Int16.MaxValue)), rndTriples);
        }

        public static void SaveScenarios(IEnumerable<Madingley.Common.ScenarioParameters> scenarioParameters, string filename)
        {
            var header = new string[] { "label", "npp", "temperature", "harvesting", "simulation number" };

            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine(System.String.Join(",", header));

                scenarioParameters.ToList().ForEach(si =>
                {
                    var s3 = si.Parameters;

                    var p3 = new Madingley.Common.ScenarioParameter[] { s3["npp"], s3["temperature"], s3["harvesting"] }.Select
                            (s3i => String.Format("{0} {1} {2}", s3i.ParamString, s3i.ParamDouble1.ToString(), s3i.ParamDouble2.ToString()));

                    var q3 = String.Join(",", p3);

                    writer.WriteLine(String.Format("{0},{1},{2}", si.Label, q3, si.SimulationNumber.ToString()));
                });
            }
        }
    }
}
