using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class Environment
    {
        public double CellSize { get; set; }

        public double BottomLatitude { get; set; }

        public double TopLatitude { get; set; }

        public double LeftmostLongitude { get; set; }

        public double RightmostLongitude { get; set; }

        public IDictionary<string, string> Units { get; set; }

        public bool SpecificLocations { get; set; }

        public IList<Tuple<int, int>> FocusCells { get; set; }

        public IList<IDictionary<string, double[]>> CellEnvironment { get; set; }

        public Environment(
            double cellSize,
            double bottomLatitude,
            double topLatitude,
            double leftmostLongitude,
            double rightmostLongitude,
            IDictionary<string, string> units,
            bool specificLocations,
            IEnumerable<Tuple<int, int>> focusCells,
            IEnumerable<IDictionary<string, double[]>> cellEnvironment)
        {
            this.CellSize = cellSize;
            this.BottomLatitude = bottomLatitude;
            this.TopLatitude = topLatitude;
            this.LeftmostLongitude = leftmostLongitude;
            this.RightmostLongitude = rightmostLongitude;
            this.Units = new SortedList<string,string>(units);
            this.SpecificLocations = specificLocations;
            this.FocusCells = focusCells.Select(fc => Tuple.Create(fc.Item1, fc.Item2)).ToArray();
            this.CellEnvironment = cellEnvironment.Select(ce => new SortedList<string, double[]>(ce.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()))).ToArray();
        }

        public Environment(Environment c)
        {
            this.CellSize = c.CellSize;
            this.BottomLatitude = c.BottomLatitude;
            this.TopLatitude = c.TopLatitude;
            this.LeftmostLongitude = c.LeftmostLongitude;
            this.RightmostLongitude = c.RightmostLongitude;
            this.Units = new SortedList<string, string>(c.Units);
            this.SpecificLocations = c.SpecificLocations;
            this.FocusCells = c.FocusCells.Select(fc => Tuple.Create(fc.Item1, fc.Item2)).ToArray();
            this.CellEnvironment = c.CellEnvironment.Select(ce => new SortedList<string, double[]>(ce.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()))).ToArray();
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as Environment;
            if ((Object)y == null) return false;

            var comparer = new StringMapEqualityComparer<double[]>(new KeyValuePairEqualityComparer<double[]>(new ArrayEqualityComparer<double>(EqualityComparer<double>.Default)));

            return
                this.CellSize.Equals(y.CellSize) &&
                this.BottomLatitude.Equals(y.BottomLatitude) &&
                this.TopLatitude.Equals(y.TopLatitude) &&
                this.LeftmostLongitude.Equals(y.LeftmostLongitude) &&
                this.RightmostLongitude.Equals(y.RightmostLongitude) &&
                this.Units.SequenceEqual(y.Units) &&
                this.SpecificLocations.Equals(y.SpecificLocations) &&
                this.FocusCells.SequenceEqual(y.FocusCells) &&
                this.CellEnvironment.SequenceEqual(y.CellEnvironment, comparer);
        }

        public override int GetHashCode()
        {
            return
                this.CellSize.GetHashCode() ^
                this.BottomLatitude.GetHashCode() ^
                this.TopLatitude.GetHashCode() ^
                this.LeftmostLongitude.GetHashCode() ^
                this.RightmostLongitude.GetHashCode() ^
                this.Units.GetHashCode() ^
                this.SpecificLocations.GetHashCode() ^
                this.FocusCells.GetHashCode() ^
                this.CellEnvironment.GetHashCode();
        }
    }
}
