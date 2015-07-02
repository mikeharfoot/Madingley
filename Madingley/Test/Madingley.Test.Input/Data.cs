using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Research.Science.Data;

namespace Madingley.Test.Input
{
    //
    // Test types
    //   Cows - just a single cohort type, intended to be the simplest viable use of Madingley "Cows on tarmac"
    //   Terrestrial - all cohorts but only on land
    //   Marine - all cohorts but only in the sea
    //
    //  1 Cell will not do any dispersal and produce cell output
    //  4 Cells will do some dispersal and produce grid output
    //
    //  1 Year runs the test for 12 months
    //  10 Years runs the test for 12 * 10 months
    //
    public enum CommonTestType
    {
        COWS_NO_NPP_1_CELL_1_YEAR,
        COWS_1_CELL_1_YEAR,
        COWS_1_CELL_10_YEARS,
        COWS_4_CELLS_10_YEARS,
        TERRESTRIAL_4_CELLS_1_YEAR,
        TERRESTRIAL_4_CELLS_10_YEARS,
        MARINE_1_CELL_1_YEAR,
        MARINE_1_CELL_10_YEARS,
        MARINE_4_CELLS_10_YEARS
    };

    public class Data
    {
        public bool Terrestrial { get; private set; }

        public int RowCount { get; private set; }

        public string ExpectedDataRoot { get; private set; }

        public string PathToModelSetup { get; private set; }

        public int SpecificCellCount { get; private set; }

        public Madingley.Test.Common.LookupColumnNames ExpectedGlobalVariableNames { get; private set; }

        public Madingley.Test.Common.LookupColumnTest ExpectedGlobalLookupColumnTest { get; private set; }

        public Madingley.Test.Common.LookupColumnNames ExpectedCellVariableNames { get; private set; }

        public Madingley.Test.Common.LookupColumnTest[] ExpectedCellLookupColumnTests { get; private set; }

        public Madingley.Test.Common.LookupColumnNames ExpectedGridVariableNames { get; private set; }

        public Madingley.Test.Common.LookupColumnTest ExpectedGridLookupColumnTest { get; private set; }

        public Madingley.Test.Common.LookupColumn3dTest ExpectedGridLookupColumn3dTest { get; private set; }

        public Madingley.Test.Common.LookupColumnTest ExpectedGridLookupColumn3dTestFlattened { get; private set; }

        public Madingley.Test.Common.LookupColumnNames ExpectedDispersalVariableNames { get; private set; }

        public Madingley.Test.Common.LookupColumnTest ExpectedDispersalLookupColumnTest { get; private set; }

        public Madingley.Test.Common.LookupColumnNames ExpectedNPPVariableNames { get; private set; }

        public Madingley.Test.Common.LookupColumnTest ExpectedNPPLookupColumnTest { get; private set; }

        public Madingley.Test.Common.LookupColumn3dTest ExpectedNPPLookupColumn3dTest { get; private set; }

        public Madingley.Test.Common.LookupColumnTest ExpectedNPPLookupColumn3dTestFlattened { get; private set; }

