using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Research.Science.Data;
using NUnit.Framework;

namespace Madingley.Test.Common
{
    public delegate string[] LookupColumnNames();

    public static class Common
    {
        public static bool RandomBool(Random rnd)
        {
            return rnd.NextDouble() > 0.5;
        }

        public static double RandomFloat(Random rnd)
        {
            return (double)((float)(rnd.NextDouble()));
        }

        public static double RandomLatitude(Random rnd)
        {
            return (rnd.NextDouble() - 90.0) * 180.0;
        }

        public static double RandomLongitude(Random rnd)
        {
            return (rnd.NextDouble() - 180.0) * 360.0;
        }

        public static string RandomString(Random rnd, int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWUXYZ0123456789";
            var charsLen = chars.Length;

            var rndChars = Enumerable.Range(0, length).Select(i => chars[rnd.Next(chars.Length)]).ToArray();

            return new string(rndChars);
        }

        public static double[] RandomDoubleArray(Random rnd, int length)
        {
            return Enumerable.Range(0, length).Select(i => rnd.NextDouble()).ToArray();
        }

        public static int[] RandomIntArray(Random rnd, int length)
        {
            return Enumerable.Range(0, length).Select(i => rnd.Next()).ToArray();
        }

        public static string[] RandomStringArray(Random rnd, int length1, int length2)
        {
            return Enumerable.Range(0, length1).Select(i => RandomString(rnd, length2).ToLower()).ToArray();
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var filePaths = Directory.GetFiles(path);
                filePaths.ToList().ForEach(filePath => File.Delete(filePath));

                var directoryPaths = Directory.GetDirectories(path);
                directoryPaths.ToList().ForEach(directoryPath => DeleteDirectory(directoryPath));

                Directory.Delete(path);
            }
        }

        public static void CopyDirectory(string target, string source)
        {
            DeleteDirectory(target);

            Directory.CreateDirectory(target);

            var filePaths = Directory.GetFiles(source);
            filePaths.ToList().ForEach(sourceFileName =>
            {
                var destFileName = Path.Combine(target, Path.GetFileName(sourceFileName));
                File.Copy(sourceFileName, destFileName);
            });

            var directoryPaths = Directory.GetDirectories(source);
            directoryPaths.ToList().ForEach(sourceDirectoryName =>
            {
                var targetDirectoryName = Path.Combine(target, Path.GetFileName(sourceDirectoryName));
                CopyDirectory(targetDirectoryName, sourceDirectoryName);
            });
        }

        public static string FormatTests<T>(IEnumerable<Tuple<T, T, bool>> test)
        {
            return string.Join ("|", test.Select(i => string.Format ("{0}={1}?{2}", i.Item1, i.Item2, i.Item3)));
        }

        public static Variable FindVariableInDataSet(DataSet data, string name)
        {
            var vs = data.Variables.Reverse().ToArray();

            return vs.FirstOrDefault(v => v.Name == name);
        }

        public static LookupColumnNames LookupColumnNamesFromDataSet(string filename)
        {
            return () =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);

                return data.Variables.Reverse().Select(v => v.Name).ToArray();
            };
        }

        public static LookupColumnNames LookupColumnNamesFromDataSetReverse(string filename)
        {
            return () =>
            {
                var fileString = "msds:nc?file=" + filename + "&openMode=readOnly";
                var data = DataSet.Open(fileString);

                return data.Variables.Select(v => v.Name).ToArray();
            };
        }

        public static LookupColumnNames LookupColumnNamesFromTSV(string filename)
        {
            return () =>
            {
                var fileString = "msds:csv?file=" + filename + "&openMode=readOnly&separator=tab";
                var data = DataSet.Open (fileString);

                return data.Variables.Reverse().Select(v => v.Name).ToArray();
            };
        }

        public static void TestVariableNames(string name, string[] actual, string[] expected)
        {
#if DEBUG_COMPARISON
            var tests = actual.Zip(expected, (a, e) => Tuple.Create(a, e, a.Equals(e)));

            Assert.IsTrue(tests.All(match => match.Item3), string.Format("Table: {0}, Columns: {1}", name, FormatTests(tests)));
#else
            Assert.That(actual, Is.EquivalentTo(expected));
#endif
        }
    }
}
