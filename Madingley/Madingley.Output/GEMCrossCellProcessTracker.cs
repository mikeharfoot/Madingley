using System.Collections.Generic;

namespace Madingley.Output
{
    internal class GEMCrossCellProcessTracker : Madingley.Common.ICrossCellProcessTracker
    {
        private MadingleyModelOutput output;

        public GEMCrossCellProcessTracker(MadingleyModelOutput m)
        {
            this.output = m;
        }

        public void RecordDispersals(int timeStep, IList<Madingley.Common.GridCellDispersal> dispersalData, int numberOfDispersals)
        {
            this.output.model.RecordDispersals(dispersalData, (uint)numberOfDispersals);
        }
    }
}
