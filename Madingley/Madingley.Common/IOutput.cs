using System;
using System.Collections.Generic;

namespace Madingley.Common
{
    public interface IOutput
    {
        IList<IEnumerable<IProcessTracker>> ProcessTracker { get; }

        IEnumerable<IGlobalProcessTracker> GlobalProcessTracker { get; }

        IEnumerable<ICrossCellProcessTracker> CrossCellProcessTracker { get; }

        void EndTimestep(int timestep, ModelState modelState);

        void SaveTimestep(int timestep);

        Object EndYear(int year);

        void EndRun();
    }
}
