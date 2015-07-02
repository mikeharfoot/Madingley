using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley.Output
{
    public static class Factory
    {
        public static string simulationInitialisationFile = "";
        public static string definitionsFilename = "FileLocationParameters.csv";
        public static string outputsFilename = "OutputControlParameters.csv";
        public static string outputPath = "";
        public static string inputPath = "";

        public static Madingley.Common.IOutput Create(
            Madingley.Common.RunState state,
            Madingley.Common.Configuration configuration,
            Madingley.Common.Environment environment,
            Madingley.Common.ModelState modelState)
        {
            // Specify the working directory
            //string OutputDir = "C:/Users/derekt/Dropbox/Madingley stuff/Model outputs/MadingleyOutputs" +
            //string OutputDir = "C:/Users/derekt/desktop/MadingleyOutputs" +
            //string OutputDir = "Ensemble" +
            string OutputDir = "GlobalStates" +
                //string OutputDir = "C:/Users/mikeha/Work/Research/Visual Studio 2010/Madingley/madingley outputs" +
                System.DateTime.Now.Year + "-"
                + System.DateTime.Now.Month + "-"
                + System.DateTime.Now.Day + "_"
                + System.DateTime.Now.Hour + "."
                + System.DateTime.Now.Minute + "."
                + System.DateTime.Now.Second + "/";

            // Create the working directory if this does not already exist
            System.IO.Directory.CreateDirectory(OutputDir);

            // Set up the suffix for the output files
            var OutputFilesSuffix = "_";

            // Add the scenario label to the suffix for the output files
            OutputFilesSuffix += configuration.ScenarioParameters[configuration.ScenarioIndex].Label + "_";

            // Add the simulation index number to the suffix
            OutputFilesSuffix += configuration.Simulation.ToString();

            var i = new MadingleyModelInitialisation(simulationInitialisationFile, definitionsFilename, outputsFilename, outputPath, inputPath);
            i.OutputPath = OutputDir;

            var output = new Madingley.Output.MadingleyModelOutput(
                OutputFilesSuffix,
                environment,
                configuration,
                i,
                modelState);

            if (state != null)
            {
                var existing = (Madingley.Output.MadingleyModelOutput)state.Output;
                output.model.Copy(existing.model);
            }

            return output;
        }
    }
}
