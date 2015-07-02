using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley.Output
{
    public static class Converters
    {
        public static Cohort ConvertCohort(Madingley.Common.Cohort c, byte functionalGroupIndex)
        {
            var cohortID = c.CohortID.Select(cs => (uint)cs).ToList();

            return new Cohort(
                (uint)c.BirthTimeStep,
                (uint)c.MaturityTimeStep,
                cohortID,
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

        public static Stock ConvertStock(Madingley.Common.Stock c, byte functionalGroupIndex)
        {
            return new Stock(
                functionalGroupIndex,
                c.IndividualBodyMass,
                c.TotalBiomass);
        }

        public static GridCellCohortHandler ConvertGridCellCohorts(IEnumerable<IEnumerable<Madingley.Common.Cohort>> cohorts)
        {
            return new GridCellCohortHandler(cohorts.Select((x, i) => x.Select(y => ConvertCohort(y, (byte)i)).ToList()).ToArray());
        }

        public static GridCellStockHandler ConvertGridCellStocks(IEnumerable<IEnumerable<Madingley.Common.Stock>> stocks)
        {
            return new GridCellStockHandler(stocks.Select((x, i) => x.Select(y => ConvertStock(y, (byte)i)).ToList()).ToArray());
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
            MadingleyModelInitialisation outputSettings,
            Madingley.Common.Configuration configuration,
            Madingley.Common.Environment e)
        {
            var i = new MadingleyModelInitialisation();

            i.GlobalModelTimeStepUnit = configuration.GlobalModelTimeStepUnit;
            i.NumTimeSteps = (uint)configuration.NumTimeSteps;
            i.CellSize = (float)e.CellSize;
            i.BottomLatitude = (float)e.BottomLatitude;
            i.TopLatitude = (float)e.TopLatitude;
            i.LeftmostLongitude = (float)e.LeftmostLongitude;
            i.RightmostLongitude = (float)e.RightmostLongitude;
            i.PlanktonDispersalThreshold = outputSettings.PlanktonDispersalThreshold;
            //i.InitialisationFileStrings = d.InitialisationFileStrings;
            i.InitialisationFileStrings = new SortedList<string, string>();
            i.SpecificLocations = e.SpecificLocations;
            i.InitialisationFileStrings.Add("OutputDetail", outputSettings.InitialisationFileStrings["OutputDetail"]);
            i.CohortFunctionalGroupDefinitions = ConvertFunctionalGroupDefinitions(configuration.CohortFunctionalGroupDefinitions);
            i.StockFunctionalGroupDefinitions = ConvertFunctionalGroupDefinitions(configuration.StockFunctionalGroupDefinitions);
            i.EnviroStack = e.CellEnvironment.Select(env => new SortedList<string, double[]>(env)).ToArray();
            i.CellList = e.FocusCells.Select(a => new UInt32[] { (UInt32)a.Item1, (UInt32)a.Item2 }).ToList();
            i.OutputPath = outputSettings.OutputPath;
            i.TrackProcesses = outputSettings.TrackProcesses;
            i.TrackCrossCellProcesses = outputSettings.TrackCrossCellProcesses;
            i.TrackGlobalProcesses = outputSettings.TrackGlobalProcesses;
            i.ProcessTrackingOutputs = outputSettings.ProcessTrackingOutputs;
            i.ModelMassBins = outputSettings.ModelMassBins;
            i.LiveOutputs = outputSettings.LiveOutputs;
            i.TrackMarineSpecifics = outputSettings.TrackMarineSpecifics;
            i.OutputMetrics = outputSettings.OutputMetrics;
            i.OutputStateTimestep = outputSettings.OutputStateTimestep;

            return i;
        }

        public static GridCell ConvertGridCell(Madingley.Common.GridCell c)
        {
            return new GridCell(
                ConvertGridCellCohorts(c.Cohorts),
                ConvertGridCellStocks(c.Stocks),
                new SortedList<string, double[]>(c.Environment),
                (float)c.Latitude,
                (float)c.Longitude);
        }

        public static GridCell[] ConvertGridCells(IEnumerable<Madingley.Common.GridCell> cs)
        {
            return cs.Select(c => ConvertGridCell(c)).ToArray();
        }

        private static void CopyDictionaryValue<T>(uint[,,] target, int ii, int jj, int kk, IDictionary<T, int> source, T key)
        {
            int value;

            if (source.TryGetValue(key, out value))
            {
                target[ii, jj, kk] = (uint)(value);
            }
        }

        public static void CopyCrossCellProcessTrackerData(
            CrossCellProcessTracker c,
            IList<Madingley.Common.RecordDispersalForACellData> dispersalData,
            uint timeStep,
            ModelGrid madingleyModelGrid)
        {
            var l0 = (int)madingleyModelGrid.NumLatCells;
            var l1 = (int)madingleyModelGrid.NumLonCells;

            var copyInboundCohorts = new uint[l0, l1, 8];

            for (var ii = 0; ii < l0; ii++)
            {
                for (var jj = 0; jj < l1; jj++)
                {
                    var cs = dispersalData[ii * l0 + jj].InboundCohorts;

                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 0, cs, Madingley.Common.CohortsEnterDirection.North);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 1, cs, Madingley.Common.CohortsEnterDirection.NorthEast);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 2, cs, Madingley.Common.CohortsEnterDirection.East);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 3, cs, Madingley.Common.CohortsEnterDirection.SouthEast);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 4, cs, Madingley.Common.CohortsEnterDirection.South);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 5, cs, Madingley.Common.CohortsEnterDirection.SouthWest);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 6, cs, Madingley.Common.CohortsEnterDirection.West);
                    CopyDictionaryValue<Madingley.Common.CohortsEnterDirection>(copyInboundCohorts, ii, jj, 7, cs, Madingley.Common.CohortsEnterDirection.NorthWest);
                }
            }

            var copyOutboundCohorts = new uint[l0, l1, 8];

            for (var ii = 0; ii < l0; ii++)
            {
                for (var jj = 0; jj < l1; jj++)
                {
                    var cs = dispersalData[ii * l0 + jj].OutboundCohorts;

                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 0, cs, Madingley.Common.CohortsExitDirection.North);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 1, cs, Madingley.Common.CohortsExitDirection.NorthEast);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 2, cs, Madingley.Common.CohortsExitDirection.East);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 3, cs, Madingley.Common.CohortsExitDirection.SouthEast);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 4, cs, Madingley.Common.CohortsExitDirection.South);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 5, cs, Madingley.Common.CohortsExitDirection.SouthWest);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 6, cs, Madingley.Common.CohortsExitDirection.West);
                    CopyDictionaryValue<Madingley.Common.CohortsExitDirection>(copyOutboundCohorts, ii, jj, 7, cs, Madingley.Common.CohortsExitDirection.NorthWest);
                }
            }

            var copyOutboundCohortWeights = new List<double>[l0, l1];

            for (var ii = 0; ii < l0; ii++)
            {
                for (var jj = 0; jj < l1; jj++)
                {
                    var index = ii * l0 + jj;

                    var cs = dispersalData[index].OutboundCohortWeights;

                    copyOutboundCohortWeights[ii, jj] = cs.ToList();
                }
            }

            c.RecordDispersalForACell(
                copyInboundCohorts,
                copyOutboundCohorts,
                copyOutboundCohortWeights,
                timeStep,
                madingleyModelGrid);

            c.TrackDispersal.Flush();
        }
    }
}
