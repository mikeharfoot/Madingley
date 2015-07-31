using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Research.Science.Data;
using NUnit.Framework;

namespace Madingley.Test.Common
{
    public delegate Column3dTest LookupColumn3dTest(string name);

    public class Column3dTest
    {
        public enum TestDataSetType
        {
            NetCDF,
            TSV
        }

        public static Column3dTest Empty = new Column3dTest(new double[,,] { }, null, false);

        public double[, ,] Expected { get; set; }

        public IEqualityComparer<double> Tolerance { get; set; }

        public bool Include { get; set; }

        public Column3dTest(
            double[, ,] expected,
            IEqualityComparer<double> tolerance,
            bool include)
        {
            this.Expected = expected;
            this.Tolerance = tolerance;
            this.Include = include;
        }

        public static void DoColumn3dTest(Column3dTest column3dTest, double[, ,] actual)
        {
#if DEBUG_COMPARISON
            Func<double, double, bool> test = column3dTest.Tolerance != null ?
                (Func<double, double, bool>)((a, e) => column3dTest.Tolerance.Equals(a, e)) :
                (Func<double, double, bool>)((a, e) => a.Equals(e));

            var count1 = actual.GetLength(0);
            var count2 = actual.GetLength(1);
            var count3 = actual.GetLength(2);

            var tests = new Tuple<double, double, bool>[count1, count2, count3];

            for (var ii = 0; ii < count1; ii++)
            {
                for (var jj = 0; jj < count2; jj++)
                {
                    for (var kk = 0; kk < count3; kk++)
                    {
                        var a = actual[ii, jj, kk];
                        var e = column3dTest.Expected[ii, jj, kk];

                        tests[ii, jj, kk] = Tuple.Create(a, e, test(a, e));
                    }
                }
            }

            foreach (var t in tests)
            {
                Assert.IsTrue(t.Item3);
            }
#else
            if (column3dTest.Tolerance != null)
            {
                var count1 = actual.GetLength(0);
                var count2 = actual.GetLength(1);
                var count3 = actual.GetLength(2);

                for (var ii = 0; ii < count1; ii++)
                {
                    for (var jj = 0; jj < count2; jj++)
                    {
                        for (var kk = 0; kk < count3; kk++)
                        {
                            Assert.IsTrue(column3dTest.Tolerance.Equals(actual[ii, jj, kk], column3dTest.Expected[ii, jj, kk]));
                        }
                    }
                }
            }
            else
            {
                Assert.That(actual, Is.EquivalentTo(column3dTest.Expected));
            }
#endif
        }

        public static void TestDataSet3DVariable(string tableName, LookupColumn3dTest lookupColumn3dTest, LookupColumnTest lookupColumnTest, Variable v)
        {
            var columnTest = lookupColumnTest.Invoke(v.Name);
            if (columnTest.Include)
            {
                ColumnTest.TestDataSetVariable(tableName, lookupColumnTest, v);
            }
            else
            {
                var column3dTest = lookupColumn3dTest.Invoke(v.Name);
                if (column3dTest.Include)
                {
                    var actualTruncated = (double[, ,])null;

                    switch (v.TypeOfData.Name.ToString().ToLower())
                    {
                        case "double":
                            {
                                var actual = (double[, ,])v.GetData();
                                var count1 = column3dTest.Expected.GetLength(0);
                                var count2 = column3dTest.Expected.GetLength(1);
                                var count3 = column3dTest.Expected.GetLength(2);

                                actualTruncated = new double[count1, count2, count3];

                                for (var ii = 0; ii < count1; ii++)
                                {
                                    for (var jj = 0; jj < count2; jj++)
                                    {
                                        for (var kk = 0; kk < count3; kk++)
                                        {
                                            actualTruncated[ii, jj, kk] = actual[ii, jj, kk];
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            throw new Exception("unexpected column type");
                    }

                    DoColumn3dTest(column3dTest, actualTruncated);
                }
                else
                {
                    Console.WriteLine("Skipping : {0}", v.Name);
                }
            }
        }

        public static void TestDataSet3D(string tableName, LookupColumn3dTest lookupColumn3dTest, LookupColumnTest lookupColumnTest, LookupColumnNames variableNames, string filename, TestDataSetType type)
        {
            var uri = (string)null;

            switch (type)
            {
                case TestDataSetType.NetCDF:
                    uri = "msds:nc?file=" + filename + "&openMode=readOnly";
                    break;

                case TestDataSetType.TSV:
                    uri = "msds:csv?file=" + filename + "&openMode=readOnly&separator=tab";
                    break;

                default:
                    throw new Exception("Unexpected test type");
            }

            var dataSet = Microsoft.Research.Science.Data.DataSet.Open(uri);

            var vs = dataSet.Variables.Reverse();

            var actualVariableNames = vs.Select(v => v.Name).ToArray();

            Common.TestVariableNames(tableName, actualVariableNames, variableNames());

            vs.ToList().ForEach((v) => TestDataSet3DVariable(tableName, lookupColumn3dTest, lookupColumnTest, v));
        }

        public static LookupColumn3dTest LookupColumn3dTestFromDataSetTruncated(string filename, int count3)
        {
            return (name) =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);
                var v = Common.FindVariableInDataSet(data, name);

                if (v != null && v.Rank == 3)
                {
                    var actualTruncated = (double[, ,])null;

                    switch (v.TypeOfData.Name.ToString().ToLower())
                    {
                        case "double":
                            {
                                var actual = (double[, ,])v.GetData();
                                var count1 = actual.GetLength(0);
                                var count2 = actual.GetLength(1);

                                actualTruncated = new double[count1, count2, count3];

                                for (var ii = 0; ii < count1; ii++)
                                {
                                    for (var jj = 0; jj < count2; jj++)
                                    {
                                        for (var kk = 0; kk < count3; kk++)
                                        {
                                            actualTruncated[ii, jj, kk] = actual[ii, jj, kk];
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            throw new Exception("unexpected column type");
                    }

                    return new Column3dTest(actualTruncated, null, true);
                }
                else
                {
                    return Empty;
                }
            };
        }
    }
}
