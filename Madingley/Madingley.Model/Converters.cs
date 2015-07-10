using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley
{
    public static class Converters
    {
        public static Madingley.Common.Cohort ConvertCohortData(Cohort c)
        {
            return new Madingley.Common.Cohort(
                (int)c.BirthTimeStep,
                (int)c.MaturityTimeStep,
                c.CohortID.Select(cs => (int)cs).ToArray(),
                c.JuvenileMass,
                c.AdultMass,
                c.IndividualBodyMass,
                c.IndividualReproductivePotentialMass,
                c.MaximumAchievedBodyMass,
                c.CohortAbundance,
                c.Merged,
                c.ProportionTimeActive,
                c.TrophicIndex,
                c.LogOptimalPreyBodySizeRatio);
        }

        public static Cohort ConvertCohort(Madingley.Common.Cohort c, byte functionalGroupIndex)
        {
            return new Cohort(
                (uint)c.BirthTimeStep,
                (uint)c.MaturityTimeStep,
                 c.IDs.Select(cs => (uint)cs).ToList(),
                c.JuvenileMass,
                c.AdultMass,
                c.IndividualBodyMass,
                c.IndividualReproductivePotentialMass,
                c.MaximumAchievedBodyMass,
                c.Abundance,
                functionalGroupIndex,
                c.Merged,
                c.ProportionTimeActive,
                c.TrophicIndex,
                c.LogOptimalPreyBodySizeRatio);
        }

        public static Madingley.Common.Stock ConvertStockData(Stock c)
        {
            return new Madingley.Common.Stock(
                c.IndividualBodyMass,
                c.TotalBiomass);
        }

        public static Stock ConvertStock(Madingley.Common.Stock c, byte functionalGroupIndex)
        {
            return new Stock(
                functionalGroupIndex,
                c.IndividualBodyMass,
                c.TotalBiomass);
        }

        public static FunctionalGroupDefinitions ConvertFunctionalGroupDefinitions(Madingley.Common.FunctionalGroupDefinitions data)
        {
            // Initialise the lists
            var length = data.Data.Count();

            var IndexLookupFromTrait = new SortedDictionary<string, SortedDictionary<string, int[]>>();
            var FunctionalGroupProperties = new SortedList<string, double[]>();
            var TraitLookupFromIndex = new SortedDictionary<string, string[]>();
            var AllFunctionalGroupsIndex = new int[length];

            // Loop over columns in the functional group definitions file
            foreach (var traitName in data.Definitions)
            {
                var traitValues =
                    data.Data.Select(d => d.Definitions.First(kv => kv.Key == traitName).Value).ToArray();

                // Declare a sorted dictionary to hold the index values for each unique trait value
                SortedDictionary<string, int[]> TraitIndexValuesList = new SortedDictionary<string, int[]>();
                // Create a string array with the values of this trait
                for (int nn = 0; nn < length; nn++)
                {
                    // Add the functional group index to the list of all indices
                    AllFunctionalGroupsIndex[nn] = nn;
                }
                // Add the trait values to the trait-value lookup list
                TraitLookupFromIndex.Add(traitName, traitValues);

                // Get the unique values for this trait
                var DistinctValues = traitValues.Distinct().ToArray();
                //Loop over the unique values for this trait and list all the functional group indices with the value
                foreach (string DistinctTraitValue in DistinctValues.ToArray())
                {
                    List<int> FunctionalGroupIndex = new List<int>();
                    //Loop over the string array associated with this trait and add the index values of matching string to a list
                    for (int kk = 0; kk < traitValues.Length; kk++)
                    {
                        if (traitValues[kk].Equals(DistinctTraitValue))
                        {
                            FunctionalGroupIndex.Add(kk);
                        }
                    }
                    //Add the unique trait value and the functional group indices to the temporary list
                    TraitIndexValuesList.Add(DistinctTraitValue, FunctionalGroupIndex.ToArray());
                }
                // Add the unique trait values and corresponding functional group indices to the functional group index lookup
                IndexLookupFromTrait.Add(traitName, TraitIndexValuesList);
            }

            // For functional group properties
            foreach (var traitName in data.Properties)
            {
                var traitValues =
                    data.Data.Select(d => d.Properties.First(kv => kv.Key == traitName).Value).ToArray();

                // Get the values for this property
                double[] TempDouble = new double[length];
                for (int nn = 0; nn < length; nn++)
                {
                    TempDouble[nn] = Convert.ToDouble(traitValues.GetValue(nn));
                }
                // Add the values to the list of functional group properties
                FunctionalGroupProperties.Add(traitName, TempDouble);
            }

            return new FunctionalGroupDefinitions(
                IndexLookupFromTrait,
                FunctionalGroupProperties,
                TraitLookupFromIndex,
                AllFunctionalGroupsIndex);
        }

        public static MadingleyModelInitialisation ConvertInitialisation(
            Madingley.Common.ModelState m,
            Madingley.Common.Configuration d,
            Madingley.Common.Environment e)
        {
            var i = new MadingleyModelInitialisation("", "", "", "");

            i.GlobalModelTimeStepUnit = d.GlobalModelTimeStepUnit;
            i.NumTimeSteps = (uint)d.NumTimeSteps;
            i.BurninTimeSteps = (uint)d.BurninTimeSteps;
            i.ImpactTimeSteps = (uint)d.ImpactTimeSteps;
            i.RecoveryTimeSteps = (uint)d.RecoveryTimeSteps;
            i.CellSize = e.CellSize;
            i.BottomLatitude = (float)e.BottomLatitude;
            i.TopLatitude = (float)e.TopLatitude;
            i.LeftmostLongitude = (float)e.LeftmostLongitude;
            i.RightmostLongitude = (float)e.RightmostLongitude;
            i.RunCellsInParallel = d.RunCellsInParallel;
            i.RunSimulationsInParallel = d.RunSimulationsInParallel;
            i.RunRealm = d.RunRealm;
            i.DrawRandomly = d.DrawRandomly;
            i.ExtinctionThreshold = d.ExtinctionThreshold;
            i.MaxNumberOfCohorts = d.MaxNumberOfCohorts;
            i.DispersalOnly = d.DispersalOnly;
            i.PlanktonDispersalThreshold = d.PlanktonDispersalThreshold;
            i.SpecificLocations = e.SpecificLocations;
            i.InitialisationFileStrings = new SortedList<string, string>();
            i.InitialisationFileStrings["OutputDetail"] = "high";
            i.InitialisationFileStrings["DispersalOnlyType"] = d.DispersalOnlyType;
            i.CohortFunctionalGroupDefinitions = ConvertFunctionalGroupDefinitions(d.CohortFunctionalGroupDefinitions);
            i.StockFunctionalGroupDefinitions = ConvertFunctionalGroupDefinitions(d.StockFunctionalGroupDefinitions);
            if (m != null)
            {
                i.EnviroStack = ConvertEnvironment(m.GridCells);
            }
            else
            {
                i.EnviroStack = ConvertEnvironment(e.CellEnvironment);
            }
            i.CellList = e.FocusCells.Select(a => new UInt32[] { (uint)a.Item1, (uint)a.Item2 }).ToList();
            i.TrackProcesses = true;
            i.TrackCrossCellProcesses = true;
            i.TrackGlobalProcesses = true;
            i.Units = new SortedList<string, string>(e.Units);
            i.ImpactCellIndices = d.ImpactCellIndices.Select(ii => (uint)ii).ToList();
            i.ImpactAll = d.ImpactAll;
            if (m != null)
            {
                i.ModelStates = ConvertModelStates(m, d, e);
                i.InputState = true;
                i.InputGlobalDiagnosticVariables = new SortedList<string, double>(m.GlobalDiagnosticVariables);
            }
            else
            {
                i.ModelStates = null;
                i.InputState = false;
            }

            return i;
        }

        public static SortedList<string, double[]>[] ConvertEnvironment(IEnumerable<IDictionary<string, double[]>> d)
        {
            return d.Select(env => new SortedList<string, double[]>(env.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()))).ToArray();
        }

        public static SortedList<string, double[]>[] ConvertEnvironment(IEnumerable<Madingley.Common.GridCell> gridCells)
        {
            return gridCells.Select(cd => new SortedList<string, double[]>(cd.Environment.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()))).ToArray();
        }

        public static Madingley.Common.GridCell[] ConvertGrid(Func<uint, uint, GridCell> gridCells, List<uint[]> cellList)
        {
            var gridCellData = new Madingley.Common.GridCell[cellList.Count];

            for (var ii = 0; ii < cellList.Count; ii++)
            {
                var cell = gridCells(cellList[ii][0], cellList[ii][1]);

                gridCellData[ii] = ConvertCellData(cell);
            }

            return gridCellData;
        }

        public static Madingley.Common.GridCell ConvertCellData(GridCell c)
        {
            return new Madingley.Common.GridCell(
                c.Latitude,
                c.Longitude,
                c.GridCellCohorts.Select(x => x.Select(y => ConvertCohortData(y))),
                c.GridCellStocks.Select(x => x.Select(y => ConvertStockData(y))),
                c.CellEnvironment.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()));
        }

        public static Madingley.Common.ModelState ConvertModelStateData(
            uint timestepsComplete,
            SortedList<string, double> globalDiagnosticVariables,
            Func<uint, uint, GridCell> gridCells,
            List<uint[]> cellList,
            Int64 nextCohortID)
        {
            var gdvDictionary = globalDiagnosticVariables.ToDictionary(kv => kv.Key, kv => kv.Value);
            var gridCellDatas = Converters.ConvertGrid(gridCells, cellList);

            return new Madingley.Common.ModelState((int)timestepsComplete, gdvDictionary, gridCellDatas, nextCohortID);
        }

        public static List<InputModelState> ConvertModelStates(
            Madingley.Common.ModelState modelState,
            Madingley.Common.Configuration c,
            Madingley.Common.Environment e)
        {
            var numLatCells = (UInt32)((e.TopLatitude - e.BottomLatitude) / e.CellSize);
            var numLonCells = (UInt32)((e.RightmostLongitude - e.LeftmostLongitude) / e.CellSize);

            // Set up a grid of grid cells
            var gridCellCohorts = new GridCellCohortHandler[numLatCells, numLonCells];
            var gridCellStocks = new GridCellStockHandler[numLatCells, numLonCells];

            gridCellCohorts[0, 0] = new GridCellCohortHandler(c.CohortFunctionalGroupDefinitions.Data.Count());

            var cellList = e.FocusCells.ToArray();
            var gridCells = modelState.GridCells.ToArray();

            for (var ii = 0; ii < cellList.Count(); ii++)
            {
                var gridCell = gridCells[ii];

                gridCellCohorts[cellList[ii].Item1, cellList[ii].Item2] = ConvertCohorts(gridCell.Cohorts);
                gridCellStocks[cellList[ii].Item1, cellList[ii].Item2] = ConvertStocks(gridCell.Stocks);
            }

            var inputModelState = new InputModelState(gridCellCohorts, gridCellStocks);

            return new List<InputModelState>() { inputModelState };
        }

        public static GridCellCohortHandler ConvertCohorts(IEnumerable<IEnumerable<Madingley.Common.Cohort>> cohorts)
        {
            return new GridCellCohortHandler(cohorts.Select((x, i) => x.Select(y => ConvertCohort(y, (byte)i)).ToList()).ToArray());
        }

        public static GridCellStockHandler ConvertStocks(IEnumerable<IEnumerable<Madingley.Common.Stock>> stocks)
        {
            return new GridCellStockHandler(stocks.Select((x, i) => x.Select(y => ConvertStock(y, (byte)i)).ToList()).ToArray());
        }

        public static void ConvertEcologicalParameters(Madingley.Common.EcologicalParameters ecologicalParameters)
        {
            EcologicalParameters.Parameters = new Dictionary<string, double>(ecologicalParameters.Parameters);
            EcologicalParameters.TimeUnits = ecologicalParameters.TimeUnits.ToArray();
        }
    }
}
