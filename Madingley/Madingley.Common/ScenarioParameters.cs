using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Parameter for running a scenario
    /// </summary>
    public class ScenarioParameter
    {
        /// <summary>
        /// ParamString.
        /// </summary>
        public string ParamString { get; set; }

        /// <summary>
        /// ParamDouble1.
        /// </summary>
        public double ParamDouble1 { get; set; }

        /// <summary>
        /// ParamDouble2.
        /// </summary>
        public double ParamDouble2 { get; set; }

        /// <summary>
        /// ScenarioParameter default constructor
        /// </summary>
        public ScenarioParameter()
        {
            this.ParamString = "";
            this.ParamDouble1 = 0.0;
            this.ParamDouble2 = 0.0;
        }

        /// <summary>
        /// ScenarioParameter constructor
        /// </summary>
        /// <param name="paramString">ParamString.</param>
        /// <param name="paramDouble1">ParamDouble1.</param>
        /// <param name="paramDouble2">ParamDouble2.</param>
        public ScenarioParameter(
            string paramString,
            double paramDouble1,
            double paramDouble2)
        {
            this.ParamString = paramString;
            this.ParamDouble1 = paramDouble1;
            this.ParamDouble2 = paramDouble2;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scenarioParameters">ScenarioParameters to copy</param>
        public ScenarioParameter(ScenarioParameter scenarioParameter)
        {
            this.ParamString = scenarioParameter.ParamString;
            this.ParamDouble1 = scenarioParameter.ParamDouble1;
            this.ParamDouble2 = scenarioParameter.ParamDouble2;
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;Tuple&lt;string, double, double&gt;&gt; for comparing ScenarioParameters.
    /// </summary>
    public class ScenarioParameterComparer : IEqualityComparer<ScenarioParameter>
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
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if objects are both ScenarioParameters and equivalent; otherwise, false.</returns>
        public bool Equals(ScenarioParameter x, ScenarioParameter y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return
                x.ParamString.Equals(y.ParamString) &&
                DE.Equals(x.ParamDouble1, y.ParamDouble1) &&
                DE.Equals(x.ParamDouble2, y.ParamDouble2);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the current object.</returns>
        public int GetHashCode(ScenarioParameter obj)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(obj, null)) return 0;

            return
                obj.ParamString.GetHashCode() ^
                obj.ParamDouble1.GetHashCode() ^
                obj.ParamDouble2.GetHashCode();
        }
    }

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
        public IDictionary<string, ScenarioParameter> Parameters { get; set; }

        /// <summary>
        /// ScenarioParameters default constructor
        /// </summary>
        public ScenarioParameters()
        {
            this.Label = "";
            this.SimulationNumber = 0;
            this.Parameters = new SortedList<string, ScenarioParameter>();
        }

        /// <summary>
        /// ScenarioParameters constructor
        /// </summary>
        /// <param name="label">Label.</param>
        /// <param name="simulationNumber">Simulation number.</param>
        /// <param name="parameters">Parameters.</param>
        public ScenarioParameters(
            string label,
            int simulationNumber,
            IDictionary<string, ScenarioParameter> parameters)
        {
            this.Label = label;
            this.SimulationNumber = simulationNumber;
            this.Parameters = new SortedList<string, ScenarioParameter>(parameters);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scenarioParameters">ScenarioParameters to copy</param>
        public ScenarioParameters(ScenarioParameters scenarioParameters)
        {
            this.Label = scenarioParameters.Label;
            this.SimulationNumber = scenarioParameters.SimulationNumber;
            this.Parameters = new SortedList<string, ScenarioParameter>(scenarioParameters.Parameters);
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
                x.Parameters.SequenceEqual(y.Parameters, new KeyValuePairEqualityComparer<ScenarioParameter>(new ScenarioParameterComparer(this.DE)));
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
