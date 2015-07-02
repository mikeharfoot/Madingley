using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class FunctionalGroupDefinitions
    {
        public static SortedList<string, string> RandomDefinitions(Random rnd, string[] definitionNames)
        {
            var rndPairsMap = definitionNames.Select(definition => Tuple.Create(definition.ToLower(), Common.RandomString(rnd, 7).ToLower()));

            var rndPairsDictionary = rndPairsMap.ToDictionary(l => l.Item1, l => l.Item2);

            return new SortedList<string, string>(rndPairsDictionary);
        }

        public static SortedList<string, double> RandomProperties(Random rnd, string[] propertyNames)
        {
            var rndPairsMap = propertyNames.Select(definition => Tuple.Create(definition.ToLower(), rnd.NextDouble()));

            var rndPairsDictionary = rndPairsMap.ToDictionary(l => l.Item1, l => l.Item2);

            return new SortedList<string, double>(rndPairsDictionary);
        }

        public static Madingley.Common.FunctionalGroupDefinition RandomFunctionalGroupDefinition(Random rnd, string[] definitionNames, string[] propertyNames)
        {
            return new Madingley.Common.FunctionalGroupDefinition(RandomDefinitions(rnd, definitionNames), RandomProperties(rnd, propertyNames));
        }

        public static Madingley.Common.FunctionalGroupDefinitions RandomFunctionalGroupDefinitions(Random rnd)
        {
            var definitionNames = Common.RandomStringArray(rnd, 3, 7);
            var propertyNames = Common.RandomStringArray(rnd, 4, 8);

            var data = Enumerable.Range(0, 6).Select(i => RandomFunctionalGroupDefinition(rnd, definitionNames, propertyNames)).ToArray();

            return new Madingley.Common.FunctionalGroupDefinitions(data, definitionNames, propertyNames);
        }

        public static void Save(Madingley.Common.FunctionalGroupDefinitions c, string filename)
        {
            var definitionHeaders = c.Definitions.Select(k => String.Format("DEFINITION_{0}", k));

            var propertyHeaders = c.Properties.Select(k => String.Format("PROPERTY_{0}", k));

            var cohortRows =
                c.Data.Select(data =>
                {
                    var definitionsElements =
                        c.Definitions.Select(definition =>
                                data.Definitions.ContainsKey(definition) ? data.Definitions[definition] : "");

                    var propertyElements =
                        c.Properties.Select(property =>
                                data.Properties.ContainsKey(property) ? data.Properties[property].ToString() : "");

                    var allElements = definitionsElements.Concat(propertyElements);

                    return System.String.Join(",", allElements);
                });

            var allHeaders = definitionHeaders.Concat(propertyHeaders);
            var header = System.String.Join(",", allHeaders);
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine(header);

                cohortRows.ToList().ForEach(line => writer.WriteLine(line));
            }
        }
    }
}
