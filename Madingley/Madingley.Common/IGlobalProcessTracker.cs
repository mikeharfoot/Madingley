
namespace Madingley.Common
{
    /// <summary>
    /// Interface for tracking global processes.
    /// </summary>
    public interface IGlobalProcessTracker
    {
        /// <summary>
        /// Record net primary productivity for a grid cell at a time step.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="stockFunctionalGroupIndex">Stock functional group index.</param>
        /// <param name="npp">Net primary productivity.</param>
        void RecordNPP(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            int stockFunctionalGroupIndex,
            double npp);

        /// <summary>
        /// Record human appropriation of net primary productivity for a grid cell at a time step.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="stockFunctionalGroupIndex">Stock functional group index.</param>
        /// <param name="hanpp">Human appropriation of net primary productivity.</param>
        void RecordHANPP(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            int stockFunctionalGroupIndex,
            double hanpp);
    }
}
