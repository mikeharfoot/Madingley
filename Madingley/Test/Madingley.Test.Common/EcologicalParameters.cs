using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class EcologicalParameters
    {
        public static Madingley.Common.EcologicalParameters RandomEcologicalParameters(Random rnd)
        {
            var parameterNames = Common.RandomStringArray(rnd, 3, 7);

            var rndPairsMap = parameterNames.Select(definition => Tuple.Create(definition.ToLower(), rnd.NextDouble()));

            var parameters = rndPairsMap.ToDictionary(l => l.Item1, l => l.Item2);

            var timeSteps = new string[] { "day", "month", "year" };

            return new Madingley.Common.EcologicalParameters(parameters, timeSteps);
        }

        public static void Save(Madingley.Common.EcologicalParameters ecologicalParameters, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("Parameter,Value");
                ecologicalParameters.Parameters.ToList().ForEach(kv => writer.WriteLine(String.Format("{0},{1}", kv.Key, kv.Value)));
            }
        }
    }
}
