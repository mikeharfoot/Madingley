
using Madingley.Output;
using Madingley.Test.Common;
using Madingley.Test.Input;

namespace Madingley.Test.Run
{
    public static class Common
    {
        public static void TestMadingleyModelOutputDataSets(CommonTestType commonTestType, int? monthsComplete, MadingleyModelOutputDataSets result)
        {
            var data = Data.LookupCommonTestTypePaths(commonTestType, monthsComplete);

            ColumnTest.TestDataSet("global", data.ExpectedGlobalLookupColumnTest, data.ExpectedGlobalVariableNames, result.Global);

            if (data.ExpectedCellVariableNames != null &&
                data.ExpectedCellLookupColumnTests != null)
            {
                ColumnTest.TestDataSetArray("cell", data.ExpectedCellLookupColumnTests, data.ExpectedCellVariableNames, result.Cells);
            }

            if (data.ExpectedGridVariableNames != null &&
                data.ExpectedGridLookupColumnTest != null &&
                data.ExpectedGridLookupColumn3dTest != null)
            {
                Column3dTest.TestDataSet3D("grid", data.ExpectedGridLookupColumn3dTest, data.ExpectedGridLookupColumnTest, data.ExpectedGridVariableNames, result.Grid);
            }

            if (data.ExpectedDispersalVariableNames != null &&
                data.ExpectedDispersalLookupColumnTest != null)
            {
                ColumnTest.TestDataSet("dispersal", data.ExpectedDispersalLookupColumnTest, data.ExpectedDispersalVariableNames, result.Dispersal);
            }

            if (data.ExpectedNPPVariableNames != null &&
                data.ExpectedNPPLookupColumnTest != null &&
                data.ExpectedNPPLookupColumn3dTest != null)
            {
                Column3dTest.TestDataSet3D("npp", data.ExpectedNPPLookupColumn3dTest, data.ExpectedNPPLookupColumnTest, data.ExpectedNPPVariableNames, result.NPP);
            }
        }
    }
}
