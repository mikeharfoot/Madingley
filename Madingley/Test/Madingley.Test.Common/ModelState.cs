using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class ModelState
    {
        public static SortedList<string, double> RandomGlobalDiagnosticVariables(Random rnd)
        {
            var globalDiagnosticVariablesArray = new Tuple<string, double>[]
                {
                    Tuple.Create(Common.RandomString(rnd, 4), rnd.NextDouble()),
                    Tuple.Create(Common.RandomString(rnd, 5), rnd.NextDouble())
                };

            var globalDiagnosticVariablesDictionary = globalDiagnosticVariablesArray.ToDictionary(l => l.Item1, l => l.Item2);

            return new SortedList<string, double>(globalDiagnosticVariablesDictionary);
        }

        public static Madingley.Common.ModelState RandomModelState(Random rnd)
        {
            var timestepsComplete = rnd.Next();
            var globalDiagnosticVariables = RandomGlobalDiagnosticVariables(rnd);
            var gridCells = Enumerable.Range(0, 10).Select(i => GridCell.RandomGridCell(rnd)).ToArray();
            var nextCohortId = (Int64)(rnd.Next());

            return new Madingley.Common.ModelState(
                timestepsComplete,
                globalDiagnosticVariables,
                gridCells,
                nextCohortId);
        }
    }
}
