using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley
{
    /// <summary>
    /// Tracks ecological processes
    /// </summary>

    public class GlobalProcessTracker
    {
        public Boolean TrackProcesses { get; set; }

        public IEnumerable<Madingley.Common.IGlobalProcessTracker> globalProcessTrackers;

        public uint TimeStep { get; set; }

        public GlobalProcessTracker(IEnumerable<Madingley.Common.IGlobalProcessTracker> globalProcessTrackers)
        {
            this.TrackProcesses = globalProcessTrackers.Count() > 0;
            this.globalProcessTrackers = globalProcessTrackers.ToArray();
        }

        /// <summary>
        /// Record a flow of biomass to plants through net primary production
        /// </summary>
        /// <param name="latIndex">The latitudinal index of the current grid cell</param>
        /// <param name="lonIndex">The longitudinal index of the current grid cell</param>
        /// <param name="val">The NPP value</param>
        public void RecordNPP(uint latIndex, uint lonIndex, uint stock, double npp)
        {
            foreach (var o in this.globalProcessTrackers)
            {
                o.RecordNPP(latIndex, lonIndex, TimeStep, stock, npp);
            }
        }

        public void RecordHANPP(uint latIndex, uint lonIndex, uint stock, double npp)
        {
            foreach (var o in this.globalProcessTrackers)
            {
                o.RecordHANPP(latIndex, lonIndex, TimeStep, stock, npp);
            }
        }
    }
}
