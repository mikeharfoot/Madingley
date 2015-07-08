using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Parameters for running a scenario
    /// </summary>
    public class ScenarioParameters
    {
        /// <summary>
        /// Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Simulation number.
        /// </summary>
        public int SimulationNumber { get; set; }

        /// <summary>
        /// Set of parameters.
        /// </summary>
        public IDictionary<string, Tuple<string, double, double>> Parameters { get; set; }

        /// <summary>
        /// ScenarioParameters constructor
        /// </summary>
        /// <param name="label">Label.</param>
        /// <param name="simulationNumber">Simulation number.</param>
        /// <param name="parameters">Parameters.</param>
        public ScenarioParameters(
            string label,
            int simulationNumber,
            IDictionary<string, Tuple<string, double, double>> parameters)
        {
            this.Label = label;
            this.SimulationNumber = simulationNumber;
            this.Parameters = new SortedList<string, Tuple<string, double, double>>(parameters);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scenarioParameters">ScenarioParameters to copy</param>
        public ScenarioParameters(ScenarioParameters scenarioParameters)
        {
            this.Label = scenarioParameters.Label;
            this.SimulationNumber = scenarioParameters.SimulationNumber;
            this.Parameters = new SortedList<string, Tuple<string, double, double>>(scenarioParameters.Parameters);
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;Tuple&lt;string, double, double&gt;&gt; for comparing ScenarioParameters.
    /// </summary>
    public class ScenarioParameterComparer : IEqualityComparer<Tuple<string, double, double>>
    {
        /// <summary>
        /// Method for comparing doubles.
        /// </summary>
        IEqualityComparer<double> DE { get; set; }

        /// <summary>
        /// ScenarioParameterComparer constructor
        /// </summary>
        /// <param name="de">Method for comparing doubles.</param>
        public ScenarioParameterComparer(IEqualityComparer<double> de)
        {
            this.DE = de;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if objects are both ScenarioParameters and equivalent; otherwise, false.</returns>
        public bool Equals(Tuple<string, double, double> x, Tuple<string, double, double> y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return
                x.Item1.Equals(y.Item1) &&
                DE.Equals(x.Item2, y.Item2) &&
                DE.Equals(x.Item3, y.Item3);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the current object.</returns>
        public int GetHashCode(Tuple<string, double, double> obj)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(obj, null)) return 0;

            return
                obj.Item1.GetHashCode() ^
                obj.Item2.GetHashCode() ^
                obj.Item3.GetHashCode();
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;ScenarioParameters&gt; implementation
    /// </summary>
    public class ScenarioParametersComparer : IEqualityComparer<ScenarioParameters>
    {
        /// <summary>
        /// Method for comparing doubles
        /// </summary>
        IEqualityComparer<double> DE { get; set; }

        /// <summary>
        /// ScenarioParametersComparer constructor
        /// </summary>
        /// <param name="de"></param>
        public ScenarioParametersComparer(IEqualityComparer<double> de)
        {
            this.DE = de;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if objects are both ScenarioParameterss and equivalent; otherwise, false.</returns>
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

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(ScenarioParameters obj)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(obj, null)) return 0;

            return
                obj.Label.GetHashCode() ^
                obj.SimulationNumber.GetHashCode() ^
                obj.Parameters.GetHashCode();
        }
    }
}
