using System;
using System.Collections.Generic;
using System.IO;

namespace Madingley
{
    public static class EcologicalParameters
    {
        public static string[] TimeUnits = { "day", "month", "year" };

        public static Madingley.Common.EcologicalParameters Load(string fileName)
        {
            //Now read the parameter values into a dictionary
            var Parameters = new Dictionary<string, double>();

            using (var reader = new StreamReader(fileName))
            {
                // Discard the header
                var line = reader.ReadLine();
                var headers = line.Split(new char[] { ',' });

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    // Split fields by commas
                    var fields = line.Split(new char[] { ',' }, headers.Length);
                    //First column is the parameter name
                    //2nd column is the parameter value

                    // Lists of the different fields
                    Parameters.Add(fields[0], Convert.ToDouble(fields[1]));
                }
            }

            return new Madingley.Common.EcologicalParameters(Parameters, TimeUnits);
        }
    }
}
