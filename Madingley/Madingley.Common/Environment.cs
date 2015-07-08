using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// The Environment for a simulation - its geographic extent and environmental data.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// Size of cells to be used in the model grid.
        /// </summary>
        public double CellSize { get; set; }

        /// <summary>
        /// Lowest extent of the model grid in degrees.
        /// </summary>
        public double BottomLatitude { get; set; }

        /// <summary>
        /// Uppermost extent of the model grid in degrees.
        /// </summary>
        public double TopLatitude { get; set; }

        /// <summary>
        /// Leftmost extent of the model grid in degrees.
        /// </summary>
        public double LeftmostLongitude { get; set; }

        /// <summary>
        /// Rightmost extent of the model grid in degrees.
        /// </summary>
        public double RightmostLongitude { get; set; }

        /// <summary>
        /// String values for the units of each environmental data layer.
        /// </summary>
        public IDictionary<string, string> Units { get; set; }

        /// <summary>
        /// Have specific locations have been specified?
        /// </summary>
        public bool SpecificLocations { get; set; }

        /// <summary>
        /// Pairs of longitude and latitude indices for all active cells in the model grid.
        /// </summary>
        public IList<Tuple<int, int>> FocusCells { get; set; }

        /// <summary>
        /// For each active cell, a set of environmental data. For each environment parameter will
        /// be either an array with a single value, or an array with a value for each month
        /// </summary>
        public IList<IDictionary<string, double[]>> CellEnvironment { get; set; }

        /// <summary>
        /// Environment constructor.
        /// </summary>
        /// <param name="cellSize">Size of cells to be used in the model grid.</param>
        /// <param name="bottomLatitude">Lowest extent of the model grid in degrees.</param>
        /// <param name="topLatitude">Uppermost extent of the model grid in degrees.</param>
        /// <param name="leftmostLongitude">Leftmost extent of the model grid in degrees.</param>
        /// <param name="rightmostLongitude">Rightmost extent of the model grid in degrees.</param>
        /// <param name="units">String values for the units of each environmental data layer.</param>
        /// <param name="specificLocations">Have specific locations have been specified?</param>
        /// <param name="focusCells">Pairs of longitude and latitude indices for all active cells in the model grid.</param>
        /// <param name="cellEnvironment">For each active cell, a set of environmental data.</param>
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

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="c"></param>
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

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both Environments and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var y = obj as Environment;
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

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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
