using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Madingley
{
    /// <summary>
    /// Generic functions
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// If longitudinal cell coordinates run from 0 to 360, the convert to -180 to 180 values
        /// </summary>
        /// <param name="lons">The longitudinal coorindates of the cells in the model grid</param>
        public static void ConvertToM180To180(double[] lons)
        {
            // Loop over longitudinal coordinates of the model grid cells
            for (int jj = 0; jj < lons.Length; jj++)
            {
                // If longitudinal coorindates exceed 180, then subtrarct 360 to correct the coorindates
                if (lons[jj] >= 180.0)
                {
                    lons[jj] -= 360.0;
                }
            }
            // Re-sort the longitudinal coordinates
            Array.Sort(lons);
        }

        /// <summary>
        /// Calculate the area of a grid cell in square km, given its dimensions and geographical position
        /// </summary>
        /// <param name="latitude">The latitude of the bottom-left corner of the grid cell</param>
        /// <param name="lonCellSize">The longitudinal dimension of the grid cell</param>
        /// <param name="latCellSize">The latitudinal dimension of the grid cell</param>
        /// <returns>The area in square km of the grid cell</returns>
        public static double CalculateGridCellArea(double latitude, double lonCellSize, double latCellSize)
        {
            // Convert from degrees to radians
            double latitudeRad = DegreesToRadians(latitude);

            // Equatorial radius in metres
            double EquatorialRadius = 6378137;

            // Polar radius in metres
            double PolarRadius = 6356752.3142;

            // Angular eccentricity
            double AngularEccentricity = Math.Acos(DegreesToRadians(PolarRadius / EquatorialRadius));

            // First eccentricity squared
            double ESquared = Math.Pow(Math.Sin(DegreesToRadians(AngularEccentricity)), 2);

            // Flattening
            double Flattening = 1 - Math.Cos(DegreesToRadians(AngularEccentricity));

            // Temporary value to save computations
            double TempVal = Math.Pow((EquatorialRadius * Math.Cos(latitudeRad)),2) + Math.Pow((PolarRadius * Math.Sin(latitudeRad)),2);

            // Meridional radius of curvature
            double MPhi = Math.Pow(EquatorialRadius * PolarRadius,2) / Math.Pow(TempVal,1.5);
            
            // Normal radius of curvature
            double NPhi = Math.Pow(EquatorialRadius,2) / Math.Sqrt(TempVal);

            // Length of latitude (km)
            double LatitudeLength = Math.PI / 180 * MPhi / 1000;

            // Length of longitude (km)
            double LongitudeLength = Math.PI / 180 * Math.Cos(latitudeRad) * NPhi / 1000;

            // Return the cell area in km^2
            return LatitudeLength * latCellSize * LongitudeLength * lonCellSize;
        }

        /// <summary>
        /// Convert from degrees to radians
        /// </summary>
        /// <param name="degrees">The value in degrees to convert</param>
        /// <returns>The value converted to radians</returns>
        public static double DegreesToRadians(double degrees)
        {
            return (degrees * Math.PI / 180.0);
        }

    }
}
