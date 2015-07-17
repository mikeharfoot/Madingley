using System.IO;
using System.Text;

using NUnit.Framework;

namespace Madingley.Test.Serialization
{
    public class Environment
    {
        [Test]
        public void TestEnvironmentSerialization()
        {
            var rnd = new System.Random();

            var expected = Madingley.Test.Common.Environment.RandomEnvironment(rnd);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            Madingley.Serialization.Environment.Serialize(expected, sw);

            var json = sb.ToString();

            var sr = new StringReader(json);

            var actual = Madingley.Serialization.Environment.Deserialize(sr);

            Assert.AreEqual(expected, actual);
        }
    }
}
