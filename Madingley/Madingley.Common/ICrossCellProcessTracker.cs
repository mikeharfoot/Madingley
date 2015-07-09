using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// For dispersal, which direction did cohort exit grid cell?
    /// </summary>
    public enum CohortsExitDirection
    {
        /// <summary>
        /// To the North
        /// </summary>
        North, // 0
        /// <summary>
        /// To the North East
        /// </summary>
        NorthEast, // 1
        /// <summary>
        /// To the East
        /// </summary>
        East, // 2
        /// <summary>
        /// To the South East
        /// </summary>
        SouthEast, // 3
        /// <summary>
        /// To the South
        /// </summary>
        South, // 4
        /// <summary>
        /// To the South West
        /// </summary>
        SouthWest, // 5
        /// <summary>
        /// To the West
        /// </summary>
        West, // 6
        /// <summary>
        /// To the North West
        /// </summary>
        NorthWest, // 7
    }

    /// <summary>
    /// For dispersal, from which direction did the cohort enter the grid cell?
    /// </summary>
    public enum CohortsEnterDirection
    {
        /// <summary>
        /// From the North
        /// </summary>
        North, // 0
        /// <summary>
        /// From the North East
        /// </summary>
        NorthEast, // 1
        /// <summary>
        /// From the East
        /// </summary>
        East, // 2
        /// <summary>
        /// From the South East
        /// </summary>
        SouthEast, // 3
        /// <summary>
        /// From the South
        /// </summary>
        South, // 4
        /// <summary>
        /// From the South West
        /// </summary>
        SouthWest, // 5
        /// <summary>
        /// From the West
        /// </summary>
        West, // 6
        /// <summary>
        /// From the North West
        /// </summary>
        NorthWest, // 7
    }

    /// <summary>
    /// Dispersal data for a grid cell.
    /// </summary>
    public class GridCellDispersal
    {
        /// <summary>
        /// Number of cohorts to enter from each direction.
        /// </summary>
        public IDictionary<CohortsEnterDirection, int> InboundCohorts { get; private set; }

        /// <summary>
        /// Number of cohorts to exit to each direction.
        /// </summary>
        public IDictionary<CohortsExitDirection, int> OutboundCohorts { get; private set; }

        /// <summary>
        /// List of cohort weights, list by cohort functional group.
        /// </summary>
        public IEnumerable<double> OutboundCohortWeights { get; private set; }

        /// <summary>
        /// Grid cell.
        /// </summary>
        public GridCell GridCell { get; private set; }

        /// <summary>
        /// GridCellDispersal constructor.
        /// </summary>
        /// <param name="inboundCohorts">Inbound cohort counts, by direction.</param>
        /// <param name="outboundCohorts">Outbound cohort counts, by direction.</param>
        /// <param name="outboundCohortWeights">Outbound cohort weights, by functional group.</param>
        /// <param name="gridCell">Grid cell.</param>
        public GridCellDispersal(
            IDictionary<CohortsEnterDirection, int> inboundCohorts,
            IDictionary<CohortsExitDirection, int> outboundCohorts,
            IEnumerable<double> outboundCohortWeights,
            GridCell gridCell)
        {
            this.InboundCohorts = new Dictionary<CohortsEnterDirection, int>(inboundCohorts);
            this.OutboundCohorts = new Dictionary<CohortsExitDirection, int>(outboundCohorts);
            this.OutboundCohortWeights = outboundCohortWeights.ToArray();
            this.GridCell = gridCell;
        }
    }

    /// <summary>
    /// Interface for tracking cross cell processes.
    /// </summary>
    public interface ICrossCellProcessTracker
    {
        /// <summary>
        /// Record dispersals for all active grid cells at a time step.
        /// </summary>
        /// <param name="timeStep">Time step.</param>
        /// <param name="gridCellDispersals">List of grid cell dispersal, one for each active grid cell.</param>
        /// <param name="numberOfDispersals">Number of dispersals.</param>
        void RecordDispersals(
            int timeStep,
            IList<GridCellDispersal> gridCellDispersals,
            int numberOfDispersals);
    }
}
