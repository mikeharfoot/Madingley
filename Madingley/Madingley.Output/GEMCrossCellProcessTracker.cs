using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley.Output
{
    internal class GEMCrossCellProcessTracker : Madingley.Common.ICrossCellProcessTracker
    {
        private MadingleyModelOutput output;

        public GEMCrossCellProcessTracker(MadingleyModelOutput m)
        {
            this.output = m;
        }

        public void RecordDispersals(
            uint timeStep,
            IList<Madingley.Common.RecordDispersalForACellData> dispersalData,
            uint numberOfDispersals)
        {
            this.output.model.RecordDispersals(dispersalData, numberOfDispersals);
        }
    }
}
