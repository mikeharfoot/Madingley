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
        /// Get the month corresponding to the current time step
        /// </summary>
        /// <param name="currentTimestep">The current model time step</param>
        /// <param name="modelTimestepUnits">The time step units</param>
        /// <returns>The month corresponding to the current time step</returns>
        public static uint GetCurrentMonth(uint currentTimestep, string modelTimestepUnits)
        {
            uint Month;
            
            double DaysInYear = 360.0;
            double MonthsInYear = 12.0;
            double DaysInWeek = 7.0;

            switch (modelTimestepUnits.ToLower())
            {
                case "year":
                    Month = 0;
                    break;
                case "month":
                    Month = currentTimestep % 12;
                    break;
                case "week":
                    Month = (uint)Math.Floor(currentTimestep / ((DaysInYear/MonthsInYear)/DaysInWeek)) % 12;
                    break;
                case "day":
                    Month = (uint)Math.Floor(currentTimestep / (DaysInYear / MonthsInYear)) % 12;
                    break;
                default:
                    Debug.Fail("Requested model time units not currently supported");
                    Month =  100;
                    break;

            }

            return Month;

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
