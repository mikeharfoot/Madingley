using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.CSV;
using Microsoft.Research.Science.Data.Imperative;
using System.Diagnostics;

using Timing;

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

        /// <summary>
        /// Get the global missing value
        /// </summary>
        public double GlobalMissingValue
        {
            get { return _GlobalMissingValue; }
        }

        // Field to hold minimum latitude of the grid
        private float _MinLatitude;

        // Field to hold minumum longitude of the grid
        private float _MinLongitude;

        // Field to hold maximum latitude of the grid
        private float _MaxLatitude;

        // Field to hold maximum longitude of the grid
        private float _MaxLongitude;

        // Field to hold latitude resolution of each grid cell
        private float _LatCellSize;
        /// <summary>
        /// Get the latitudinal length of each grid cell. Currently assumes all cells are equal sized.
        /// </summary>
        public float LatCellSize
        {
            get { return _LatCellSize; }
        }

        // Field to hold longitude resolution of each grid cell
        private float _LonCellSize;
        /// <summary>
        /// Get the longitudinal length of each grid cell. Currently assumes all cells are equal sized. 
        /// </summary>
        public float LonCellSize
        {
            get { return _LonCellSize; }
        }

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
        /// Get the number of longitudinal cells in the model grid
        /// </summary>
        public UInt32 NumLonCells
        {
            get { return _NumLonCells; }
        }

        /// <summary>
        /// The bottom (southern-most) latitude of each row of grid cells
        /// </summary>
        private float[] _Lats;
        /// <summary>
        /// Get the bottom (southern-most) latitude of each row of grid cells
        /// </summary>
        public float[] Lats
        {
            get { return _Lats; }
        }

        /// <summary>
        /// The left (western-most) longitude of each column of grid cells
        /// </summary>
        private float[] _Lons;
        /// <summary>
        /// Get the left (western-most) longitude of each column of grid cells
        /// </summary>
        public float[] Lons
        {
            get { return _Lons; }
        }

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
        /// <param name="cohortFunctionalGroups">The functional group definitions for cohorts in the model</param>
        /// <param name="stockFunctionalGroups">The functional group definitions for stocks in the model</param>
        /// <param name="globalDiagnostics">Global diagnostic variables</param>
        /// <param name="tracking">Whether process tracking is enabled</param>
        /// <param name="specificLocations">Whether the model is to be run for specific locations</param>
        /// <param name="runInParallel">Whether model grid cells will be run in parallel</param>
        public ModelGrid(float minLat, float minLon, float maxLat, float maxLon, float latCellSize, float lonCellSize, List<uint[]> cellList,
            FunctionalGroupDefinitions cohortFunctionalGroups,
            FunctionalGroupDefinitions stockFunctionalGroups, SortedList<string, double> globalDiagnostics, Boolean tracking,
            Boolean specificLocations, Boolean runInParallel)
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
#endif
        {
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
        }

        /// <summary>
        /// Returns the stocks within the specified grid cell
        /// </summary>
        /// <param name="latIndex">Latitude index</param>
        /// <param name="lonIndex">Longitude index</param>
        /// <returns>The stock handler for the specified grid cell</returns>
        public GridCellStockHandler GetGridCellStocks(uint latIndex, uint lonIndex)
        {
            return InternalGrid[latIndex, lonIndex].GridCellStocks;
        }

        /// <summary>
        /// Returns the array (indexed by functional group) of lists of gridCellCohorts for the specified grid cell
        /// </summary>
        /// <param name="latIndex">Latitude index of grid cell</param>
        /// <param name="lonIndex">Longitude index of grid cell</param>
        /// <returns>Arry (indexed by functional group) of lists of gridCellCohorts</returns>
        public GridCellCohortHandler GetGridCellCohorts(uint latIndex, uint lonIndex)
        {
            return InternalGrid[latIndex, lonIndex].GridCellCohorts;
        }

        /// <summary>
        /// Return the value of a specified environmental layer from an individual grid cell
        /// </summary>
        /// <param name="variableName">The name of the environmental lyaer</param>
        /// <param name="timeInterval">The desired time interval within the environmental variable (i.e. 0 if it is a yearly variable
        /// or the month index - 0=Jan, 1=Feb etc. - for monthly variables)</param>
        /// <param name="latCellIndex">The latitudinal cell index</param>
        /// <param name="lonCellIndex">The longitudinal cell index</param>
        /// <param name="variableExists">Returns false if the environmental layer does not exist, true if it does</param>
        /// <returns>The value of the environmental layer, or a missing value if the environmental layer does not exist</returns>
        public double GetEnviroLayer(string variableName, uint timeInterval, uint latCellIndex, uint lonCellIndex, out bool variableExists)
        {
            return InternalGrid[latCellIndex, lonCellIndex].GetEnviroLayer(variableName, timeInterval, out variableExists);
        }

        /// <summary>
        /// Get the total of a state variable for specific cells
        /// </summary>
        /// <param name="variableName">The name of the variable</param>
        /// <param name="traitValue">The functional group trait value to get data for</param>
        /// <param name="functionalGroups">A vector of functional group indices to consider</param>
        /// <param name="cellIndices">List of indices of active cells in the model grid</param>
        /// <param name="stateVariableType">A string indicating the type of state variable; 'cohort' or 'stock'</param>
        /// <param name="initialisation">The Madingley Model intialisation</param>
        /// <returns>Summed value of variable over whole grid</returns>
        /// <todo>Overload to work with vector and array state variables</todo>
        public double StateVariableGridTotal(string variableName, string traitValue, int[] functionalGroups, List<uint[]> cellIndices,
            string stateVariableType, MadingleyModelInitialisation initialisation)
        {

            double tempVal = 0;

            double[,] TempStateVariable = this.GetStateVariableGrid(variableName, traitValue, functionalGroups, cellIndices, stateVariableType, initialisation);

            // Loop through and sum values across a grid, excluding missing values
            for (int ii = 0; ii < cellIndices.Count; ii++)
            {
                tempVal += TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]];
            }

            return tempVal;
        }

        /// <summary>
        /// Gets a state variable for specified functional groups of specified entity types in a specified grid cell
        /// </summary>
        /// <param name="variableName">The name of the variable to get: 'biomass' or 'abundance'</param>
        /// <param name="traitValue">The functional group trait value to get data for</param>
        /// <param name="functionalGroups">The functional group indices to get the state variable for</param>
        /// <param name="latCellIndex">The latitudinal index of the cell</param>
        /// <param name="lonCellIndex">The longitudinal index of the cell</param>
        /// <param name="stateVariableType">The type of entity to return the state variable for: 'stock' or 'cohort'</param>
        /// <param name="modelInitialisation">The Madingley Model initialisation</param>
        /// <returns>The state variable for specified functional groups of specified entity types in a specified grid cell</returns>
        public double GetStateVariable(string variableName, string traitValue, int[] functionalGroups, uint latCellIndex, uint lonCellIndex,
            string stateVariableType, MadingleyModelInitialisation modelInitialisation)
        {

            double returnValue = 0.0;

            switch (stateVariableType.ToLower())
            {
                case "cohort":

                    GridCellCohortHandler TempCohorts = InternalGrid[latCellIndex, lonCellIndex].GridCellCohorts;

                    switch (variableName.ToLower())
                    {
                        case "biomass":
                            if (traitValue != "Zooplankton")
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        returnValue += ((item.IndividualBodyMass + item.IndividualReproductivePotentialMass) * item.CohortAbundance);
                                    }
                                }
                            }
                            else
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        if (item.IndividualBodyMass <= modelInitialisation.PlanktonDispersalThreshold)
                                            returnValue += ((item.IndividualBodyMass + item.IndividualReproductivePotentialMass) * item.CohortAbundance);
                                    }
                                }
                            }
                            break;

                        case "abundance":
                            if (traitValue != "Zooplankton")
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        returnValue += item.CohortAbundance;
                                    }
                                }
                            }
                            else
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        if (item.IndividualBodyMass <= modelInitialisation.PlanktonDispersalThreshold)
                                            returnValue += item.CohortAbundance;
                                    }
                                }
                            }
                            break;

                        default:
                            Debug.Fail("For cohorts, state variable name must be either 'biomass' or 'abundance'");
                            break;
                    }
                    break;

                case "stock":
                    GridCellStockHandler TempStocks = InternalGrid[latCellIndex, lonCellIndex].GridCellStocks;

                    switch (variableName.ToLower())
                    {
                        case "biomass":
                            foreach (int f in functionalGroups)
                            {
                                foreach (var item in TempStocks[f])
                                {
                                    returnValue += item.TotalBiomass;
                                }
                            }
                            break;
                        default:
                            Debug.Fail("For stocks, state variable name must be 'biomass'");
                            break;
                    }
                    break;

                default:
                    Debug.Fail("State variable type must be either 'cohort' or 'stock'");
                    break;

            }

            return returnValue;
        }

        /// <summary>
        /// Gets a state variable density for specified functional groups of specified entity types in a specified grid cell
        /// </summary>
        /// <param name="variableName">The name of the variable to get: 'biomass' or 'abundance'</param>
        /// <param name="traitValue">The functional group trait value to get data for</param>
        /// <param name="functionalGroups">The functional group indices to get the state variable for</param>
        /// <param name="latCellIndex">The latitudinal index of the cell</param>
        /// <param name="lonCellIndex">The longitudinal index of the cell</param>
        /// <param name="stateVariableType">The type of entity to return the state variable for: 'stock' or 'cohort'</param>
        /// <param name="modelInitialisation">The Madingley Model initialisation</param>
        /// <returns>The state variable density for specified functional groups of specified entity types in a specified grid cell</returns>
        public double GetStateVariableDensity(string variableName, string traitValue, int[] functionalGroups, uint latCellIndex,
            uint lonCellIndex, string stateVariableType, MadingleyModelInitialisation modelInitialisation)
        {

            double returnValue = 0.0;

            switch (stateVariableType.ToLower())
            {
                case "cohort":

                    GridCellCohortHandler TempCohorts = InternalGrid[latCellIndex, lonCellIndex].GridCellCohorts;

                    switch (variableName.ToLower())
                    {
                        case "biomass":
                            if (traitValue != "Zooplankton (all)")
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        returnValue += ((item.IndividualBodyMass + item.IndividualReproductivePotentialMass) * item.CohortAbundance);
                                    }
                                }
                            }
                            else
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        if (item.IndividualBodyMass <= modelInitialisation.PlanktonDispersalThreshold)
                                            returnValue += ((item.IndividualBodyMass + item.IndividualReproductivePotentialMass) * item.CohortAbundance);
                                    }
                                }
                            }
                            break;

                        case "abundance":
                            if (traitValue != "Zooplankton (all)")
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        returnValue += item.CohortAbundance;
                                    }
                                }
                            }
                            else
                            {
                                foreach (int f in functionalGroups)
                                {
                                    foreach (var item in TempCohorts[f])
                                    {
                                        if (item.IndividualBodyMass <= modelInitialisation.PlanktonDispersalThreshold)
                                            returnValue += item.CohortAbundance;
                                    }
                                }
                            }
                            break;

                        default:
                            Debug.Fail("For cohorts, state variable name must be either 'biomass' or 'abundance'");
                            break;
                    }
                    break;

                case "stock":
                    GridCellStockHandler TempStocks = InternalGrid[latCellIndex, lonCellIndex].GridCellStocks;

                    switch (variableName.ToLower())
                    {
                        case "biomass":
                            foreach (int f in functionalGroups)
                            {
                                foreach (var item in TempStocks[f])
                                {
                                    returnValue += item.TotalBiomass;
                                }
                            }
                            break;
                        default:
                            Debug.Fail("For stocks, state variable name must be 'biomass'");
                            break;
                    }
                    break;

                default:
                    Debug.Fail("State variable type must be either 'cohort' or 'stock'");
                    break;

            }

            return returnValue / (InternalGrid[latCellIndex, lonCellIndex].CellEnvironment["Cell Area"][0]);
        }

        /// <summary>
        /// Return an array of values for a single state variable over specific cells
        /// </summary>
        /// <param name="variableName">Variable name</param>
        /// <param name="traitValue">The trait values of functional groups to get data for</param>
        /// <param name="functionalGroups">A vector of functional group indices to consider</param>
        /// <param name="cellIndices">List of indices of active cells in the model grid</param>
        /// <param name="stateVariableType">A string indicating the type of state variable; 'cohort' or 'stock'</param>
        /// <param name="initialisation">The Madingley Model initialisation</param>
        /// <returns>Array of state variable values for each grid cell</returns>
        public double[,] GetStateVariableGrid(string variableName, string traitValue, int[] functionalGroups, List<uint[]> cellIndices,
            string stateVariableType, MadingleyModelInitialisation initialisation)
        {
            double[,] TempStateVariable = new double[this.NumLatCells, this.NumLonCells];

            switch (variableName.ToLower())
            {
                case "biomass":
                    for (int ii = 0; ii < cellIndices.Count; ii++)
                    {
                        // Check whether the state variable concerns cohorts or stocks
                        if (stateVariableType.ToLower() == "cohort")
                        {
                            if (traitValue != "Zooplankton")
                            {
                                // Check to make sure that the cell has at least one cohort
                                if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts != null)
                                {
                                    for (int nn = 0; nn < functionalGroups.Length; nn++)
                                    {
                                        if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]] != null)
                                        {
                                            foreach (Cohort item in InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]].ToArray())
                                            {
                                                TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] += ((item.IndividualBodyMass + item.IndividualReproductivePotentialMass) * item.CohortAbundance);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Check to make sure that the cell has at least one cohort
                                if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts != null)
                                {
                                    for (int nn = 0; nn < functionalGroups.Length; nn++)
                                    {
                                        if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]] != null)
                                        {
                                            foreach (Cohort item in InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]].ToArray())
                                            {
                                                if (item.IndividualBodyMass <= initialisation.PlanktonDispersalThreshold)
                                                    TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] += ((item.IndividualBodyMass + item.IndividualReproductivePotentialMass) * item.CohortAbundance);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (stateVariableType.ToLower() == "stock")
                        {
                            // Check to make sure that the cell has at least one stock
                            if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellStocks != null)
                            {
                                for (int nn = 0; nn < functionalGroups.Length; nn++)
                                {
                                    if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellStocks[functionalGroups[nn]] != null)
                                    {
                                        foreach (Stock item in InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellStocks[functionalGroups[nn]].ToArray())
                                        {
                                            TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] += (item.TotalBiomass);

                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            Debug.Fail("Variable 'state variable type' must be either 'stock' 'or 'cohort'");
                        }

                    }
                    break;
                case "abundance":
                    for (int ii = 0; ii < cellIndices.Count; ii++)
                    {
                        // Check whether the state variable concerns cohorts or stocks
                        if (stateVariableType.ToLower() == "cohort")
                        {
                            if (traitValue != "Zooplankton")
                            {
                                // Check to make sure that the cell has at least one cohort
                                if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts != null)
                                {
                                    for (int nn = 0; nn < functionalGroups.Length; nn++)
                                    {
                                        if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]] != null)
                                        {
                                            foreach (Cohort item in InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]].ToArray())
                                            {
                                                TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] += item.CohortAbundance;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Check to make sure that the cell has at least one cohort
                                if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts != null)
                                {
                                    for (int nn = 0; nn < functionalGroups.Length; nn++)
                                    {
                                        if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]] != null)
                                        {
                                            foreach (Cohort item in InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].GridCellCohorts[functionalGroups[nn]].ToArray())
                                            {
                                                if (item.IndividualBodyMass <= initialisation.PlanktonDispersalThreshold)
                                                    TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] += item.CohortAbundance;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Fail("Currently abundance cannot be calculated for grid cell stocks");
                        }
                    }
                    break;
                default:
                    Debug.Fail("Invalid search string passed for cohort property");
                    break;
            }

            return TempStateVariable;

        }

        /// <summary>
        /// Return an array of log(values + 1) for a state variable for particular functional groups over specific cells. State variable (currently only biomass or abundance) must be >= 0 in all grid cells
        /// </summary>
        /// <param name="variableName">The name of the variable</param>
        /// <param name="traitValue">The functional group trait value to get data for</param>
        /// <param name="functionalGroups">A vector of functional group indices to consider</param>
        /// <param name="cellIndices">List of indices of active cells in the model grid</param>
        /// <param name="stateVariableType">A string indicating the type of state variable; 'cohort' or 'stock'</param>
        /// <param name="initialisation">The Madingley Model intialisation</param>
        /// <returns>Array of log(state variable values +1 ) for each grid cell</returns>
        public double[,] GetStateVariableGridLogDensityPerSqKm(string variableName, string traitValue, int[] functionalGroups,
            List<uint[]> cellIndices, string stateVariableType, MadingleyModelInitialisation initialisation)
        {

            double[,] TempStateVariable = new double[this.NumLatCells, this.NumLonCells];
            double CellArea;

            TempStateVariable = this.GetStateVariableGrid(variableName, traitValue, functionalGroups, cellIndices, stateVariableType, initialisation);

            for (int ii = 0; ii < cellIndices.Count; ii++)
            {
                CellArea = GetCellEnvironment(cellIndices[ii][0], cellIndices[ii][1])["Cell Area"][0];
                TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] /= CellArea;
                TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] = Math.Log(TempStateVariable[cellIndices[ii][0], cellIndices[ii][1]] + 1);
            }

            return TempStateVariable;

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

        /// <summary>
        /// Get a grid of values for an environmental data layer
        /// </summary>
        /// <param name="enviroVariable"> The name of the environmental data layer</param>
        /// <param name="timeInterval">The desired time interval within the environmental variable (i.e. 0 if it is a yearly variable
        /// or the month index - 0=Jan, 1=Feb etc. - for monthly variables)</param>
        /// <returns>The values in each grid cell</returns>
        public double[,] GetEnviroGrid(string enviroVariable, uint timeInterval)
        {
            // Check to see if environmental variable exists
            for (int ii = 0; ii < _NumLatCells; ii++)
            {
                for (int jj = 0; jj < _NumLonCells; jj++)
                {
                    if (InternalGrid[ii, jj] != null)
                        Debug.Assert(InternalGrid[ii, jj].CellEnvironment.ContainsKey(enviroVariable), "Environmental variable not found when running GetEnviroGrid");
                }
            }

            double[,] outputData = new double[_NumLatCells, _NumLonCells];

            for (int ii = 0; ii < _NumLatCells; ii += GridCellRarefaction)
            {
                for (int jj = 0; jj < _NumLonCells; jj += GridCellRarefaction)
                {
                    outputData[ii, jj] = InternalGrid[ii, jj].CellEnvironment[enviroVariable][timeInterval];
                }
            }

            return outputData;
        }

        /// <summary>
        /// Get a grid of values for an environmental data layer in specific cells
        /// </summary>
        /// <param name="enviroVariable">The name of the environmental data layer to return</param>
        /// <param name="timeInterval">The desired time interval for which to get data (i.e. 0 if it is a yearly variable
        /// or the month index - 0=Jan, 1=Feb etc. - for monthly variables)</param>
        /// <param name="cellIndices">List of active cells in the model grid</param>
        /// <returns>The values in each grid cell</returns>
        public double[,] GetEnviroGrid(string enviroVariable, uint timeInterval, List<uint[]> cellIndices)
        {
            // Check to see if environmental variable exists
            for (int ii = 0; ii < cellIndices.Count; ii++)
            {
                if (InternalGrid[cellIndices[ii][0], cellIndices[ii][1]] != null)
                    Debug.Assert(InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].CellEnvironment.ContainsKey(enviroVariable),
                        "Environmental variable not found when running GetEnviroGrid");

            }

            // Create grid to hold the data to return
            double[,] outputData = new double[_NumLatCells, _NumLonCells];

            for (int ii = 0; ii < cellIndices.Count; ii++)
            {
                outputData[cellIndices[ii][0], cellIndices[ii][1]] = InternalGrid[cellIndices[ii][0], cellIndices[ii][1]].CellEnvironment
                    [enviroVariable][timeInterval];
            }

            return outputData;
        }

        /// <summary>
        /// Return the sum of an environmental variable over specific cells
        /// </summary>
        /// <param name="enviroVariable">The environmental variable</param>
        /// <param name="timeInterval">The desired time interval within the environmental variable (i.e. 0 if it is a yearly variable
        /// or the month index - 0=Jan, 1=Feb etc. - for monthly variables)</param>
        /// <param name="cellIndices">List of active cells in the model grid</param>
        /// <returns>The total of the variable over the whole grid</returns>
        public double GetEnviroGridTotal(string enviroVariable, uint timeInterval, List<uint[]> cellIndices)
        {
            double[,] enviroGrid = GetEnviroGrid(enviroVariable, timeInterval, cellIndices);
            double enviroTotal = 0.0;

            for (int ii = 0; ii < cellIndices.Count; ii++)
            {
                enviroTotal += enviroGrid[cellIndices[ii][0], cellIndices[ii][1]];
            }

            return enviroTotal;
        }

#if true
        public void SetGridCells(GridCell[] gridCells, List<uint[]> cellList)
        {
            // Set up a grid of grid cells
            this.InternalGrid = new GridCell[_NumLatCells, _NumLonCells];

            // Loop over cells to set up the model grid
            for (int ii = 0; ii < cellList.Count; ii++)
            {
                // Create the grid cell at the specified position
                InternalGrid[cellList[ii][0], cellList[ii][1]] = gridCells[ii];
            }
        }
#endif
    }
}
