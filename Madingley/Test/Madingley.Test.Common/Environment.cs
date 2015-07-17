//#define HAVE_MUCH_MEMORY

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;

namespace Madingley.Test.Common
{
    public static class Environment
    {
        public static void CreateNetCdf2d<X, Y, V>(
            string filename,
            string[] geographicalDimensions,
            string[] units,
            string dataName,
            string dataUnits,
            string missingValue,
            Func<float, X> xconv,
            Func<float, Y> yconv,
            Func<int, int, V> mask)
        {
            var longitudeCount = 3600;
            var longitudeMinimum = -180.0f;
            var longitudeStep = 0.1f;

            var latitudeCount = 1800;
            var latitudeMinimum = -89.9f;
            var latitudeStep = 0.1f;

            var longitudes = new X[longitudeCount];
            var latitudes = new Y[latitudeCount];
            var data2 = new V[longitudeCount, latitudeCount];

            for (var j = 0; j < longitudeCount; j++)
            {
                longitudes[j] = xconv(longitudeMinimum + j * longitudeStep);
            }

            for (var i = 0; i < latitudeCount; i++)
            {
                latitudes[i] = yconv(latitudeMinimum + i * latitudeStep);
            }

            for (var j = 0; j < longitudeCount; j++)
            {
                for (var i = 0; i < latitudeCount; i++)
                {
                    data2[j, i] = mask(j, i);
                }
            }

            var fullFilename = "msds:nc?file=" + filename + ".nc&openMode=create";
            var dataSet = DataSet.Open(fullFilename);
            dataSet.IsAutocommitEnabled = false;

            var longitudev = dataSet.AddVariable<X>(geographicalDimensions[0], longitudes, geographicalDimensions[0]);
            //var longitudev = dataSet.AddVariable<float>(geographicalDimensions[0], geographicalDimensions[0]);
            //dataSet.PutData<float[]>(longitudev.ID, longitudes, DataSet.FromToEnd(0));
            if (units.Length > 0)
            {
                longitudev.Metadata["units"] = units[0];
            }

            var latitudev = dataSet.AddVariable<Y>(geographicalDimensions[1], latitudes, geographicalDimensions[1]);
            if (units.Length > 1)
            {
                latitudev.Metadata["units"] = units[1];
            }

            var datav = dataSet.AddVariable<V>(dataName, data2, geographicalDimensions);
            if (dataUnits.Length > 0)
            {
                datav.Metadata["units"] = "unitless";
            }
            if (missingValue.Length > 0)
            {
                datav.Metadata["missing_value"] = missingValue;
            }

            dataSet.Commit();
        }

        public static void CreateNetCdf2d(
            string filename,
            string[] geographicalDimensions,
            string dataName,
            string missingValue,
            Func<int, int, int, double> npp)
        {
            var longitudeCount = 180;
            var longitudeMinimum = -180.0;
            var longitudeStep = 360.0 / longitudeCount;

            var latitudeCount = 90;
            var latitudeMinimum = -89.9;
            var latitudeStep = 180.0 / latitudeCount;

            var timestepCount = 12;

            var longitudes = new double[longitudeCount];
            var latitudes = new double[latitudeCount];
            var timesteps = new int[timestepCount];
#if HAVE_MUCH_MEMORY
            var data3 = new double[longitudeCount, latitudeCount, timestepCount];
#else
            var data2 = new double[longitudeCount, latitudeCount];
#endif

            for (var j = 0; j < longitudeCount; j++)
            {
                longitudes[j] = longitudeMinimum + j * longitudeStep;
            }

            for (var i = 0; i < latitudeCount; i++)
            {
                latitudes[i] = latitudeMinimum + i * latitudeStep;
            }

            for (var t = 0; t < timestepCount; t++)
            {
                timesteps[t] = t;
            }

#if HAVE_MUCH_MEMORY
            for (var j = 0; j < longitudeCount; j++)
            {
                for (var i = 0; i < latitudeCount; i++)
                {
                    for (var t = 0; t < timestepCount; t++)
                    {
                        data3[j, i, t] = npp(j, i, t);
                    }
                }
            }
#else
            for (var j = 0; j < longitudeCount; j++)
            {
                for (var i = 0; i < latitudeCount; i++)
                {
                    data2[j, i] = npp(j, i, 0);
                }
            }
#endif

            var fullFilename = "msds:nc?file=" + filename + ".nc&openMode=create";
            var dataSet = DataSet.Open(fullFilename);
            dataSet.IsAutocommitEnabled = false;

            var longitudev = dataSet.AddVariable<double>(geographicalDimensions[0], longitudes, geographicalDimensions[0]);
            //longitudev.Metadata["units"] = units[0];

            var latitudev = dataSet.AddVariable<double>(geographicalDimensions[1], latitudes, geographicalDimensions[1]);
            //latitudev.Metadata["units"] = units[1];

            var timestepv = dataSet.AddVariable<int>(geographicalDimensions[2], timesteps, geographicalDimensions[2]);

#if HAVE_MUCH_MEMORY
            var datav = dataSet.AddVariable<double>("NPP", data3, geographicalDimensions);
            //datav.Metadata["units"] = "unitless";
#else
            var datav = dataSet.AddVariable<double>(dataName, geographicalDimensions);
            if (missingValue.Length > 0)
            {
                datav.Metadata["missing_value"] = missingValue;
            }
            //datav.Metadata["units"] = "unitless";

            for (var t = 0; t < timestepCount; t++)
            {
                for (var j = 0; j < longitudeCount; j++)
                {
                    for (var i = 0; i < latitudeCount; i++)
                    {
                        data2[j, i] = npp(j, i, t);
                    }
                }

                dataSet.PutData<double[,]>(datav.ID, data2, DataSet.FromToEnd(0), DataSet.FromToEnd(0), DataSet.ReduceDim(t));
                // Can't refer to it by name, or its name is not "NPP"... :-(
                //dataSet.Append<double[,]>("NPP", data2, DataSet.FromToEnd(0), DataSet.FromToEnd(0), DataSet.ReduceDim(t));
            }
#endif

            dataSet.Commit();
        }

