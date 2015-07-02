using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Research.Science.Data;
using NUnit.Framework;

namespace Madingley.Test.Common
{
    public delegate ColumnTest LookupColumnTest(string name);

    public class ColumnTest
    {
        public static ColumnTest Empty = new ColumnTest(new double[] { }, null, false);

        public static LookupColumnTest EmptyLookup = (name) => Empty;

        public double[] Expected { get; set; }

        public IEqualityComparer<double> Tolerance { get; set; }

        public bool Include { get; set; }

        public ColumnTest(
            double[] expected,
            IEqualityComparer<double> tolerance,
            bool include)
        {
            this.Expected = expected;
            this.Tolerance = tolerance;
            this.Include = include;
        }

        public static void DoColumnTest(string tableName, string columnName, ColumnTest columnTest, double[] actual)
        {
#if DEBUG_COMPARISON
            Func<double, double, bool> test = columnTest.Tolerance != null ?
                (Func<double, double, bool>)((a, e) => columnTest.Tolerance.Equals(a, e)) :
                (Func<double, double, bool>)((a, e) => a.Equals(e));

            var tests = actual.Zip(columnTest.Expected, (a, e) => Tuple.Create(a, e, test(a, e)));

            Assert.IsTrue(tests.All(match => match.Item3), String.Format("Table: {0}, Column: {1}, Tests: {2}", tableName, columnName, Common.FormatTests(tests)));
#else
            if (columnTest.Tolerance != null)
            {
                Assert.That(actual.SequenceEqual(columnTest.Expected, columnTest.Tolerance));
            }
            else
            {
                Assert.That(actual, Is.EquivalentTo(columnTest.Expected));
            }
#endif
        }

        public static void TestDataSetVariable(string tableName, LookupColumnTest lookupColumnTest, Variable v)
        {
            var columnTest = lookupColumnTest.Invoke(v.Name);

            if (columnTest.Include)
            {
                var count = columnTest.Expected.Count();
                var actualTruncated = (double[])null;

                switch (v.TypeOfData.Name.ToString().ToLower())
                {
                    case "single":
                        var actual = (float[])v.GetData();

                        actualTruncated = actual.Select(a => (double)a).Take(count).ToArray();
                        break;

                    case "double":
                        actualTruncated = ((double[])v.GetData()).Take(count).ToArray();
                        break;

                    default:
                        throw new Exception("unexpected column type");
                }

                DoColumnTest(tableName, v.Name, columnTest, actualTruncated);
            }
            else
            {
                Console.WriteLine("Skipping : {0}", v.Name);
            }
        }

        public static void TestDataSet(string name, LookupColumnTest lookupColumnTest, LookupColumnNames variableNames, DataSet dataSet)
        {
            var vs = dataSet.Variables.Reverse();

            var actualVariableNames = vs.Select(v => v.Name).ToArray();

            Common.TestVariableNames(name, actualVariableNames, variableNames.Invoke());

            vs.ToList().ForEach(v => TestDataSetVariable(name, lookupColumnTest, v));
        }

        public static void TestDataSetArray(string name, LookupColumnTest[] lookupColumnTests, LookupColumnNames variableNames, DataSet[] dataSets)
        {
            Enumerable.Range(0, dataSets.Length).ToList().ForEach(
                ii =>
                {
                    if (ii < lookupColumnTests.Length && 
                        lookupColumnTests[ii] != null)
                    {
                        TestDataSet(name, lookupColumnTests[ii], variableNames, dataSets[ii]);
                    }
                });
        }

        public static LookupColumnTest LookupColumnTestFromDataSetAll(string filename)
        {
            return (name) =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);
                var v = Common.FindVariableInDataSet(data, name);

                if (v != null && v.Rank == 1)
                {
                    var actualTruncated = (double[])null;

                    switch (v.TypeOfData.Name.ToString().ToLower())
                    {
                        case "single":
                            var actual = (float[])v.GetData();

                            actualTruncated = actual.Select(a => (double)a).ToArray();
                            break;

                        case "double":
                            actualTruncated = (double[])v.GetData();
                            break;

                        default:
                            throw new Exception("unexpected column type");
                    }

                    return new ColumnTest(actualTruncated, null, true);
                }
                else
                {
                    return Empty;
                }
            };
        }

        public static LookupColumnTest LookupColumnTestFromDataSetTruncated(string filename, int count)
        {
            return (name) =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);
                var v = Common.FindVariableInDataSet(data, name);

                if (v != null && v.Rank == 1)
                {
                    var actualTruncated = (double[])null;

                    switch (v.TypeOfData.Name.ToString().ToLower())
                    {
                        case "single":
                            var actual = (float[])v.GetData();

                            actualTruncated = actual.Select(a => (double)a).Take(count).ToArray();
                            break;

                        case "double":
                            actualTruncated = ((double[])v.GetData()).Take(count).ToArray();
                            break;

                        default:
                            throw new Exception("unexpected column type");
                    }

                    return new ColumnTest(actualTruncated, null, true);
                }
                else
                {
                    return Empty;
                }
            };
        }

        public static LookupColumnTest LookupColumnTestFromTSVTruncated(string filename, int count)
        {
            return (name) =>
            {
                var fileString = "msds:csv?file=" + filename + "&openMode=readOnly&separator=tab";
                var data = DataSet.Open(fileString);
                var v = Common.FindVariableInDataSet(data, name);

                if (v != null)
                {
                    var actualTruncated = (double[])null;

                    switch (v.TypeOfData.Name.ToString().ToLower())
                    {
                        case "single":
                            var actual = (float[])v.GetData();

                            actualTruncated = actual.Select(a => (double)a).Take(count).ToArray();
                            break;

                        case "double":
                            actualTruncated = ((double[])v.GetData()).Take(count).ToArray();
                            break;

                        default:
                            throw new Exception("unexpected column type");
                    }

                    return new ColumnTest(actualTruncated, null, true);
                }
                else
                {
                    return Empty;
                }
            };
        }
    }
}
