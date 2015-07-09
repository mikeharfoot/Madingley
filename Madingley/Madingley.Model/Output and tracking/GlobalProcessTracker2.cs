using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley
{
    /// <summary>
    /// Tracks ecological processes
    /// </summary>

    public class GlobalProcessTracker
    {
        public Boolean TrackProcesses { get; set; }

        public IEnumerable<Madingley.Common.IGlobalProcessTracker> GlobalProcessTrackers { get; set; }

        public uint TimeStep { get; set; }

        public GlobalProcessTracker(IEnumerable<Madingley.Common.IGlobalProcessTracker> globalProcessTrackers)
        {
            this.TrackProcesses = globalProcessTrackers.Count() > 0;
            this.GlobalProcessTrackers = globalProcessTrackers.ToArray();
        }

        /// <summary>
        /// Record a flow of biomass to plants through net primary production
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="stockFunctionalGroupIndex">Stock functional group</param>
        /// <param name="npp">The NPP value</param>
        public void RecordNPP(uint latitudeIndex, uint longitudeIndex, uint stockFunctionalGroupIndex, double npp)
        {
            foreach (var o in this.GlobalProcessTrackers)
            {
                o.RecordNPP((int)latitudeIndex, (int)longitudeIndex, (int)TimeStep, (int)stockFunctionalGroupIndex, npp);
            }
        }

        public void RecordHANPP(uint latitudeIndex, uint longitudeIndex, uint stockFunctionalGroupIndex, double npp)
        {
            foreach (var o in this.GlobalProcessTrackers)
            {
                o.RecordHANPP((int)latitudeIndex, (int)longitudeIndex, (int)TimeStep, (int)stockFunctionalGroupIndex, npp);
            }
        }
    }
}
