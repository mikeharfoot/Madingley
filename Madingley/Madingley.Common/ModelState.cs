using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Full state of the model after each year. This is enough to restart the model.
    /// </summary>
    public class ModelState
    {
        /// <summary>
        /// Number of time steps complete.
        /// </summary>
        public int TimestepsComplete { get; set; }

        /// <summary>
        /// Set of global diagnostic variables.
        /// </summary>
        public IDictionary<string, double> GlobalDiagnosticVariables { get; set; }

        /// <summary>
        /// List of active grid cells.
        /// </summary>
        public IList<GridCell> GridCells { get; set; }

        /// <summary>
        /// Next ID to use for Cohort creation.
        /// </summary>
        public Int64 NextCohortID { get; set; }

        /// <summary>
        /// ModelState default constructor
        /// </summary>
        public ModelState()
        {
            this.TimestepsComplete = 0;
            this.GlobalDiagnosticVariables = new SortedList<string, double>();
            this.GridCells = new List<GridCell>();
            this.NextCohortID = 0;
        }

        /// <summary>
        /// ModelState constructor.
        /// </summary>
        /// <param name="timestepsComplete">Number of time steps complete.</param>
        /// <param name="globalDiagnosticVariables">Set of global diagnostic variables.</param>
        /// <param name="gridCells">List of active grid cells.</param>
        /// <param name="nextCohortID">Next ID to use for Cohort creation.</param>
        public ModelState(
            int timestepsComplete,
            IDictionary<string, double> globalDiagnosticVariables,
            IEnumerable<GridCell> gridCells,
            Int64 nextCohortID)
        {
            this.TimestepsComplete = timestepsComplete;
            this.GlobalDiagnosticVariables = new SortedList<string, double>(globalDiagnosticVariables);
            this.GridCells = gridCells.ToArray();
            this.NextCohortID = nextCohortID;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="modelState"></param>
        public ModelState(ModelState modelState)
        {
            this.TimestepsComplete = modelState.TimestepsComplete;
            this.GlobalDiagnosticVariables = new SortedList<string, double>(modelState.GlobalDiagnosticVariables);
            this.GridCells = modelState.GridCells.ToArray();
            this.NextCohortID = modelState.NextCohortID;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both ModelStates and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var modelStateObj = obj as ModelState;
            if ((Object)modelStateObj == null) return false;

            return
                this.TimestepsComplete.Equals(modelStateObj.TimestepsComplete) &&
                this.GlobalDiagnosticVariables.SequenceEqual(modelStateObj.GlobalDiagnosticVariables, new KeyValuePairEqualityComparer<double>(EqualityComparer<double>.Default)) &&
                this.GridCells.SequenceEqual(modelStateObj.GridCells, EqualityComparer<GridCell>.Default) &&
                this.NextCohortID.Equals(modelStateObj.NextCohortID);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.TimestepsComplete.GetHashCode() ^
                this.GlobalDiagnosticVariables.GetHashCode() ^
                this.GridCells.GetHashCode() ^
                this.NextCohortID.GetHashCode();
        }
    }
}
