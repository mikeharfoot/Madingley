using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.CSV;
using Microsoft.Research.Science.Data.Imperative;
using Microsoft.Research.Science.Data.Utilities;


namespace Madingley
{
    /// <summary>
    /// Stores properties of grid cells
    /// <todoD>Remove single valued state-variables and convert model to work with functional groups</todoD>
    /// <todoD>Check the get/set methods and overloads</todoD>
    /// <todoD>Convert GetEnviroLayer to field terminology</todoD>
    /// </summary>
    public class GridCell
    {

        /// <summary>
        /// The handler for the cohorts in this grid cell
        /// </summary>
        private GridCellCohortHandler _GridCellCohorts;
        /// <summary>
        /// Get or set the cohorts in this grid cell
        /// </summary>
        public GridCellCohortHandler GridCellCohorts
        {
            get { return _GridCellCohorts; }
            set { _GridCellCohorts = value; }
        }

        /// <summary>
        /// The handler for the stocks in this grid cell
        /// </summary>
        private GridCellStockHandler _GridCellStocks;
        /// <summary>
        /// Get or set the stocks in this grid cell
        /// </summary>
        public GridCellStockHandler GridCellStocks
        {
            get { return _GridCellStocks; }
            set { _GridCellStocks = value; }
        }

        /// <summary>
        /// The environmental data for this grid cell
        /// </summary>
        private SortedList<string, double[]> _CellEnvironment;
        /// <summary>
        /// Get the environmental data for this grid cell
        /// </summary>
        public SortedList<string, double[]> CellEnvironment { get { return _CellEnvironment; } set { _CellEnvironment = value; } }

        /// <summary>
        /// The latitude of this grid cell
        /// </summary>
        private float _Latitude;
        /// <summary>
        /// Get the latitude of this grid cell
        /// </summary>
        public float Latitude
        {
            get { return _Latitude; }
        }

        /// <summary>
        /// The longitude of this grid cell
        /// </summary>
        private float _Longitude;
        /// <summary>
        /// Get the longitude of this grid cell
        /// </summary>
        public float Longitude
        {
            get { return _Longitude; }
        }

        /// <summary>
        /// Gets the value in this grid cell of a specified environmental variable at a specified time interval
        /// </summary>
        /// <param name="variableName">The name of the environmental layer from which to extract the value</param>
        /// <param name="timeInterval">The index of the time interval to return data for (i.e. 0 if it is a yearly variable
        /// or the month index - 0=Jan, 1=Feb etc. - for monthly variables)</param>
        /// <param name="variableFound">Returns whether the variable was found in the cell environment</param>
        /// <returns>The value in this grid cell of a specified environmental variable at a specified time interval</returns>
        public double GetEnviroLayer(string variableName, uint timeInterval, out bool variableFound)
        {
            // If the specified variable is in the cell environment then return the requested value, otherwise set variable found boolean
            // to false and return a missing value
            if (_CellEnvironment.ContainsKey(variableName))
            {
                variableFound = true;
                return _CellEnvironment[variableName][timeInterval];
            }
            else
            {
                variableFound = false;
                Console.WriteLine("Attempt to get environmental layer value failed: {0} does not exist", variableName);
                return _CellEnvironment["Missing Value"][0];
            }
        }

#if true
        public GridCell(
            GridCellCohortHandler GridCellCohorts,
            GridCellStockHandler GridCellStocks,
            SortedList<string, double[]> CellEnvironment,
            float Latitude,
            float Longitude)
        {
            this.GridCellCohorts = GridCellCohorts;
            this.GridCellStocks = GridCellStocks;
            this.CellEnvironment = CellEnvironment;
            this._Latitude = Latitude;
            this._Longitude = Longitude;
        }
#endif
    }


}

