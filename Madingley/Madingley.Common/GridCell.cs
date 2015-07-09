using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Full properties of grid cells for model state and output.
    /// </summary>
    public class GridCell
    {
        /// <summary>
        /// Latitude of this grid cell.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of this grid cell.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// List of cohorts in this grid cell.
        /// </summary>
        public IList<IEnumerable<Cohort>> Cohorts { get; set; }

        /// <summary>
        /// List of stocks in this grid cell.
        /// </summary>
        public IList<IEnumerable<Stock>> Stocks { get; set; }

        /// <summary>
        /// Environmental data for this grid cell.
        /// </summary>
        public IDictionary<string, double[]> Environment { get; set; }

        /// <summary>
        /// GridCell constructor.
        /// </summary>
        /// <param name="latitude">Latitude of this grid cell.</param>
        /// <param name="longitude">Longitude of this grid cell.</param>
        /// <param name="cohorts">List of cohorts in this grid cell.</param>
        /// <param name="stocks">List of stocks in this grid cell.</param>
        /// <param name="environment">Environmental data for this grid cell.</param>
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

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="gridCell">GridCell to copy.</param>
        public GridCell(GridCell gridCell)
        {
            this.Latitude = gridCell.Latitude;
            this.Longitude = gridCell.Longitude;
            this.Cohorts = gridCell.Cohorts.Select(cs => cs.ToArray()).ToArray();
            this.Stocks = gridCell.Stocks.Select(ss => ss.ToArray()).ToArray();
            this.Environment = new SortedList<string, double[]>(gridCell.Environment.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()));
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both GridCells and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var gridCellObj = obj as GridCell;
            if ((Object)gridCellObj == null) return false;

            return
                this.Latitude.Equals(gridCellObj.Latitude) &&
                this.Longitude.Equals(gridCellObj.Longitude) &&
                this.Cohorts.SequenceEqual(gridCellObj.Cohorts, new ArrayEqualityComparer<Cohort>(EqualityComparer<Cohort>.Default)) &&
                this.Stocks.SequenceEqual(gridCellObj.Stocks, new ArrayEqualityComparer<Stock>(EqualityComparer<Stock>.Default)) &&
                this.Environment.SequenceEqual(gridCellObj.Environment, new KeyValuePairEqualityComparer<double[]>(new ArrayEqualityComparer<double>(EqualityComparer<double>.Default)));
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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
