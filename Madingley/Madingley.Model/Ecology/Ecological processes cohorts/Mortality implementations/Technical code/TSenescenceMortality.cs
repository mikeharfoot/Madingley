using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Madingley
{
    /// <summary>
    /// A formulation of the process of senescence mortality
    /// </summary>
    public partial class SenescenceMortality : IMortalityImplementation
    {
        
        /// <summary>
        /// Scalar to convert from the time step units used by this mortality implementation to global model time step units
        /// </summary>
        private double _DeltaT;
        /// <summary>
        /// Get the scalar to convert from the time step units used by this mortality implementation to global model time step units
        /// </summary>
        public double DeltaT { get { return _DeltaT; } }

        /// <summary>
        /// Constructor for senscence mortality: assigns all parameter values
        /// </summary>
        public SenescenceMortality(string globalModelTimeStepUnit)
        {
            InitialiseParametersSenescenceMortality();

            // Calculate the scalar to convert from the time step units used by this implementation of mortality to the global model time step units
            _DeltaT = Utilities.ConvertTimeUnits(globalModelTimeStepUnit, _TimeUnitImplementation);
        }
    }
}
