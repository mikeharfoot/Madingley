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


#if true
        /// <summary>
        /// Constructor for a grid cell; creates cell and reads in environmental data
        /// </summary>
        /// <param name="latitude">The latitude of the grid cell</param>
        /// <param name="latIndex">The latitudinal index of the grid cell</param>
        /// <param name="longitude">The longitude of the grid cell</param>
        /// <param name="lonIndex">The longitudinal index of the grid cell</param>
        /// <param name="latCellSize">The latitudinal dimension of the grid cell</param>
        /// <param name="lonCellSize">The longitudinal dimension of the grid cell</param>
        /// <param name="dataLayers">A list of environmental data variables in the model</param>
        /// <param name="missingValue">The missing value to be applied to all data in the grid cell</param>
        /// <param name="specificLocations">Whether the model is being run for specific locations</param>
        public GridCell(float latitude, uint latIndex, float longitude, uint lonIndex, float latCellSize, float lonCellSize,
            SortedList<string, EnviroData> dataLayers, double missingValue, 
            bool specificLocations)
#else
        /// <summary>
        /// Constructor for a grid cell; creates cell and reads in environmental data
        /// </summary>
        /// <param name="latitude">The latitude of the grid cell</param>
        /// <param name="latIndex">The latitudinal index of the grid cell</param>
        /// <param name="longitude">The longitude of the grid cell</param>
        /// <param name="lonIndex">The longitudinal index of the grid cell</param>
        /// <param name="latCellSize">The latitudinal dimension of the grid cell</param>
        /// <param name="lonCellSize">The longitudinal dimension of the grid cell</param>
        /// <param name="dataLayers">A list of environmental data variables in the model</param>
        /// <param name="missingValue">The missing value to be applied to all data in the grid cell</param>
        /// <param name="cohortFunctionalGroups">The definitions for cohort functional groups in the model</param>
        /// <param name="stockFunctionalGroups">The definitions for stock functional groups in the model</param>
        /// <param name="globalDiagnostics">A list of global diagnostic variables for the model grid</param>
        /// <param name="tracking">Whether process-tracking is enabled</param>
        /// <param name="specificLocations">Whether the model is being run for specific locations</param>
        public GridCell(float latitude, uint latIndex, float longitude, uint lonIndex, float latCellSize, float lonCellSize, 
            SortedList<string, EnviroData> dataLayers, double missingValue, FunctionalGroupDefinitions cohortFunctionalGroups, 
            FunctionalGroupDefinitions stockFunctionalGroups, SortedList<string, double> globalDiagnostics,Boolean tracking,
            bool specificLocations)