        public static void CreateFiles(string root, bool terrestrial)
        {
            // 1 is land, 0 is sea
            CreateNetCdf2d<float, float, float>(
                Path.Combine(root, "LandSeaMask"),
                new string[] { "X", "Y" },
                new string[] { "degree_east", "degree_north" },
                "land_sea_mask",
                "unitless",
                "",
                (x) => x,
                (y) => y,
                (j, i) => terrestrial ? 1 : 0);

            /*
             * Values range from near 0 grams of carbon per square meter per day (tan) to 6.5 grams per square meter per day
             * A negative value means decomposition or respiration overpowered carbon absorption;
             * more carbon was released to the atmosphere than the plants took in
             */
            CreateNetCdf2d<double, double, double>(
                Path.Combine(root, "Land", "hanpp_2005"),
                new string[] { "Lons", "Lats" },
                new string[] { },
                "HANPP",
                "",
                "-9999",
                (x) => x,
                (y) => y,
                (j, i) => 0.0);

            CreateNetCdf2d(
                Path.Combine(root, "Land", "NPP"),
                new string[] { "long", "lat", "month" },
                "NPP",
                "99999",
                (j, i, t) => 0.0);

            CreateNetCdf2d<double, double, int>(
                Path.Combine(root, "Land", "AvailableWaterCapacity"),
                new string[] { "lon", "lat" },
                new string[] { },
                "AWC",
                "",
                "-9999",
                (x) => x,
                (y) => y,
                (j, i) => 500);

            CreateNetCdf2d(
                Path.Combine(root, "Ocean", "OceanNPP"),
                new string[] { "long", "lat", "month" },
                "NPP",
                "-9999",
                (j, i, t) => 0.0);

            CreateNetCdf2d(
                Path.Combine(root, "Ocean", "KennedyetalDTR_monthly_climatology"),
                new string[] { "longitude", "latitude", "month" },
                "DTR",
                "-9999",
                (j, i, t) => 0.0);

            CreateNetCdf2d(
                Path.Combine(root, "Ocean", "averaged_SST_50y_top100m_monthly"),
                new string[] { "lon", "lat", "month" },
                "SST",
                "-9999",
                (j, i, t) => 0.0);

            CreateNetCdf2d(
                Path.Combine(root, "Ocean", "averaged_u_50y_top100m_monthly"),
                new string[] { "lon", "lat", "month" },
                "uVel",
                "-9999",
                (j, i, t) => 0.0);

            CreateNetCdf2d(
                Path.Combine(root, "Ocean", "averaged_v_50y_top100m_monthly"),
                new string[] { "lon", "lat", "month" },
                "vVel",
                "-9999",
                (j, i, t) => 0.0);
        }

        public static IDictionary<string, string> RandomUnits(Random rnd, int length)
        {
            var rndPairsArray = Enumerable.Range(0, length).Select(i => Tuple.Create(Common.RandomString(rnd, 6), Common.RandomString(rnd, 8)));

            return rndPairsArray.ToDictionary(l => l.Item1, l => l.Item2);
        }

        public static Tuple<int, int>[] RandomFocusCells(Random rnd, int length)
        {
            return Enumerable.Range(0, length).Select(i => Tuple.Create(rnd.Next(), rnd.Next())).ToArray();
        }

        public static IDictionary<string, double[]> RandomCellEnv(Random rnd, int length)
        {
            var rndPairsArray = Enumerable.Range(0, length).Select(i => Tuple.Create(Common.RandomString(rnd, 6), Common.RandomDoubleArray(rnd, 7)));

            return rndPairsArray.ToDictionary(l => l.Item1, l => l.Item2);
        }

        public static IEnumerable<IDictionary<string, double[]>> RandomCellEnvironment(Random rnd, int length1, int length2)
        {
            return Enumerable.Range(0, length1).Select(i => RandomCellEnv(rnd, length2)).ToArray();
        }

        public static Madingley.Common.Environment RandomEnvironment(Random rnd)
        {
            var cellSize = rnd.NextDouble();
            var bottomLatitude = rnd.NextDouble();
            var topLatitude = rnd.NextDouble();
            var leftmostLongitude = rnd.NextDouble();
            var rightmostLongitude = rnd.NextDouble();
            var units = RandomUnits(rnd, 5);
            var specificLocations = Common.RandomBool(rnd);
            var focusCells = RandomFocusCells(rnd, 20);
            var cellEnvironment = RandomCellEnvironment(rnd, 5, 7);
            var fileNames = new string[] {
                //@"Model setup\SimulationControlParameters.csv",
                @"Model setup\FileLocationParameters.csv",
                @"Model setup\Ecological Definition Files\CohortFunctionalGroupDefinitions.csv",
                @"Model setup\Ecological Definition Files\StockFunctionalGroupDefinitions.csv",
                @"Model setup\Ecological Definition Files\EcologicalParameters.csv"
            };

            return new Madingley.Common.Environment(
                cellSize,
                bottomLatitude,
                topLatitude,
                leftmostLongitude,
                rightmostLongitude,
                units,
                specificLocations,
                focusCells,
                cellEnvironment,
                fileNames);
        }

        public static void CreateDirectories(string directory)
        {
            Common.DeleteDirectory(directory);

            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(Path.Combine(directory, "Land"));
            Directory.CreateDirectory(Path.Combine(directory, "Ocean"));
        }
    }
}
