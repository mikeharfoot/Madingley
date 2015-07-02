using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class EcologicalParameters
    {
        public IDictionary<string, double> Parameters { get; set; }

        public IEnumerable<string> TimeUnits { get; set; }

        public EcologicalParameters(
            IDictionary<string, double> parameters,
            IEnumerable<string> timeUnits)
        {
            this.Parameters = new SortedList<string,double>(parameters);
            this.TimeUnits = timeUnits.OrderBy(m => m).ToArray();
        }

        public EcologicalParameters(EcologicalParameters e)
        {
            this.Parameters = new SortedList<string, double>(e.Parameters);
            this.TimeUnits = e.TimeUnits.ToArray();
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as EcologicalParameters;
            if ((Object)y == null) return false;

            return
                this.Parameters.SequenceEqual(y.Parameters, new KeyValuePairEqualityComparer<double>(new FixedDoubleComparer())) &
                this.TimeUnits.SequenceEqual(y.TimeUnits);
        }

        public override int GetHashCode()
        {
            return
                this.Parameters.GetHashCode() ^
                this.TimeUnits.GetHashCode();
        }
    }
}
