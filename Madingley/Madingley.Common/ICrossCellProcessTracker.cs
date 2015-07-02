using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public enum CohortsExitDirection
    {
        North, // 0
        NorthEast, // 1
        East, // 2
        SouthEast, // 3
        South, // 4
        SouthWest, // 5
        West, // 6
        NorthWest, // 7
    }

    public enum CohortsEnterDirection
    {
        North, // 0
        NorthEast, // 1
        East, // 2
        SouthEast, // 3
        South, // 4
        SouthWest, // 5
        West, // 6
        NorthWest, // 7
    }

    /// <summary>
    /// Record dispersal events in the dispersal tracker
    /// </summary>
    /// <param name="inboundCohorts">The cohorts arriving in a cell in the current time step</param>
    /// <param name="outboundCohorts">The cohorts leaving a cell in the current time step</param>
    /// <param name="outboundCohortWeights">The body masses of cohorts leaving the cell in the current time step</param>
    public class RecordDispersalForACellData
    {
        public IDictionary<CohortsEnterDirection, int> InboundCohorts { get; set; }

        public IDictionary<CohortsExitDirection, int> OutboundCohorts { get; set; }

        public IEnumerable<double> OutboundCohortWeights { get; set; }

        public GridCell GridCell { get; set; }

        public RecordDispersalForACellData(
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

    public interface ICrossCellProcessTracker
    {
        void RecordDispersals(
            uint timeStep,
            IList<RecordDispersalForACellData> dispersalData,
            uint numberOfDispersals);
    }
}
