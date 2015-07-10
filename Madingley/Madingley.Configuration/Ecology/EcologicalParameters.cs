using System;
using System.Collections.Generic;
using System.IO;

namespace Madingley
{
    public static class EcologicalParameters
    {
        public static string[] TimeUnits = { "day", "month", "year" };

        public static Madingley.Common.EcologicalParameters Load(string parametersFile, string inputPath)
        {
            //Now read the parameter values into a dictionary
            var Parameters = new Dictionary<string, double>();
            StreamReader r_env = new StreamReader(System.IO.Path.Combine(inputPath, "Ecological Definition Files", parametersFile));

#if false
            //Copy the parameter values to the output directory
            System.IO.File.Copy("input/Model setup/Ecological definition files/" + parametersFile, outputPath + parametersFile, true);

            StreamReader r_env = new StreamReader("input/Model setup/Ecological definition files/" + parametersFile);
#endif

            string l;
            char[] comma = ",".ToCharArray();

            string[] f;

            l = r_env.ReadLine();
            while (!r_env.EndOfStream)
            {
                l = r_env.ReadLine();
                // Split fields by commas
                f = l.Split(comma);
                //First column is the parameter name
                //2nd column is the parameter value

                // Lists of the different fields
                Parameters.Add(f[0], Convert.ToDouble(f[1]));
            }

            r_env.Close();

            return new Madingley.Common.EcologicalParameters(Parameters, TimeUnits);
        }
    }
}
