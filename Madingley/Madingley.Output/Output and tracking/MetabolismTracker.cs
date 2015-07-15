using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Madingley
{
    /// <summary>
    /// Tracks variables associated with cohort metabolism
    /// </summary>
    public class MetabolismTracker
    {
        /// <summary>
        /// File to write metabolism data to
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Set up the metabolism tracker
        /// </summary>
        /// <param name="metabolismFilename">Name of the metabolism tracker file to write to</param>
        /// <param name="outputPath">The path to the folder in which the metabolism tracker data file will be stored</param>
        /// <param name="outputFilesSuffix">A suffix for the filename in the case that there is more than one scenario</param>
        /// <param name="cellIndex">The index of the current cell within the list of all cells in this simulation</param>
        public MetabolismTracker(string metabolismFilename, string outputPath, string outputFilesSuffix, int cellIndex)
        {
            this.FileName = outputPath + metabolismFilename + outputFilesSuffix + "_Cell" + cellIndex + ".txt";

            using (var MetabolismWriter = new StreamWriter(this.FileName))
            {
                MetabolismWriter.WriteLine("Latitude\tLongitude\ttime_step\tCurrent_body_mass\tfunctional_group\tAmbient_temp\tMetabolic_mass_loss");
            }
        }

        /// <summary>
        /// Record the metabolic loss of individuals in a cohort
        /// </summary>
        /// <param name="latIndex">The latitudinal index of the current grid cell</param>
        /// <param name="lonIndex">The longitudinal index of the current grid cell</param>
        /// <param name="timeStep">The current time step</param>
        /// <param name="currentBodyMass">The current body mass of individuals in the cohort</param>
        /// <param name="functionalGroup">The index of the functional group that the cohort belongs to</param>
        /// <param name="temperature">The ambient temperature this cohort is experiencing</param>
        /// <param name="metabolicLoss">The metabolic loss of this cohort in this time step</param>
        public void RecordMetabolism(uint latIndex, uint lonIndex, uint timeStep, double currentBodyMass, int functionalGroup, double temperature, double metabolicLoss)
        {
            using (var MetabolismWriter = File.AppendText(this.FileName))
            {
                MetabolismWriter.WriteLine(Convert.ToString(latIndex) + "\t" +
                                           Convert.ToString(lonIndex) + "\t" +
                                           Convert.ToString(timeStep) + "\t" +
                                           Convert.ToString(currentBodyMass) + "\t" +
                                           Convert.ToString(functionalGroup) + "\t" +
                                           Convert.ToString(temperature) + "\t" +
                                           Convert.ToString(metabolicLoss));
            }
        }
    }
}
