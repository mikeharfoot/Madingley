using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class ModelState
    {
        public int TimestepsComplete { get; set; }

        public IDictionary<string, double> GlobalDiagnosticVariables { get; set; }

        public IList<GridCell> GridCells { get; set; }

        public Int64 NextCohortID { get; set; }

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

        public ModelState(ModelState c)
        {
            this.TimestepsComplete = c.TimestepsComplete;
            this.GlobalDiagnosticVariables = new SortedList<string, double>(c.GlobalDiagnosticVariables);
            this.GridCells = c.GridCells.ToArray();
            this.NextCohortID = c.NextCohortID;
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as ModelState;
            if ((Object)y == null) return false;

            return
                this.TimestepsComplete.Equals(y.TimestepsComplete) &&
                this.GlobalDiagnosticVariables.SequenceEqual(y.GlobalDiagnosticVariables, new KeyValuePairEqualityComparer<double>(EqualityComparer<double>.Default)) &&
                this.GridCells.SequenceEqual(y.GridCells, EqualityComparer<GridCell>.Default) &&
                this.NextCohortID.Equals(y.NextCohortID);
        }

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
