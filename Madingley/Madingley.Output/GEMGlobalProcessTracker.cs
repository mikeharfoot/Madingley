using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley.Output
{
    internal class GEMGlobalProcessTracker : Madingley.Common.IGlobalProcessTracker
    {
        private MadingleyModelOutput output;

        public GEMGlobalProcessTracker(MadingleyModelOutput m)
        {
            this.output = m;
        }

        public void RecordNPP(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            uint stock,
            double npp)
        {
            var p = this.GetGlobalProcessTracker();

            if (p.TrackProcesses)
            {
                p.RecordNPP(
                    latIndex,
                    lonIndex,
                    stock,
                    npp);
            }
        }

        public void RecordHANPP(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            uint stock,
            double npp)
        {
            var p = this.GetGlobalProcessTracker();

            if (p.TrackProcesses)
            {
                p.RecordHANPP(
                    latIndex,
                    lonIndex,
                    stock,
                    npp);
            }
        }

        public GlobalProcessTracker GetGlobalProcessTracker()
        {
            return this.output.model.GetGlobalProcessTracker();
        }
    }
}