        public Data(
            bool terrestrial,
            int rowCount,
            string expectedDataRoot,
            string pathToModelSetup,
            int specificCellCount,
            Madingley.Test.Common.LookupColumnNames expectedGlobalVariableNames,
            Madingley.Test.Common.LookupColumnTest expectedGlobalLookupData,
            Madingley.Test.Common.LookupColumnNames expectedCellVariableNames,
            Madingley.Test.Common.LookupColumnTest[] expectedCellLookupDatas,
            Madingley.Test.Common.LookupColumnNames expectedGridVariableNames,
            Madingley.Test.Common.LookupColumnTest expectedGridLookupData,
            Madingley.Test.Common.LookupColumn3dTest expectedGridLookupData3d,
            Madingley.Test.Common.LookupColumnTest expectedGridLookupData3dFlattened,
            Madingley.Test.Common.LookupColumnNames expectedDispersalVariableNames,
            Madingley.Test.Common.LookupColumnTest expectedDispersalLookupData,
            Madingley.Test.Common.LookupColumnNames expectedNPPVariableNames,
            Madingley.Test.Common.LookupColumnTest expectedNPPLookupData,
            Madingley.Test.Common.LookupColumn3dTest expectedNPPLookupData3d,
            Madingley.Test.Common.LookupColumnTest expectedNPPLookupData3dFlattened)
        {
            this.Terrestrial = terrestrial;
            this.RowCount = rowCount;
            this.ExpectedDataRoot = expectedDataRoot;
            this.PathToModelSetup = pathToModelSetup;
            this.SpecificCellCount = specificCellCount;
            this.ExpectedGlobalVariableNames = expectedGlobalVariableNames;
            this.ExpectedGlobalLookupColumnTest = expectedGlobalLookupData;
            this.ExpectedCellVariableNames = expectedCellVariableNames;
            this.ExpectedCellLookupColumnTests = expectedCellLookupDatas;
            this.ExpectedGridVariableNames = expectedGridVariableNames;
            this.ExpectedGridLookupColumnTest = expectedGridLookupData;
            this.ExpectedGridLookupColumn3dTest = expectedGridLookupData3d;
            this.ExpectedGridLookupColumn3dTestFlattened = expectedGridLookupData3dFlattened;
            this.ExpectedDispersalVariableNames = expectedDispersalVariableNames;
            this.ExpectedDispersalLookupColumnTest = expectedDispersalLookupData;
            this.ExpectedNPPVariableNames = expectedNPPVariableNames;
            this.ExpectedNPPLookupColumnTest = expectedNPPLookupData;
            this.ExpectedNPPLookupColumn3dTest = expectedNPPLookupData3d;
            this.ExpectedNPPLookupColumn3dTestFlattened = expectedNPPLookupData3dFlattened;
        }

