using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley.Common
{
    public class ScenarioParameters
    {
        public string Label { get; set; }

        public int SimulationNumber { get; set; }

        public SortedList<string, Tuple<string, double, double>> Parameters { get; set; }

        public ScenarioParameters(
            string label,
            int simulationNumber,
            IDictionary<string, Tuple<string, double, double>> parameters)
        {
            this.Label = label;
            this.SimulationNumber = simulationNumber;
            this.Parameters = new SortedList<string, Tuple<string, double, double>>(parameters);
        }

        public ScenarioParameters(ScenarioParameters s)
        {
            this.Label = s.Label;
            this.SimulationNumber = s.SimulationNumber;
            this.Parameters = new SortedList<string, Tuple<string, double, double>>(s.Parameters);
        }
    }

    public class ScenarioParameterComparer : IEqualityComparer<Tuple<string, double, double>>
    {
        IEqualityComparer<double> de { get; set; }

        public ScenarioParameterComparer(IEqualityComparer<double> de)
        {
            this.de = de;
        }

        public bool Equals(Tuple<string, double, double> x, Tuple<string, double, double> y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return
                x.Item1.Equals(y.Item1) &&
                de.Equals(x.Item2, y.Item2) &&
                de.Equals(x.Item3, y.Item3);
        }

        public int GetHashCode(Tuple<string, double, double> x)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(x, null)) return 0;

            return
                x.Item1.GetHashCode() ^
                x.Item2.GetHashCode() ^
                x.Item3.GetHashCode();
        }
    }

    public class ScenarioParametersComparer : IEqualityComparer<ScenarioParameters>
    {
        IEqualityComparer<double> DE { get; set; }

        public ScenarioParametersComparer(IEqualityComparer<double> de)
        {
            this.DE = de;
        }

        public bool Equals(ScenarioParameters x, ScenarioParameters y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return
                x.Label.Equals(y.Label) &&
                x.SimulationNumber.Equals(y.SimulationNumber) &&
                x.Parameters.SequenceEqual(y.Parameters, new KeyValuePairEqualityComparer<Tuple<string, double, double>>(new ScenarioParameterComparer(this.DE)));
        }

        public int GetHashCode(ScenarioParameters x)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(x, null)) return 0;

            return
                x.Label.GetHashCode() ^
                x.SimulationNumber.GetHashCode() ^
                x.Parameters.GetHashCode();
        }
    }
}
