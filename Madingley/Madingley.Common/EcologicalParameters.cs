using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Sets of parameters to use for Ecological processes.
    /// </summary>
    public class EcologicalParameters
    {
        /// <summary>
        /// Set of parameters
        /// </summary>
        public IDictionary<string, double> Parameters { get; set; }

        /// <summary>
        /// List of valid time step units
        /// </summary>
        public IEnumerable<string> TimeUnits { get; set; }

        /// <summary>
        /// EcologicalParameters constructor
        /// </summary>
        /// <param name="parameters">Set of parameters</param>
        /// <param name="timeUnits">List of valid time step units</param>
        public EcologicalParameters(
            IDictionary<string, double> parameters,
            IEnumerable<string> timeUnits)
        {
            this.Parameters = new SortedList<string,double>(parameters);
            this.TimeUnits = timeUnits.OrderBy(m => m).ToArray();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="e">EcologicalParameters to copy</param>
        public EcologicalParameters(EcologicalParameters e)
        {
            this.Parameters = new SortedList<string, double>(e.Parameters);
            this.TimeUnits = e.TimeUnits.ToArray();
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both EcologicalParameters and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var y = obj as EcologicalParameters;
            if ((Object)y == null) return false;

            return
                this.Parameters.SequenceEqual(y.Parameters, new KeyValuePairEqualityComparer<double>(new FixedDoubleComparer())) &
                this.TimeUnits.SequenceEqual(y.TimeUnits);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.Parameters.GetHashCode() ^
                this.TimeUnits.GetHashCode();
        }
    }
}