        public static Madingley.Test.Common.LookupColumnTest LookupCrossCellProcessData(string filename, int count)
        {
            return (name) =>
            {
                var columnTest = Madingley.Test.Common.ColumnTest.LookupColumnTestFromTSVTruncated(filename, count).Invoke(name);

                switch (name)
                {
                    case "MeanDispersingCohortWeight":
                    case "MeanCohortWeight":
                    //return new Common.ColumnTest(columnTest.Expected, (IEqualityComparer<double>)(new Madingley.Common.FixedDoubleComparer()), columnTest.Include);

                    default:
                        return columnTest;
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest LookupDataFromNCGlobal(string filename, int count)
        {
            return (name) =>
            {
                var columnTest = Madingley.Test.Common.ColumnTest.LookupColumnTestFromDataSetTruncated(filename, count).Invoke(name);

                switch (name)
                {
                    case "Total living biomass":
                    //return new Common.ColumnTest(columnTest.Expected, (IEqualityComparer<double>)(new Madingley.Common.TolerantDoubleComparer(2L)), columnTest.Include);

                    default:
                        return columnTest;
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest LookupDataFromNCGridOutputs(string filename, int timestepCount)
        {
            return (name) =>
            {
                switch (name)
                {
                    case "Latitude":
                    case "Longitude":
                        return Madingley.Test.Common.ColumnTest.LookupColumnTestFromDataSetAll(filename).Invoke(name);

                    case "Time step":
                        return Madingley.Test.Common.ColumnTest.LookupColumnTestFromDataSetTruncated(filename, timestepCount).Invoke(name);

                    default:
                        return Madingley.Test.Common.ColumnTest.Empty;
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest Create1d(DataSet data, int timestepCount)
        {
            return (name) =>
            {
                var latitudeVar = Madingley.Test.Common.Common.FindVariableInDataSet(data, "Latitude");
                var latitudes = (float[])latitudeVar.GetData();
                var latitudeCount = latitudes.Length;

                var longitudeVar = Madingley.Test.Common.Common.FindVariableInDataSet(data, "Longitude");
                var longitudes = (float[])longitudeVar.GetData();
                var longitudeCount = longitudes.Length;

                var timestepVar = Madingley.Test.Common.Common.FindVariableInDataSet(data, "Time step");
                var timesteps = (float[])timestepVar.GetData();

                var count = latitudeCount * longitudeCount * timestepCount;

                Func<int, int> latitudeIndex = (i) => ((int)(i / longitudeCount)) % latitudeCount;
                Func<int, int> longitudeIndex = (i) => i % longitudeCount;
                Func<int, int> timestepIndex = (i) =>
                {
                    var r = latitudeCount * longitudeCount;
                    return (int)(i / r);
                };

                switch (name)
                {
                    case "Latitude":
                        {
                            var a = new double[count];

                            for (var ii = 0; ii < count; ii++)
                            {
                                a[ii] = (double)latitudes[latitudeIndex(ii)];
                            }

                            return new Common.ColumnTest(a, null, true);
                        }
                    case "Longitude":
                        {
                            var a = new double[count];

                            for (var ii = 0; ii < count; ii++)
                            {
                                a[ii] = (double)longitudes[longitudeIndex(ii)];
                            }

                            return new Common.ColumnTest(a, null, true);
                        }
                    case "Time step":
                        {
                            var a = new double[count];

                            for (var ii = 0; ii < count; ii++)
                            {
                                a[ii] = (double)timesteps[timestepIndex(ii)];
                            }

                            return new Common.ColumnTest(a, null, true);
                        }
                    default:
                        return Madingley.Test.Common.ColumnTest.Empty;
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest Create3d(string filename, int timestepCount)
        {
            return (name) =>
            {
                var columnTest = Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(filename, timestepCount).Invoke(name);

                if (columnTest.Include)
                {
                    var latitudeCount = columnTest.Expected.GetLength(0);
                    var longitudeCount = columnTest.Expected.GetLength(1);

                    var count = latitudeCount * longitudeCount * timestepCount;

                    Func<int, int> latitudeIndex = (i) => ((int)(i / longitudeCount)) % latitudeCount;
                    Func<int, int> longitudeIndex = (i) => i % longitudeCount;
                    Func<int, int> timestepIndex = (i) =>
                    {
                        var r = latitudeCount * longitudeCount;
                        return (int)(i / r);
                    };

                    var a = new double[count];

                    for (var ii = 0; ii < count; ii++)
                    {
                        a[ii] = columnTest.Expected[latitudeIndex(ii), longitudeIndex(ii), timestepIndex(ii)];
                    }

                    return new Common.ColumnTest(a, columnTest.Tolerance, columnTest.Include);
                }
                else
                {
                    return Madingley.Test.Common.ColumnTest.Empty;
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest LookupDataFromNCGridOutputs3d(string filename, int timestepCount)
        {
            return (name) =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);

                switch (name)
                {
                    case "Latitude":
                    case "Longitude":
                    case "Time step":
                        return Create1d(data, timestepCount).Invoke(name);

                    case "Rao Functional Evenness":
                    case "Biomass Richness":
                    case "Trophic Richness":
                    //var columnTest = Create3d(filename, timestepCount).Invoke(name);
                    //return new Common.ColumnTest(columnTest.Expected, (IEqualityComparer<double>)(new Madingley.Common.TolerantDoubleComparer(1L)), columnTest.Include);

                    default:
                        return Create3d(filename, timestepCount).Invoke(name);
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest LookupDataFromNCCell0(string filename, int count)
        {
            return (name) =>
            {
                // TODO! Include "Total Biomass density", "Heterotroph Abundance density", "Heterotroph Biomass density" in DataSet
                var columnTest = Madingley.Test.Common.ColumnTest.LookupColumnTestFromDataSetTruncated(filename, count).Invoke(name);

                switch (name)
                {
                    case "autotroph biomass density":
                    case "deciduous biomass density":
                    case "Rao Functional Evenness":
                    //return new Common.ColumnTest(columnTest.Expected, (IEqualityComparer<double>)(new Madingley.Common.TolerantDoubleComparer(2L)), columnTest.Include);

                    default:
                        return columnTest;
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest LookupDataFromNCNPPOutputs3d(string filename, int timestepCount)
        {
            return (name) =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);

                switch (name)
                {
                    case "Latitude":
                    case "Longitude":
                    case "Time step":
                        return Create1d(data, timestepCount).Invoke(name);

                    default:
                        return Create3d(filename, timestepCount).Invoke(name);
                }
            };
        }

        public static Madingley.Test.Common.LookupColumnTest[] LookupCell0(string filename, int timestepCount)
        {
            return new Madingley.Test.Common.LookupColumnTest[]
            {
                LookupDataFromNCCell0 (filename, timestepCount)
            };
        }

        //
        // Map from test to a set of paths to folders and configuration options
        //
        public static Data LookupCommonTestTypePaths(CommonTestType commonTestType, int? monthsComplete)
        {
            var shortRunLength = 1;
            var longRunLength = 10; // years

            Func<int, int> allRowCount = (int yearCount) => 12 * yearCount;

            Func<int, int> truncateRowCount = (int yearCount) =>
                {
                    var rowCount = allRowCount.Invoke(yearCount);

                    if (monthsComplete.HasValue)
                    {
                        return Math.Min(rowCount, monthsComplete.Value);
                    }
                    else
                    {
                        return rowCount;
                    }
                };

            Func<int, int> truncateDispersalCount = (int rowCount) =>
                {
                    if (monthsComplete.HasValue)
                    {
                        return Math.Min(rowCount, monthsComplete.Value * 4); // 4 cells
                    }
                    else
                    {
                        var longRunDispersalCount = 4 * 12 * longRunLength; // 4 cells, 12 months/year
                        return Math.Min(rowCount, longRunDispersalCount);
                    }
                };

            switch (commonTestType)
            {
                case CommonTestType.COWS_NO_NPP_1_CELL_1_YEAR:
                    {
                        var expectedDataRoot = @"Expected/Cows no NPP/1 Cell/1 Year";
                        var pathToModelSetup = @"Model setup/Cows no NPP/1 Cell/1 Year";
                        var rowCount = truncateRowCount.Invoke(shortRunLength);

                        return new Data(
                            true,
                            allRowCount.Invoke(shortRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            1,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromDataSetReverse(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc")),
                            Data.LookupCell0(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc"), (rowCount + 1)),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null);
                    }

                case CommonTestType.COWS_1_CELL_1_YEAR:
                    {
                        var expectedDataRoot = @"Expected/Cows/1 Cell/10 Years";
                        var pathToModelSetup = @"Model setup/Cows/1 Cell/1 Year";
                        var rowCount = truncateRowCount.Invoke(shortRunLength);

                        return new Data(
                            true,
                            allRowCount.Invoke(shortRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            1,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromDataSetReverse(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc")),
                            Data.LookupCell0(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc"), (rowCount + 1)),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null);
                    }

                case CommonTestType.COWS_1_CELL_10_YEARS:
                    {
                        var expectedDataRoot = @"Expected/Cows/1 Cell/10 Years";
                        var pathToModelSetup = @"Model setup/Cows/1 Cell/10 Years";
                        var rowCount = truncateRowCount.Invoke(longRunLength);

                        return new Data(
                            true,
                            allRowCount.Invoke(longRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            1,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromDataSetReverse(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc")),
                            Data.LookupCell0(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc"), (rowCount + 1)),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null);
                    }

                case CommonTestType.COWS_4_CELLS_10_YEARS:
                    {
                        var expectedDataRoot = @"Expected/Cows/4 Cells/10 Years";
                        var pathToModelSetup = @"Model setup/Cows/4 Cells/10 Years";
                        var rowCount = truncateRowCount.Invoke(longRunLength);
                        var dispersalRowCount = truncateDispersalCount.Invoke(467);

                        return new Data(
                            true,
                            allRowCount.Invoke(longRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            0,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            null,
                            null,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Data.LookupDataFromNCGridOutputs3d(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromTSV(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt")),
                            Data.LookupCrossCellProcessData(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt"), dispersalRowCount),
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "NPPOutput.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Data.LookupDataFromNCNPPOutputs3d(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount)
                        );
                    }

                case CommonTestType.TERRESTRIAL_4_CELLS_1_YEAR:
                    {
                        var expectedDataRoot = @"Expected/Terrestrial/4 Cells/10 Years";
                        var pathToModelSetup = @"Model setup/Terrestrial/4 Cells/1 Year";
                        var rowCount = truncateRowCount.Invoke(shortRunLength);

                        return new Data(
                            true,
                            allRowCount.Invoke(shortRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            0,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            null,
                            null,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Data.LookupDataFromNCGridOutputs3d(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromTSV(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt")),
                            Data.LookupCrossCellProcessData(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt"), 0),
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "NPPOutput.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Data.LookupDataFromNCNPPOutputs3d(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount)
                        );
                    }

                case CommonTestType.TERRESTRIAL_4_CELLS_10_YEARS:
                    {
                        var expectedDataRoot = @"Expected/Terrestrial/4 Cells/10 Years";
                        var pathToModelSetup = @"Model setup/Terrestrial/4 Cells/10 Years";
                        var rowCount = truncateRowCount.Invoke(longRunLength);
                        var dispersalRowCount = truncateDispersalCount.Invoke(448);

                        return new Data(
                            true,
                            allRowCount.Invoke(longRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            0,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            null,
                            null,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Data.LookupDataFromNCGridOutputs3d(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromTSV(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt")),
                            Data.LookupCrossCellProcessData(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt"), dispersalRowCount),
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "NPPOutput.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Data.LookupDataFromNCNPPOutputs3d(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount)
                        );
                    }

                case CommonTestType.MARINE_1_CELL_1_YEAR:
                    {
                        var expectedDataRoot = @"Expected/Marine/1 Cell/10 Years";
                        var pathToModelSetup = @"Model setup/Marine/1 Cell/1 Year";
                        var rowCount = truncateRowCount.Invoke(shortRunLength);

                        return new Data(
                            false,
                            allRowCount.Invoke(shortRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            1,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromDataSetReverse(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc")),
                            Data.LookupCell0(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc"), (rowCount + 1)),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null);
                    }

                case CommonTestType.MARINE_1_CELL_10_YEARS:
                    {
                        var expectedDataRoot = @"Expected/Marine/1 Cell/10 Years";
                        var pathToModelSetup = @"Model setup/Marine/1 Cell/10 Years";
                        var rowCount = truncateRowCount.Invoke(longRunLength);

                        return new Data(
                            false,
                            allRowCount.Invoke(longRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            1,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromDataSetReverse(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc")),
                            Data.LookupCell0(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Cell0.nc"), (rowCount + 1)),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null);
                    }

                case CommonTestType.MARINE_4_CELLS_10_YEARS:
                    {
                        var expectedDataRoot = @"Expected/Marine/4 Cells/10 Years";
                        var pathToModelSetup = @"Model setup/Marine/4 Cells/10 Years";
                        var rowCount = truncateRowCount.Invoke(longRunLength);
                        var dispersalRowCount = truncateDispersalCount.Invoke(441);

                        return new Data(
                            false,
                            allRowCount.Invoke(longRunLength),
                            expectedDataRoot,
                            pathToModelSetup,
                            0,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc")),
                            Data.LookupDataFromNCGlobal(Path.Combine(expectedDataRoot, "BasicOutputs_NI_0_Global.nc"), (rowCount + 1)),
                            null,
                            null,
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Data.LookupDataFromNCGridOutputs3d(Path.Combine(expectedDataRoot, "GridOutputs_NI_0.nc"), (rowCount + 1)),
                            Common.Common.LookupColumnNamesFromTSV(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt")),
                            Data.LookupCrossCellProcessData(Path.Combine(expectedDataRoot, "DispersalData_NI_0.txt"), dispersalRowCount),
                            Common.Common.LookupColumnNamesFromDataSet(Path.Combine(expectedDataRoot, "NPPOutput.nc")),
                            Data.LookupDataFromNCGridOutputs(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Madingley.Test.Common.Column3dTest.LookupColumn3dTestFromDataSetTruncated(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount),
                            Data.LookupDataFromNCNPPOutputs3d(Path.Combine(expectedDataRoot, "NPPOutput.nc"), rowCount)
                        );
                    }

                default:
                    throw new Exception("Unexpected test type");
            }
        }
    }
}
