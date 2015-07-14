using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley
{
    /// <summary>
    /// Tracks diagnostics about the ecological processes
    /// </summary>

    public class CrossCellProcessTracker
    {
        /// <summary>
        /// Whether to track cross-cell processes
        /// </summary>
        public Boolean TrackCrossCellProcesses { get; set; }

        /// <summary>
        /// Get and set the reproduction tracker
        /// </summary>
        public Madingley.Common.GridCellDispersal[] GridCellDispersals { get; set; }

        public CrossCellProcessTracker(Boolean trackCrossCellProcesses)
        {
            this.TrackCrossCellProcesses = trackCrossCellProcesses;
        }

        /// <summary>
        /// Record dispersal events in the dispersal tracker
        /// </summary>
        /// <param name="inboundCohorts">The cohorts arriving in a cell in the current time step</param>
        /// <param name="outboundCohorts">The cohorts leaving a cell in the current time step</param>
        /// <param name="outboundCohortWeights">The body masses of cohorts leaving the cell in the current time step</param>
        /// <param name="timestep">The current model time step</param>
        /// <param name="madingleyModelGrid">The model grid</param>
        public void RecordDispersalForACell(uint[, ,] inboundCohorts, uint[, ,] outboundCohorts, List<double>[,] outboundCohortWeights, uint timestep, ModelGrid madingleyModelGrid)
        {
            var count = inboundCohorts.GetLength(0) * inboundCohorts.GetLength(1);

            var copy = new Madingley.Common.GridCellDispersal[count];

            for (var kk = 0; kk < count; kk++)
            {
                var ii = (int)kk / inboundCohorts.GetLength(1);
                var jj = kk % inboundCohorts.GetLength(1);

                var denter =
                    new Tuple<Madingley.Common.CohortsEnterDirection, int>[]
                    {
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.North, (int)inboundCohorts[ii, jj, 0]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.NorthEast, (int)inboundCohorts[ii, jj, 1]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.East, (int)inboundCohorts[ii, jj, 2]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.SouthEast, (int)inboundCohorts[ii, jj, 3]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.South, (int)inboundCohorts[ii, jj, 4]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.SouthWest, (int)inboundCohorts[ii, jj, 5]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.West, (int)inboundCohorts[ii, jj, 6]),
                        Tuple.Create(Madingley.Common.CohortsEnterDirection.NorthWest, (int)inboundCohorts[ii, jj, 7]),
                    };

                var enter = denter.ToDictionary(l => l.Item1, l => l.Item2);

                var dexit =
                    new Tuple<Madingley.Common.CohortsExitDirection, int>[]
                    {
                        Tuple.Create(Madingley.Common.CohortsExitDirection.North, (int)outboundCohorts[ii, jj, 0]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.NorthEast, (int)outboundCohorts[ii, jj, 1]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.East, (int)outboundCohorts[ii, jj, 2]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.SouthEast, (int)outboundCohorts[ii, jj, 3]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.South, (int)outboundCohorts[ii, jj, 4]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.SouthWest, (int)outboundCohorts[ii, jj, 5]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.West, (int)outboundCohorts[ii, jj, 6]),
                        Tuple.Create(Madingley.Common.CohortsExitDirection.NorthWest, (int)outboundCohorts[ii, jj, 7]),
                    };

                var exit = dexit.ToDictionary(l => l.Item1, l => l.Item2);

                var weights = outboundCohortWeights[ii, jj].ToArray();

                var cell = madingleyModelGrid.GetGridCell((uint)ii, (uint)jj);

                var ccell = Converters.ConvertCellData(cell);

                copy[kk] = new Madingley.Common.GridCellDispersal(enter, exit, weights, ccell);
            }

            this.GridCellDispersals = copy;
        }
    }
}
