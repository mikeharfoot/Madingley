using System.IO;
using System.Text;

using NUnit.Framework;

namespace Madingley.Test.Serialization
{
    public class Configuration
    {
        [Test]
        public void TestConfigurationSerialization()
        {
            var rnd = new System.Random();

            var expected = Madingley.Test.Common.Configuration.RandomConfiguration(rnd);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            Madingley.Serialization.Configuration.Serialize(expected, sw);

            var json = sb.ToString();

            var sr = new StringReader(json);

            var actual = Madingley.Serialization.Configuration.Deserialize(sr);

            Assert.AreEqual(expected, actual);
        }
    }
}
