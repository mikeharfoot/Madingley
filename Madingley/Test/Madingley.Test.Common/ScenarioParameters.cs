using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class ScenarioParameters
    {
        public static Madingley.Common.ScenarioParameters RandomScenarioParameter(Random rnd, int length)
        {
            var validScenarios = new string[] { "npp", "temperature", "harvesting" };

            var rndTriplesMap = validScenarios.Select(scenario => Tuple.Create(scenario, Tuple.Create(Common.RandomString(rnd, 5), rnd.NextDouble(), rnd.NextDouble())));

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

                    var p3 = new Tuple<string, double, double>[] { s3["npp"], s3["temperature"], s3["harvesting"] }.Select
                            (s3i =>
                            {
                                var s31 = s3i.Item1;
                                var s32 = s3i.Item2;
                                var s33 = s3i.Item3;

                                return String.Format("{0} {1} {2}", s31, s32.ToString(), s33.ToString());
                            });

                    var q3 = String.Join(",", p3);

                    writer.WriteLine(String.Format("{0},{1},{2}", si.Label, q3, si.SimulationNumber.ToString()));
                });
            }
        }
    }
}
