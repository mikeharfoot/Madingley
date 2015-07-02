using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class GridCell
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IList<IEnumerable<Cohort>> Cohorts { get; set; }

        public IList<IEnumerable<Stock>> Stocks { get; set; }

        public IDictionary<string, double[]> Environment { get; set; }

        public GridCell(
            double latitude,
            double longitude,
            IEnumerable<IEnumerable<Cohort>> cohorts,
            IEnumerable<IEnumerable<Stock>> stocks,
            IDictionary<string, double[]> environment)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Cohorts = cohorts.Select(cs => cs.ToArray()).ToArray();
            this.Stocks = stocks.Select(ss => ss.ToArray()).ToArray();
            this.Environment = new SortedList<string, double[]>(environment.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()));
        }

        public GridCell(GridCell c)
        {
            this.Latitude = c.Latitude;
            this.Longitude = c.Longitude;
            this.Cohorts = c.Cohorts.Select(cs => cs.ToArray()).ToArray();
            this.Stocks = c.Stocks.Select(ss => ss.ToArray()).ToArray();
            this.Environment = new SortedList<string, double[]>(c.Environment.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()));
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as GridCell;
            if ((Object)y == null) return false;

            return
                this.Latitude.Equals(y.Latitude) &&
                this.Longitude.Equals(y.Longitude) &&
                this.Cohorts.SequenceEqual(y.Cohorts, new ArrayEqualityComparer<Cohort>(EqualityComparer<Cohort>.Default)) &&
                this.Stocks.SequenceEqual(y.Stocks, new ArrayEqualityComparer<Stock>(EqualityComparer<Stock>.Default)) &&
                this.Environment.SequenceEqual(y.Environment, new KeyValuePairEqualityComparer<double[]>(new ArrayEqualityComparer<double>(EqualityComparer<double>.Default)));
        }

        public override int GetHashCode()
        {
            return
                this.Latitude.GetHashCode() ^
                this.Longitude.GetHashCode() ^
                this.Cohorts.GetHashCode() ^
                this.Stocks.GetHashCode() ^
                this.Environment.GetHashCode();
        }
    }
}
