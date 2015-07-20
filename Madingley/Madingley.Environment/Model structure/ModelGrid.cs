using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.CSV;
using Microsoft.Research.Science.Data.Imperative;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;

namespace Madingley
{
    /// <summary>
    /// A class containing the model grid (composed of individual grid cells) along with grid attributes.
    /// The model grid is referenced by [Lat index, Lon index]\
    /// <todoD>Check Set and Get state variable methods</todoD>
    /// </summary>
    public class ModelGrid
    {
        // Model grid standardised missing value (applied to all grid cells)
        private double _GlobalMissingValue = -9999;

        // Field to hold minimum latitude of the grid
        private float _MinLatitude;

        // Field to hold minumum longitude of the grid
        private float _MinLongitude;

        // Field to hold maximum latitude of the grid
        private float _MaxLatitude;
        /// <summary>
        /// Get the lowest latitude of the highest cell in the grid
        /// </summary>
        public float MaxLatitude
        {
            get { return _MaxLatitude; }
        }

        // Field to hold maximum longitude of the grid
        private float _MaxLongitude;
        /// <summary>
        /// Get the leftmost longitude of the rightmost cell in the grid
        /// </summary>
        public float MaxLongitude
        {
            get { return _MaxLongitude; }
        }

        // Field to hold latitude resolution of each grid cell
        private float _LatCellSize;

        // Field to hold longitude resolution of each grid cell
        private float _LonCellSize;

        /// <summary>
        /// The rarefaction of grid cells to be applied to active cells in the model grid
        /// </summary>
        private int _GridCellRarefaction;
        /// <summary>
        /// Get the rarefaction of grid cells to be applied to active cells in the model grid
        /// </summary>
        public int GridCellRarefaction
        { get { return _GridCellRarefaction; } }

        /// <summary>
        /// The number of latitudinal cells in the model grid
        /// </summary>
        private UInt32 _NumLatCells;
        /// <summary>
        /// Get the number of latitudinal cells in the model grid
        /// </summary>
        public UInt32 NumLatCells
        {
            get { return _NumLatCells; }
        }

        /// <summary>
        /// The number of longitudinal cells in the model grid
        /// </summary>
        private UInt32 _NumLonCells;

        /// <summary>
        /// The bottom (southern-most) latitude of each row of grid cells
        /// </summary>
        private float[] _Lats;

        /// <summary>
        /// The left (western-most) longitude of each column of grid cells
        /// </summary>
        private float[] _Lons;

