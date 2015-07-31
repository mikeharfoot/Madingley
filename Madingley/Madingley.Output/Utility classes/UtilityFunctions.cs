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
                    Month = (uint)Math.Floor(currentTimestep / ((DaysInYear / MonthsInYear) / DaysInWeek)) % 12;
                    break;
                case "day":
                    Month = (uint)Math.Floor(currentTimestep / (DaysInYear / MonthsInYear)) % 12;
                    break;
                default:
                    Debug.Fail("Requested model time units not currently supported");
                    Month = 100;
                    break;

            }

            return Month;

        }

        public static string MakeYearlyFileName(string fileName, int year)
        {
            var cloneFileNameWithoutExtension = string.Format("{0}_{1}", System.IO.Path.GetFileNameWithoutExtension(fileName), year);
            var cloneFileName = System.IO.Path.ChangeExtension(cloneFileNameWithoutExtension, System.IO.Path.GetExtension(fileName));

            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileName), cloneFileName);
        }

        public static string CloneDataSet(Microsoft.Research.Science.Data.DataSet source, string fileName, int year)
        {
            var cloneFileName = MakeYearlyFileName(fileName, year);
            var targetUri = "msds:nc?file=" + cloneFileName + "&openMode=create";

            var dataSet = source.Clone("msds:memory");
            var dataSet2 = dataSet.Clone(targetUri);

            dataSet.Dispose();
            dataSet = null;

            dataSet2.Dispose();
            dataSet2 = null;

            return cloneFileName;
        }
    }
}
