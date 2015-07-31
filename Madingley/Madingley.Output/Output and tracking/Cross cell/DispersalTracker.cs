using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Microsoft.Research.Science.Data;

namespace Madingley
{
    /// <summary>
    /// Tracks results associated with cohort extinction
    /// </summary>
    public class DispersalTracker
    {
        private string FileName { get; set; }

        /// <summary>
        /// Constructor for the dispersal tracker: sets up output file
        /// </summary>
        /// <param name="dispersalFilename">The filename for the output file</param>
        /// <param name="outputPath">The path to the output directory</param>
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model simulation</param>
        public DispersalTracker(string dispersalFilename, string outputPath, string outputFilesSuffix)
        {
            // Initialise streamwriter to output properties and ids of extinct cohorts
            this.FileName = outputPath + dispersalFilename + outputFilesSuffix + ".txt";

            using (var DispersalWriter = new StreamWriter(this.FileName))
            {
                DispersalWriter.WriteLine("TimeStep\tCellrow\tCellCol\tLatitude\tLongitude\ttcohortsExitNorth\tcohortsExitNorthEast\tcohortsExitEast\tcohortsExitSouthEast\tcohortsExitSouth\tcohortsExitSouthWest\tcohortsExitWest\tcohortsExitNorthWest\tcohortsEnterNorth\tcohortsEnterNorthEast\tcohortsEnterEast\tcohortsEnterSouthEast\tcohortsEnterSouth\tcohortsEnterSouthWest\tcohortsEnterWest\tcohortsEnterNorthWest\tMeanDispersingCohortWeight\tMeanCohortWeight");
            }
        }

        /// <summary>
        /// Record dispersal events in the dispersal tracker
        /// </summary>
        /// <param name="inboundCohorts">The cohorts arriving in a grid cell in the current time step</param>
        /// <param name="outboundCohorts">The cohorts leaving a ce  ll in the current time step</param>
        /// <param name="outboundCohortWeights">The body masses of cohorts leaving the cell in the current time step</param>
        /// <param name="currentTimeStep">The current model time step</param>
        /// <param name="madingleyModelGrid">The model grid</param>
        public void RecordDispersal(uint[, ,] inboundCohorts, uint[, ,] outboundCohorts, List<double>[,] outboundCohortWeights, uint currentTimeStep, ModelGrid madingleyModelGrid)
        {
            using (var DispersalWriter = File.AppendText(this.FileName))
            {
                // Loop through cells in the grid and write out the necessary data
                for (uint ii = 0; ii < outboundCohorts.GetLength(0); ii++)
                {
                    for (uint jj = 0; jj < outboundCohorts.GetLength(1); jj++)
                    {
                        double MeanOutboundCohortWeight = new double();

                        // Calculate the mean weight of outbound cohorts (ignoring abundance)
                        if (outboundCohortWeights[ii, jj].Count == 0)
                        {
                            MeanOutboundCohortWeight = 0.0;
                        }
                        else
                        {
                            MeanOutboundCohortWeight = outboundCohortWeights[ii, jj].Average();
                        }

                        // Calculate the mean weight of all cohorts (ignoring abundance)
                        List<double> TempList = new List<double>();

                        GridCellCohortHandler Temp1 = madingleyModelGrid.GetGridCellCohorts(ii, jj);

                        // Loop through functional groups
                        for (int kk = 0; kk < Temp1.Count; kk++)
                        {
                            // Loop through cohorts
                            for (int hh = 0; hh < Temp1[kk].Count; hh++)
                            {
                                // Add the cohort weight to the list
                                TempList.Add(Temp1[kk][hh].IndividualBodyMass);
                            }
                        }

                        // Calculate the mean weight
                        double MeanCohortWeight = new double();
                        if (TempList.Count == 0)
                        {
                            MeanCohortWeight = 0.0;
                        }
                        else
                        {
                            MeanCohortWeight = TempList.Average();
                        }

                        string newline = Convert.ToString(currentTimeStep) + '\t' + Convert.ToString(ii) + '\t' +
                            Convert.ToString(jj) + '\t' + Convert.ToString(madingleyModelGrid.GetCellLatitude(ii)) + '\t' +
                            Convert.ToString(madingleyModelGrid.GetCellLongitude(jj)) + '\t' +
                           Convert.ToString(outboundCohorts[ii, jj, 0]) + '\t' + Convert.ToString(outboundCohorts[ii, jj, 1]) + '\t' +
                           Convert.ToString(outboundCohorts[ii, jj, 2]) + '\t' + Convert.ToString(outboundCohorts[ii, jj, 3]) + '\t' +
                           Convert.ToString(outboundCohorts[ii, jj, 4]) + '\t' + Convert.ToString(outboundCohorts[ii, jj, 5]) + '\t' +
                           Convert.ToString(outboundCohorts[ii, jj, 6]) + '\t' + Convert.ToString(outboundCohorts[ii, jj, 7]) + '\t' +
                           Convert.ToString(inboundCohorts[ii, jj, 0]) + '\t' + Convert.ToString(inboundCohorts[ii, jj, 1]) + '\t' +
                           Convert.ToString(inboundCohorts[ii, jj, 2]) + '\t' + Convert.ToString(inboundCohorts[ii, jj, 3]) + '\t' +
                           Convert.ToString(inboundCohorts[ii, jj, 4]) + '\t' + Convert.ToString(inboundCohorts[ii, jj, 5]) + '\t' +
                           Convert.ToString(inboundCohorts[ii, jj, 6]) + '\t' + Convert.ToString(inboundCohorts[ii, jj, 7]) + '\t' +
                           Convert.ToString(String.Format("{0:.000000}", MeanOutboundCohortWeight) + '\t' +
                           Convert.ToString(String.Format("{0:.000000}", MeanCohortWeight)));

                        DispersalWriter.WriteLine(newline);
                    }
                }
            }
        }

        public string Clone(int year)
        {
            var extension = string.Format("year_{0}.tsv", year);
            var copyFileName = System.IO.Path.ChangeExtension(this.FileName, extension);
            System.IO.File.Copy(this.FileName, copyFileName, true);

            return copyFileName;
        }

        public void Copy(DispersalTracker existing)
        {
            System.IO.File.Copy(existing.FileName, this.FileName, true);
        }
    }
}
