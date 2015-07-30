using System;
using System.Collections.Generic;

namespace Madingley.Common
{
    /// <summary>
    /// Output interface.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Return a list of IProcessTrackers, (possibly empty).
        /// </summary>
        IList<IEnumerable<IProcessTracker>> ProcessTracker { get; }

        /// <summary>
        /// Return a list of IGlobalProcessTrackers (possibly empty).
        /// </summary>
        IEnumerable<IGlobalProcessTracker> GlobalProcessTracker { get; }

        /// <summary>
        /// Return a list of ICrossCellProcessTrackers (possibly empty).
        /// </summary>
        IEnumerable<ICrossCellProcessTracker> CrossCellProcessTracker { get; }

        /// <summary>
        /// Called when a time step begins.
        /// </summary>
        /// <param name="timestep"></param>
        void BeginTimestep(int timestep);

        /// <summary>
        /// Called when a time step is complete, there may still be updates to CrossCellProcessTrackers.
        /// </summary>
        /// <param name="timestep"></param>
        /// <param name="modelState"></param>
        void EndTimestep(int timestep, ModelState modelState);

        /// <summary>
        /// Called after a time step is complete, and CrossCellProcessTrackers have been updated.
        /// </summary>
        /// <param name="timestep"></param>
        void SaveTimestep(int timestep);

        /// <summary>
        /// Called at the end of a year of simulation.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        Object EndYear(int year);

        /// <summary>
        /// Called at the end of the model run, clean up resources.
        /// </summary>
        void EndRun();
    }
}
