
namespace Madingley.Output
{
    internal class GEMGlobalProcessTracker : Madingley.Common.IGlobalProcessTracker
    {
        private MadingleyModelOutput output;

        public GEMGlobalProcessTracker(MadingleyModelOutput m)
        {
            this.output = m;
        }

        public void RecordNPP(int latitudeIndex, int longitudeIndex, int timeStep, int stockFunctionalGroupIndex, double npp)
        {
            var p = this.GetGlobalProcessTracker();

            if (p.TrackProcesses)
            {
                p.RecordNPP((uint)latitudeIndex, (uint)longitudeIndex, (uint)stockFunctionalGroupIndex, npp);
            }
        }

        public void RecordHANPP(int latitudeIndex, int longitudeIndex, int timeStep, int stockFunctionalGroupIndex, double hanpp)
        {
            var p = this.GetGlobalProcessTracker();

            if (p.TrackProcesses)
            {
                p.RecordHANPP((uint)latitudeIndex, (uint)longitudeIndex, (uint)stockFunctionalGroupIndex, hanpp);
            }
        }

        public GlobalProcessTracker GetGlobalProcessTracker()
        {
            return this.output.model.GetGlobalProcessTracker();
        }
    }
}