#endif
        {

            // Boolean to track when environmental data are missing
            Boolean EnviroMissingValue;

            // Temporary vector for holding initial values of grid cell properties
            double[] tempVector;

            // Set the grid cell values of latitude, longitude and missing value as specified
            _Latitude = latitude;
            _Longitude = longitude;


            // Initialise list of environmental data layer values
            _CellEnvironment = new SortedList<string, double[]>();

            //Add the latitude and longitude
            tempVector = new double[1];
            tempVector[0] = latitude;
            _CellEnvironment.Add("Latitude", tempVector);
            tempVector = new double[1];
            tempVector[0] = longitude;
            _CellEnvironment.Add("Longitude", tempVector);


            // Add an organic matter pool to the cell environment to track organic biomass not held by animals or plants with an initial value of 0
            tempVector = new double[1];
            tempVector[0] = 0.0;
            _CellEnvironment.Add("Organic Pool", tempVector);

            // Add a repsiratory CO2 pool to the cell environment with an initial value of 0
            tempVector = new double[1];
            tempVector[0] = 0.0;
            _CellEnvironment.Add("Respiratory CO2 Pool", tempVector);

            // Add the grid cell area (in km2) to the cell environment with an initial value of 0
            tempVector = new double[1];
            // Calculate the area of this grid cell
            tempVector[0] = Utilities.CalculateGridCellArea(latitude, lonCellSize, latCellSize);
            // Add it to the cell environment
            _CellEnvironment.Add("Cell Area", tempVector);

            //Add the latitude and longitude indices
            tempVector = new double[1];
            tempVector[0] = latIndex;
            _CellEnvironment.Add("LatIndex", tempVector);
            tempVector = new double[1];
            tempVector[0] = lonIndex;
            _CellEnvironment.Add("LonIndex", tempVector);


            // Add the missing value of data in the grid cell to the cell environment
            tempVector = new double[1];
            tempVector[0] = missingValue;
            _CellEnvironment.Add("Missing Value", tempVector);

            // Loop through environmental data layers and extract values for this grid cell
            // Also standardise missing values

            // Loop over variables in the list of environmental data
            foreach (string LayerName in dataLayers.Keys)
            {
                // Initiliase the temporary vector of values to be equal to the number of time intervals in the environmental variable
                tempVector = new double[dataLayers[LayerName].NumTimes];
                // Loop over the time intervals in the environmental variable
                for (int hh = 0; hh < dataLayers[LayerName].NumTimes; hh++)
                {
                    // Add the value of the environmental variable at this time interval to the temporary vector
                    tempVector[hh] = dataLayers[LayerName].GetValue(_Latitude, _Longitude, (uint)hh, out EnviroMissingValue,latCellSize,lonCellSize);
                    // If the environmental variable is a missing value, then change the value to equal the standard missing value for this cell
                    if (EnviroMissingValue)
                        tempVector[hh] = missingValue;
                }
                // Add the values of the environmental variables to the cell environment, with the name of the variable as the key
                _CellEnvironment.Add(LayerName, tempVector);
            }


            if (_CellEnvironment.ContainsKey("LandSeaMask"))
            {
                if (_CellEnvironment["LandSeaMask"][0].CompareTo(0.0) == 0)
                {
                    if (ContainsData(_CellEnvironment["OceanTemp"], _CellEnvironment["Missing Value"][0]))
                    {
                        //This is a marine cell
                        tempVector = new double[1];
                        tempVector[0] = 2.0;
                        _CellEnvironment.Add("Realm", tempVector);

                        _CellEnvironment.Add("NPP", _CellEnvironment["OceanNPP"]);
                        _CellEnvironment.Add("DiurnalTemperatureRange", _CellEnvironment["OceanDTR"]);
                        if (_CellEnvironment.ContainsKey("Temperature"))
                        {
                            if(_CellEnvironment.ContainsKey("SST"))
                            {
                                _CellEnvironment["Temperature"] = _CellEnvironment["SST"];
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            _CellEnvironment.Add("Temperature", _CellEnvironment["SST"]);
                        }

                    }
                    else
                    {
                        //This is a freshwater cell and in this model formulation is characterised as belonging to the terrestrial realm
                        tempVector = new double[1];
                        tempVector[0] = 1.0;
                        _CellEnvironment.Add("Realm", tempVector);

                        _CellEnvironment.Add("NPP", _CellEnvironment["LandNPP"]);
                        _CellEnvironment.Add("DiurnalTemperatureRange", _CellEnvironment["LandDTR"]);
                    }
                }
                else
                {
                    //This is a land cell
                    tempVector = new double[1];
                    tempVector[0] = 1.0;
                    _CellEnvironment.Add("Realm", tempVector);

                    _CellEnvironment.Add("NPP", _CellEnvironment["LandNPP"]);
                    _CellEnvironment.Add("DiurnalTemperatureRange", _CellEnvironment["LandDTR"]);

                }
            }
            else
            {
                Debug.Fail("No land sea mask defined - a mask is required to initialise appropriate ecology");
            }

            //Calculate and add the standard deviation of monthly temperature as a measure of seasonality
            //Also calculate and add the annual mean temperature for this cell
            tempVector = new double[12];
            double[] sdtemp = new double[12];
            double[] meantemp = new double[12];

            tempVector = _CellEnvironment["Temperature"];

            double Average = tempVector.Average();
            meantemp[0] = Average;
            double SumOfSquaresDifferences = tempVector.Select(val => (val - Average) * (val - Average)).Sum();
            sdtemp[0] = Math.Sqrt(SumOfSquaresDifferences / tempVector.Length);

            _CellEnvironment.Add("SDTemperature", sdtemp);
            _CellEnvironment.Add("AnnualTemperature", meantemp);

            //Remove unrequired cell environment layers
            if (_CellEnvironment.ContainsKey("LandNPP")) _CellEnvironment.Remove("LandNPP");
            if (_CellEnvironment.ContainsKey("LandDTR")) _CellEnvironment.Remove("LandDTR");
            if (_CellEnvironment.ContainsKey("OceanNPP")) _CellEnvironment.Remove("OceanNPP");
            if (_CellEnvironment.ContainsKey("OceanDTR")) _CellEnvironment.Remove("OceanDTR");
            if (_CellEnvironment.ContainsKey("SST")) _CellEnvironment.Remove("SST");

            // CREATE NPP SEASONALITY LAYER
            _CellEnvironment.Add("Seasonality", CalculateNPPSeasonality(_CellEnvironment["NPP"], _CellEnvironment["Missing Value"][0]));

            // Calculate other climate variables from temperature and precipitation

            // Calculate the fraction of the year that experiences frost
            double[] NDF = new double[1];
            NDF[0] = CVC.GetNDF(_CellEnvironment["FrostDays"], _CellEnvironment["Temperature"],_CellEnvironment["Missing Value"][0]);
            _CellEnvironment.Add("Fraction Year Frost", NDF);

            double[] frostMonthly = new double[12];
            frostMonthly[0] = Math.Min(_CellEnvironment["FrostDays"][0] / 31.0, 1.0);
            frostMonthly[1] = Math.Min(_CellEnvironment["FrostDays"][1] / 28.0, 1.0);
            frostMonthly[2] = Math.Min(_CellEnvironment["FrostDays"][2] / 31.0, 1.0);
            frostMonthly[3] = Math.Min(_CellEnvironment["FrostDays"][3] / 30.0, 1.0);
            frostMonthly[4] = Math.Min(_CellEnvironment["FrostDays"][4] / 31.0, 1.0);
            frostMonthly[5] = Math.Min(_CellEnvironment["FrostDays"][5] / 30.0, 1.0);
            frostMonthly[6] = Math.Min(_CellEnvironment["FrostDays"][6] / 31.0, 1.0);
            frostMonthly[7] = Math.Min(_CellEnvironment["FrostDays"][7] / 31.0, 1.0);
            frostMonthly[8] = Math.Min(_CellEnvironment["FrostDays"][8] / 30.0, 1.0);
            frostMonthly[9] = Math.Min(_CellEnvironment["FrostDays"][9] / 31.0, 1.0);
            frostMonthly[10] = Math.Min(_CellEnvironment["FrostDays"][10] / 30.0, 1.0);
            frostMonthly[11] = Math.Min(_CellEnvironment["FrostDays"][11] / 31.0, 1.0);

            _CellEnvironment.Add("Fraction Month Frost", frostMonthly);
            _CellEnvironment.Remove("FrostDays");

            // Calculate AET and the fractional length of the fire season
            Tuple<double[], double, double> TempTuple = new Tuple<double[], double, double>(new double[12], new double(), new double());
            TempTuple = CVC.MonthlyActualEvapotranspirationSoilMoisture(_CellEnvironment["AWC"][0], _CellEnvironment["Precipitation"], _CellEnvironment["Temperature"]);
            _CellEnvironment.Add("AET", TempTuple.Item1);
            _CellEnvironment.Add("Fraction Year Fire", new double[1] { TempTuple.Item3 / 360 });

            // Designate a breeding season for this grid cell, where a month is considered to be part of the breeding season if its NPP is at
            // least 80% of the maximum NPP throughout the whole year
            double[] BreedingSeason = new double[12];
            for (int i = 0; i < 12; i++)
            {
                if ((_CellEnvironment["Seasonality"][i] / _CellEnvironment["Seasonality"].Max()) > 0.5)
                {
                    BreedingSeason[i] = 1.0;
                }
                else
                {
                    BreedingSeason[i] = 0.0;
                }
            }
            _CellEnvironment.Add("Breeding Season", BreedingSeason);
        }

        /// <summary>
        /// Converts any missing values to zeroes
        /// </summary>
        /// <param name="data">the data vector to convert</param>
        /// <param name="missingValue">Missing data value to be converted to zero</param>
        /// <returns>The data vector with any missing data values converted to zero</returns>
        public double[] ConvertMissingValuesToZero(double[] data, double missingValue)
        {
            double[] TempArray = data;

            for (int ii = 0; ii < TempArray.Length; ii++)
            {
                TempArray[ii] = (TempArray[ii].CompareTo(missingValue) != 0) ? TempArray[ii] : 0.0;
            }

            return TempArray;
        }

        /// <summary>
        /// Checks if any non-missing value data exists in the vector data
        /// </summary>
        /// <param name="data">The data vector to be checked</param>
        /// <param name="missingValue">The missing value to which the data will be compared</param>
        /// <returns>True if non missing values are found, false if not</returns>
        public Boolean ContainsData(double[] data, double missingValue)
        {
            Boolean ContainsData = false;
            for (int ii = 0; ii < data.Length; ii++)
            {
                if (data[ii].CompareTo(missingValue) != 0) ContainsData = true;
            }
            return ContainsData;
        }



        /// <summary>
        /// Checks if any non-missing value data exists in the vector data
        /// </summary>
        /// <param name="data">The data vector to be checked</param>
        /// <param name="missingValue">The missing value to which the data will be compared</param>
        /// <returns>True if non missing values are found, false if not</returns>
        public Boolean ContainsMissingValue(double[] data, double missingValue)
        {
            Boolean ContainsMV = false;
            for (int ii = 0; ii < data.Length; ii++)
            {
                if (data[ii].CompareTo(missingValue) == 0) ContainsMV = true;
            }
            return ContainsMV;
        }

        /// <summary>
        /// Calculate monthly seasonality values of Net Primary Production - ignores missing values. If there is no NPP data (ie all zero or missing values)
        /// then assign 1/12 for each month.
        /// </summary>
        /// <param name="NPP">Monthly values of NPP</param>
        /// <param name="missingValue">Missing data value to which the data will be compared against</param>
        /// <returns>The contribution that each month's NPP makes to annual NPP</returns>
        public double[] CalculateNPPSeasonality(double[] NPP, double missingValue)
        {

            // Check that the NPP data is of monthly temporal resolution
            Debug.Assert(NPP.Length == 12, "Error: currently NPP data must be of monthly temporal resolution");

            // Temporary vector to hold seasonality values
            double[] NPPSeasonalityValues = new double[12];

            // Loop over months and calculate total annual NPP
            double TotalNPP = 0.0;
            for (int i = 0; i < 12; i++)
            {
                if (NPP[i].CompareTo(missingValue) != 0 && NPP[i].CompareTo(0.0) > 0) TotalNPP += NPP[i];
            }
            if (TotalNPP.CompareTo(0.0) == 0)
            {
                // Loop over months and calculate seasonality
                // If there is no NPP value then asign a uniform flat seasonality
                for (int i = 0; i < 12; i++)
                {
                    NPPSeasonalityValues[i] = 1.0/12.0;
                }

            }
            else
            {
                // Some NPP data exists for this grid cell so use that to infer the NPP seasonality
                // Loop over months and calculate seasonality
                for (int i = 0; i < 12; i++)
                {
                    if (NPP[i].CompareTo(missingValue) != 0 && NPP[i].CompareTo(0.0) > 0)
                    {
                        NPPSeasonalityValues[i] = NPP[i] / TotalNPP;
                    }
                    else
                    {
                        NPPSeasonalityValues[i] = 0.0;
                    }
                }
            }

            return NPPSeasonalityValues;
        }
    }


}

