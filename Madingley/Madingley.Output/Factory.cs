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
            string OutputDir = "Parameters";
            OutputDir += System.DateTime.Now.Year + "-"
                + System.DateTime.Now.Month + "-"
                + System.DateTime.Now.Day + "_"
                + System.DateTime.Now.Hour + "."
                + System.DateTime.Now.Minute + "."
                + System.DateTime.Now.Second + "/";

            // Create the working directory if this does not already exist
            System.IO.Directory.CreateDirectory(OutputDir);

            foreach (var sourceFileName in configuration.FileNames)
            {
                var fileName = System.IO.Path.GetFileName(sourceFileName);
                var destFileName = System.IO.Path.Combine(OutputDir, fileName);

                System.IO.File.Copy(sourceFileName, destFileName, true);
            }

            foreach (var sourceFileName in environment.FileNames)
            {
                var fileName = System.IO.Path.GetFileName(sourceFileName);
                var destFileName = System.IO.Path.Combine(OutputDir, fileName);

                System.IO.File.Copy(sourceFileName, destFileName, true);
            }

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