        /// <summary>
        /// Array of grid cells
        /// </summary>
        GridCell[,] InternalGrid;

#if true
        /// <summary>
        /// Overloaded constructor for model grid to construct the grid for specific locations
        /// </summary>
        /// <param name="minLat">Minimum grid latitude (degrees)</param>
        /// <param name="minLon">Minimum grid longitude (degrees, currently -180 to 180)</param>
        /// <param name="maxLat">Maximum grid latitude (degrees)</param>
        /// <param name="maxLon">Maximum grid longitude (degrees, currently -180 to 180)</param>
        /// <param name="latCellSize">Latitudinal size of grid cells</param>
        /// <param name="lonCellSize">Longitudinal size of grid cells</param>
        /// <param name="cellList">List of indices of active cells in the model grid</param>
        /// <param name="enviroStack">List of environmental data layers</param>
        /// <param name="specificLocations">Whether the model is to be run for specific locations</param>
        public ModelGrid(float minLat, float minLon, float maxLat, float maxLon, float latCellSize, float lonCellSize, List<uint[]> cellList,
            SortedList<string, EnviroData> enviroStack,
            Boolean specificLocations)
        {
            var runInParallel = false;
#else
        /// <summary>
        /// Overloaded constructor for model grid to construct the grid for specific locations
        /// </summary>
        /// <param name="minLat">Minimum grid latitude (degrees)</param>
        /// <param name="minLon">Minimum grid longitude (degrees, currently -180 to 180)</param>
        /// <param name="maxLat">Maximum grid latitude (degrees)</param>
        /// <param name="maxLon">Maximum grid longitude (degrees, currently -180 to 180)</param>
        /// <param name="latCellSize">Latitudinal size of grid cells</param>
        /// <param name="lonCellSize">Longitudinal size of grid cells</param>
        /// <param name="cellList">List of indices of active cells in the model grid</param>
        /// <param name="enviroStack">List of environmental data layers</param>
        /// <param name="cohortFunctionalGroups">The functional group definitions for cohorts in the model</param>
        /// <param name="stockFunctionalGroups">The functional group definitions for stocks in the model</param>
        /// <param name="globalDiagnostics">Global diagnostic variables</param>
        /// <param name="tracking">Whether process tracking is enabled</param>
        /// <param name="specificLocations">Whether the model is to be run for specific locations</param>
        /// <param name="runInParallel">Whether model grid cells will be run in parallel</param>
        public ModelGrid(float minLat, float minLon, float maxLat, float maxLon, float latCellSize, float lonCellSize, List<uint[]> cellList, 
            SortedList<string, EnviroData> enviroStack, FunctionalGroupDefinitions cohortFunctionalGroups,
            FunctionalGroupDefinitions stockFunctionalGroups, SortedList<string, double> globalDiagnostics, Boolean tracking, 
            Boolean specificLocations, Boolean runInParallel)
        { 
#endif
            // CURRENTLY DEFINING MODEL CELLS BY BOTTOM LEFT CORNER
            _MinLatitude = minLat;
            _MinLongitude = minLon;
            _MaxLatitude = maxLat;
            _MaxLongitude = maxLon;
            _LatCellSize = latCellSize;
            _LonCellSize = lonCellSize;
            _GridCellRarefaction = 1;

            // Check to see if the number of grid cells is an integer
            Debug.Assert((((_MaxLatitude - _MinLatitude) % _LatCellSize) == 0), "Error: number of grid cells is non-integer: check cell size");

            _NumLatCells = (UInt32)((_MaxLatitude - _MinLatitude) / _LatCellSize);
            _NumLonCells = (UInt32)((_MaxLongitude - _MinLongitude) / _LonCellSize);
            _Lats = new float[_NumLatCells];
            _Lons = new float[_NumLonCells];

            // Set up latitude and longitude vectors - lower left
            for (int ii = 0; ii < _NumLatCells; ii++)
            {
                _Lats[ii] = _MinLatitude + ii * _LatCellSize;
            }
            for (int jj = 0; jj < _NumLonCells; jj++)
            {
                _Lons[jj] = _MinLongitude + jj * _LonCellSize;
            }

            // Set up a grid of grid cells
            InternalGrid = new GridCell[_NumLatCells, _NumLonCells];

            Console.WriteLine("Initialising grid cell environment:");

            int Count = 0;

            int NCells = cellList.Count;

            if (!runInParallel)
            {
                // Loop over cells to set up the model grid
                for (int ii = 0; ii < cellList.Count; ii++)
                {
                    // Create the grid cell at the specified position
#if true
                    InternalGrid[cellList[ii][0], cellList[ii][1]] = new GridCell(_Lats[cellList[ii][0]], cellList[ii][0],
                        _Lons[cellList[ii][1]], cellList[ii][1], latCellSize, lonCellSize, enviroStack, _GlobalMissingValue,
                        specificLocations);
#else
                    InternalGrid[cellList[ii][0], cellList[ii][1]] = new GridCell(_Lats[cellList[ii][0]], cellList[ii][0],
                        _Lons[cellList[ii][1]], cellList[ii][1], latCellSize, lonCellSize, enviroStack, _GlobalMissingValue,
                        cohortFunctionalGroups, stockFunctionalGroups, globalDiagnostics, tracking, specificLocations);
#endif
                    if (!specificLocations)
                    {
                        Count++;
                        Console.Write("\rInitialised {0} of {1}", Count, NCells);
                    }
                    else
                    {
                        Console.Write("\rRow {0} of {1}", ii + 1, NumLatCells / GridCellRarefaction);
                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                }
            }
            else
            {

                // Run a parallel loop over rows

                Parallel.For(0, NCells, ii =>
                {
                    // Create the grid cell at the specified position
#if true
                    InternalGrid[cellList[ii][0], cellList[ii][1]] = new GridCell(_Lats[cellList[ii][0]], cellList[ii][0],
                        _Lons[cellList[ii][1]], cellList[ii][1], latCellSize, lonCellSize, enviroStack, _GlobalMissingValue,
                        specificLocations);
#else
                    InternalGrid[cellList[ii][0], cellList[ii][1]] = new GridCell(_Lats[cellList[ii][0]], cellList[ii][0],
                        _Lons[cellList[ii][1]], cellList[ii][1], latCellSize, lonCellSize, enviroStack, _GlobalMissingValue,
                        cohortFunctionalGroups, stockFunctionalGroups, globalDiagnostics, tracking, specificLocations);
#endif

                    Count++;
                    Console.Write("\rInitialised {0} of {1}", Count, NCells);
                }
                 );

            }

            if (!specificLocations)
            {
                InterpolateMissingValues();
            }

            Console.WriteLine("\n");
        }

        /// <summary>
        /// Estimates missing environmental data for grid cells by interpolation
        /// </summary>
        public void InterpolateMissingValues()
        {
            SortedList<string, double[]> WorkingCellEnvironment = new SortedList<string, double[]>();
            Boolean Changed = false;

            for (uint ii = 0; ii < _NumLatCells; ii++)
            {
                for (uint jj = 0; jj < _NumLonCells; jj++)
                {
                    WorkingCellEnvironment = GetCellEnvironment(ii, jj);

                    // If the cell environment does not contain valid NPP data then interpolate values
                    if (!InternalGrid[ii, jj].ContainsData(WorkingCellEnvironment["NPP"], WorkingCellEnvironment["Missing Value"][0]))
                    {
                        //If NPP doesn't exist the interpolate from surrounding values (of the same realm)
                        WorkingCellEnvironment["NPP"] = GetInterpolatedValues(ii, jj, GetCellLatitude(ii), GetCellLongitude(jj), "NPP", WorkingCellEnvironment["Realm"][0]);

                        //Calculate NPP seasonality - for use in converting annual NPP estimates to monthly
                        WorkingCellEnvironment["Seasonality"] = InternalGrid[ii, jj].CalculateNPPSeasonality(WorkingCellEnvironment["NPP"], WorkingCellEnvironment["Missing Value"][0]);
                        Changed = true;
                    }
                    // Otherwise convert the missing data values to zeroes where they exist amongst valid data eg in polar regions.
                    else
                    {
                        WorkingCellEnvironment["NPP"] = InternalGrid[ii, jj].ConvertMissingValuesToZero(WorkingCellEnvironment["NPP"], WorkingCellEnvironment["Missing Value"][0]);
                    }

                    // If the cell environment does not contain valid monthly mean diurnal temperature range data then interpolate values
                    if (InternalGrid[ii, jj].ContainsMissingValue(WorkingCellEnvironment["DiurnalTemperatureRange"], WorkingCellEnvironment["Missing Value"][0]))
                    {
                        //If NPP doesn't exist the interpolate from surrounding values (of the same realm)
                        WorkingCellEnvironment["DiurnalTemperatureRange"] = FillWithInterpolatedValues(ii, jj, GetCellLatitude(ii), GetCellLongitude(jj), "DiurnalTemperatureRange", WorkingCellEnvironment["Realm"][0]);

                        Changed = true;
                    }

                    // Same for u and v velocities
                    if (!InternalGrid[ii, jj].ContainsData(WorkingCellEnvironment["uVel"], WorkingCellEnvironment["Missing Value"][0]))
                    {
                        //If u doesn't exist the interpolate from surrounding values (of the same realm)
                        WorkingCellEnvironment["uVel"] = GetInterpolatedValues(ii, jj, GetCellLatitude(ii), GetCellLongitude(jj), "uVel", WorkingCellEnvironment["Realm"][0]);

                        Changed = true;
                    }
                    // Otherwise convert the missing data values to zeroes where they exist amongst valid data eg in polar regions.
                    else
                    {
                        WorkingCellEnvironment["uVel"] = InternalGrid[ii, jj].ConvertMissingValuesToZero(WorkingCellEnvironment["uVel"], WorkingCellEnvironment["Missing Value"][0]);
                    }

                    if (!InternalGrid[ii, jj].ContainsData(WorkingCellEnvironment["vVel"], WorkingCellEnvironment["Missing Value"][0]))
                    {
                        //If v vel doesn't exist the interpolate from surrounding values (of the same realm)
                        WorkingCellEnvironment["vVel"] = GetInterpolatedValues(ii, jj, GetCellLatitude(ii), GetCellLongitude(jj), "vVel", WorkingCellEnvironment["Realm"][0]);

                        Changed = true;
                    }
                    // Otherwise convert the missing data values to zeroes where they exist amongst valid data eg in polar regions.
                    else
                    {
                        WorkingCellEnvironment["vVel"] = InternalGrid[ii, jj].ConvertMissingValuesToZero(WorkingCellEnvironment["vVel"], WorkingCellEnvironment["Missing Value"][0]);
                    }

                    if (Changed) InternalGrid[ii, jj].CellEnvironment = WorkingCellEnvironment;
                }
            }
        }

        /// <summary>
        /// Calculate the weighted average of surrounding grid cell data, where those grid cells are of the specified realm and contain
        /// non missing data values
        /// </summary>
        /// <param name="latIndex">Index of the latitude cell for which the weighted average over surrounding cells is requested</param>
        /// <param name="lonIndex">Index of the longitude cell for which the weighted average over surrounding cells is requested</param>
        /// <param name="lat">Latitude of the cell for which the weighted value is requested</param>
        /// <param name="lon">Longitude of the cell for which the weighted value is requested</param>
        /// <param name="dataName">Names of the data for which weighted value is requested</param>
        /// <param name="realm">Realm of the grid cell for which data is to be averaged over</param>
        /// <returns>The weighted average value of the specified data type across surrounding grid cells of the specified realm</returns>
        private double[] GetInterpolatedValues(uint latIndex, uint lonIndex, double lat, double lon, string dataName, double realm)
        {
            SortedList<string, double[]> TempCellEnvironment = GetCellEnvironment(latIndex, lonIndex);
            double[] InterpData = new double[TempCellEnvironment[dataName].Length];
            uint[] InterpCount = new uint[TempCellEnvironment[dataName].Length];

            uint LowerLatIndex = latIndex - 1;
            uint UpperLatIndex = latIndex + 1;
            uint LowerLonIndex = lonIndex - 1;
            uint UpperLonIndex = lonIndex + 1;

            if (latIndex == 0) LowerLatIndex = latIndex;
            if (lat.CompareTo(this.MaxLatitude) == 0) UpperLatIndex = latIndex;

            if (lonIndex == 0) LowerLonIndex = lonIndex;
            if (lon.CompareTo(this.MaxLongitude) == 0) UpperLonIndex = lonIndex;

            //Loop over surrounding cells in the datalayer
            for (uint ii = LowerLatIndex; ii <= UpperLatIndex; ii++)
            {
                for (uint jj = LowerLonIndex; jj < UpperLonIndex; jj++)
                {
                    if (ii < _NumLatCells && jj < _NumLonCells)
                    {
                        TempCellEnvironment = GetCellEnvironment(ii, jj);

                        for (uint hh = 0; hh < InterpData.Length; hh++)
                        {
                            //If the cell contains data then sum this and increment count
                            if (TempCellEnvironment[dataName][hh] != TempCellEnvironment["Missing Value"][0] && TempCellEnvironment["Realm"][0] == realm)
                            {
                                InterpData[hh] += TempCellEnvironment[dataName][hh];
                                InterpCount[hh]++;
                            }
                        }
                    }
                }
            }

            //take the mean over surrounding valid cells for each timestep
            for (int hh = 0; hh < InterpData.Length; hh++)
            {
                if (InterpCount[hh] > 0)
                {
                    InterpData[hh] /= InterpCount[hh];
                }
                else
                {
                    InterpData[hh] = 0.0;
                }
            }
            return InterpData;
        }

        /// <summary>
        /// Calculate the weighted average of surrounding grid cell data, where those grid cells are of the specified realm and contain
        /// non missing data values
        /// </summary>
        /// <param name="latIndex">Index of the latitude cell for which the weighted average over surrounding cells is requested</param>
        /// <param name="lonIndex">Index of the longitude cell for which the weighted average over surrounding cells is requested</param>
        /// <param name="lat">Latitude of the cell for which the weighted value is requested</param>
        /// <param name="lon">Longitude of the cell for which the weighted value is requested</param>
        /// <param name="dataName">Names of the data for which weighted value is requested</param>
        /// <param name="realm">Realm of the grid cell for which data is to be averaged over</param>
        /// <returns>The weighted average value of the specified data type across surrounding grid cells of the specified realm</returns>
        private double[] FillWithInterpolatedValues(uint latIndex, uint lonIndex, double lat, double lon, string dataName, double realm)
        {
            SortedList<string, double[]> TempCellEnvironment = GetCellEnvironment(latIndex, lonIndex);
            double[] InterpData = new double[TempCellEnvironment[dataName].Length];
            uint[] InterpCount = new uint[TempCellEnvironment[dataName].Length];
            uint LowerLatIndex = latIndex - 1;
            uint UpperLatIndex = latIndex + 1;
            uint LowerLonIndex = lonIndex - 1;
            uint UpperLonIndex = lonIndex + 1;

            if (latIndex == 0) LowerLatIndex = latIndex;
            if (lat.CompareTo(this.MaxLatitude) == 0) UpperLatIndex = latIndex;

            if (lonIndex == 0) LowerLonIndex = lonIndex;
            if (lon.CompareTo(this.MaxLongitude) == 0) UpperLonIndex = lonIndex;

            for (uint hh = 0; hh < InterpData.Length; hh++)
            {
                if (TempCellEnvironment[dataName][hh] == TempCellEnvironment["Missing Value"][0])
                {
                    //Loop over surrounding cells in the datalayer
                    for (uint ii = LowerLatIndex; ii <= UpperLatIndex; ii++)
                    {
                        for (uint jj = LowerLonIndex; jj <= UpperLonIndex; jj++)
                        {
                            if (ii < _NumLatCells && jj < _NumLonCells)
                            {
                                TempCellEnvironment = GetCellEnvironment(ii, jj);

                                //If the cell contains data then sum this and increment count
                                if (TempCellEnvironment[dataName][hh] != TempCellEnvironment["Missing Value"][0] && TempCellEnvironment["Realm"][0] == realm)
                                {
                                    InterpData[hh] += TempCellEnvironment[dataName][hh];
                                    InterpCount[hh]++;
                                }

                            }
                        }
                    }
                    //take the mean over surrounding valid cells for each timestep
                    if (InterpCount[hh] > 0)
                    {
                        InterpData[hh] /= InterpCount[hh];
                    }
                    else
                    {
                        InterpData[hh] = 0.0;
                    }
                }
                else
                {
                    InterpData[hh] = TempCellEnvironment[dataName][hh];
                }
            }

            return InterpData;
        }

        /// <summary>
        /// Return the longitude of a cell at a particular lon. index
        /// </summary>
        /// <param name="cellLonIndex">The longitudinal index (i.e. row) of the cell</param>
        /// <returns>Returns the longitude of the bottom of the cell, in degrees</returns>
        public double GetCellLongitude(uint cellLonIndex)
        {
            Debug.Assert((cellLonIndex <= (_NumLonCells - 1)), "Error: Cell index out of range when trying to find the longitude for a particular cell");

            double TempLongitude = double.MaxValue;

            for (int ii = 0; ii < _NumLatCells; ii++)
            {
                if (InternalGrid[ii, cellLonIndex] != null)
                    TempLongitude = InternalGrid[ii, cellLonIndex].Longitude;
            }

            Debug.Assert(TempLongitude != double.MaxValue, "Error trying to find cell longitude - no grid cells have been initialised for this latitude index: " + cellLonIndex.ToString());

            return TempLongitude;
        }

        /// <summary>
        /// Return the latitude of a cell at a particular lat. index
        /// </summary>
        /// <param name="cellLatIndex">The latitudinal index (i.e. row) of the cell</param>
        /// <returns>Returns the latitude of the bottom of the cell, in degrees</returns>
        public double GetCellLatitude(uint cellLatIndex)
        {
            Debug.Assert((cellLatIndex <= (_NumLatCells - 1)), "Error: Cell index out of range when trying to find the latitude for a particular cell");

            double TempLatitude = double.MaxValue;

            for (int jj = 0; jj < _NumLonCells; jj++)
            {
                if (InternalGrid[cellLatIndex, jj] != null)
                {
                    TempLatitude = InternalGrid[cellLatIndex, jj].Latitude;
                    break;
                }
            }

            Debug.Assert(TempLatitude != double.MaxValue, "Error trying to find cell latitude - no grid cells have been initialised for this latitude index: " + cellLatIndex.ToString());

            return TempLatitude;

        }

        /// <summary>
        /// A method to return the values for all environmental data layers for a particular grid cell
        /// </summary>
        /// <param name="cellLatIndex">Latitude index of grid cell</param>
        /// <param name="cellLonIndex">Longitude index of grid cell</param>
        /// <returns>A sorted list containing environmental data layer names and values</returns>
        public SortedList<string, double[]> GetCellEnvironment(uint cellLatIndex, uint cellLonIndex)
        {
            return InternalGrid[cellLatIndex, cellLonIndex].CellEnvironment;
        }

    }
}
