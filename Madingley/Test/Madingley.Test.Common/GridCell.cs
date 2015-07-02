using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class GridCell
    {
        public static IList<IEnumerable<Madingley.Common.Cohort>> RandomCohorts(Random rnd)
        {
            return new List<IEnumerable<Madingley.Common.Cohort>>
            {
                new Madingley.Common.Cohort[]
                    {
                        Cohort.RandomCohort(rnd)
                    },
                new Madingley.Common.Cohort[]
                    {
                        Cohort.RandomCohort(rnd),
                        Cohort.RandomCohort(rnd)
                    }
            };
        }

        public static IList<IEnumerable<Madingley.Common.Stock>> RandomStocks(Random rnd)
        {
            return new List<IEnumerable<Madingley.Common.Stock>>
            {
                new Madingley.Common.Stock[]
                    {
                        Stock.RandomStock(rnd),
                        Stock.RandomStock(rnd),
                        Stock.RandomStock(rnd)
                    },
                new Madingley.Common.Stock[]
                    {
                        Stock.RandomStock(rnd)
                    },
                new Madingley.Common.Stock[]
                    {
                        Stock.RandomStock(rnd),
                        Stock.RandomStock(rnd)
                    }
            };
        }

        public static SortedList<string, double[]> RandomEnvironment(Random rnd)
        {
            var environmentArray = new Tuple<string, double[]>[]
                {
                    Tuple.Create("ce1", Common.RandomDoubleArray(rnd,3)),
                    Tuple.Create("ce2", Common.RandomDoubleArray(rnd, 7))
                };

            var environmentDictionary = environmentArray.ToDictionary(l => l.Item1, l => l.Item2);

            return new SortedList<string, double[]>(environmentDictionary);
        }

        public static Madingley.Common.GridCell RandomGridCell(Random rnd)
        {
            var latitude = Common.RandomLatitude(rnd);
            var longitude = Common.RandomLongitude(rnd);
            var cohorts = RandomCohorts(rnd);
            var stocks = RandomStocks(rnd);
            var environment = RandomEnvironment(rnd);

            return new Madingley.Common.GridCell(
                latitude,
                longitude,
                cohorts,
                stocks,
                environment);
        }
    }
}
