using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;


namespace Madingley
{
    /// <summary>
    /// Reads the file specifying which scenarios will be run, and stores this information
    /// </summary>
    public class ScenarioParameterInitialisation
    {
        /// <summary>
        /// The number of scenarios to be run
        /// </summary>
        private int _scenarioNumber;
        /// <summary>
        /// Get the number of scenarios to be run
        /// </summary>
        public int scenarioNumber
        { get { return _scenarioNumber; } }

        /// <summary>
        /// Parameters for all scenarios in the model run
        /// </summary>
        private List<Tuple<string,int,SortedList<string,Tuple<string,double, double>>>> _scenarioParameters;
        /// <summary>
        /// Get the parameters for all scenarios in the model run
        /// </summary>
        public List<Tuple<string,int, SortedList<string, Tuple<string, double, double>>>> scenarioParameters
        { get { return _scenarioParameters; } }
        
        public ScenarioParameterInitialisation(List<Tuple<string, int, SortedList<string, Tuple<string, double, double>>>> scenarioParameters)
        {
            this._scenarioParameters = scenarioParameters;
            this._scenarioNumber = scenarioParameters.Count;
        }

    }
}
