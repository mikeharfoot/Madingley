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
        /// Calculates factors to convert between different time units
        /// </summary>
        /// <param name="fromUnit">Time unit to convert from</param>
        /// <param name="toUnit">Time unit to convert to</param>
        /// <returns>Factor to convert between time units</returns>
        public static double ConvertTimeUnits(string fromUnit, string toUnit)
        {
            // Variable to hold the conversion factor
            double ConversionValue;
            double DaysInYear = 360.0;
            double MonthsInYear = 12.0;
            double DaysInWeek = 7.0;

            // Determine which combination of time units is being requested and return the appropriate scaling factor
            switch (fromUnit.ToLower())
            {
                case "year":
                    switch (toUnit.ToLower())
                    {
                        case "year":
                            ConversionValue = 1.0;
                            break;
                        case "month":
                            ConversionValue = MonthsInYear;
                            break;
                        case "bimonth":
                            ConversionValue = MonthsInYear * 2;
                            break;
                        case "week":
                            ConversionValue = DaysInYear / DaysInWeek;
                            break;
                        case "day":
                            ConversionValue = DaysInYear;
                            break;
                        default:
                            Debug.Fail("Requested combination of time units not currently supported");
                            ConversionValue = 0;
                            break;
                    }
                    break;
                case "month":
                    switch (toUnit.ToLower())
                    {
                        case "year":
                            ConversionValue = 1.0 / MonthsInYear;
                            break;
                        case "month":
                            ConversionValue = 1.0;
                            break;
                        case "bimonth":
                            ConversionValue = 2.0;
                            break;
                        case "week":
                            ConversionValue = (DaysInYear / MonthsInYear) / DaysInWeek;
                            break;
                        case "day":
                            ConversionValue = (DaysInYear / MonthsInYear);
                            break;
                        case "second":
                            ConversionValue = (DaysInYear / MonthsInYear) * 24.0 * 60.0 * 60.0;
                            break;
                        default:
                            Debug.Fail("Requested combination of time units not currently supported");
                            ConversionValue = 0;
                            break;
                    }
                    break;
                case "bimonth":
                    switch (toUnit.ToLower())
                    {
                        case "year":
                            ConversionValue = 1.0 / (MonthsInYear * 2);
                            break;
                        case "month":
                            ConversionValue = 1 / 2.0;
                            break;
                        case "bimonth":
                            ConversionValue = 1.0;
                            break;
                        case "week":
                            ConversionValue = (DaysInYear / (MonthsInYear * 2)) / DaysInWeek;
                            break;
                        case "day":
                            ConversionValue = (DaysInYear / (MonthsInYear * 2));
                            break;
                        case "second":
                            ConversionValue = (DaysInYear / (MonthsInYear * 2)) * 24.0 * 60.0 * 60.0;
                            break;
                        default:
                            Debug.Fail("Requested combination of time units not currently supported");
                            ConversionValue = 0;
                            break;
                    }
                    break;

                case "week":
                    switch (toUnit.ToLower())
                    {
                        case "year":
                            ConversionValue = DaysInWeek / DaysInYear;
                            break;
                        case "month":
                            ConversionValue = DaysInWeek / (DaysInYear / MonthsInYear);
                            break;
                        case "bimonth":
                            ConversionValue = DaysInWeek / (DaysInYear / (MonthsInYear * 2));
                            break;
                        case "week":
                            ConversionValue = 1.0;
                            break;
                        case "day":
                            ConversionValue = DaysInWeek;
                            break;
                        case "second":
                            ConversionValue = DaysInWeek * 24.0 * 60.0 * 60.0;
                            break;
                        default:
                            Debug.Fail("Requested combination of time units not currently supported");
                            ConversionValue = 0;
                            break;
                    }
                    break;
                case "day":
                    switch (toUnit.ToLower())
                    {
                        case "year":
                            ConversionValue = 1.0 / DaysInYear;
                            break;
                        case "month":
                            ConversionValue = 1.0 / (DaysInYear / MonthsInYear);
                            break;
                        case "bimonth":
                            ConversionValue = 1.0 / (DaysInYear / (MonthsInYear * 2));
                            break;
                        case "week":
                            ConversionValue = 1.0 / DaysInWeek;
                            break;
                        case "day":
                            ConversionValue = 1.0;
                            break;
                        default:
                            Debug.Fail("Requested combination of time units not currently supported");
                            ConversionValue = 0;
                            break;
                    }
                    break;
                default:
                    Debug.Fail("Requested combination of time units not currently supported");
                    ConversionValue = 0;
                    break;
            }

            // Return the conversion factor
            return ConversionValue;
        }
    }
}
