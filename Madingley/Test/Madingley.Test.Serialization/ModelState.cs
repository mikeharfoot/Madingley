using System.IO;
using System.Text;

using NUnit.Framework;

namespace Madingley.Test.Serialization
{
    public class ModelState
    {
        [Test]
        public void TestModelStateSerialization()
        {
            var rnd = new System.Random();

            var expected = Madingley.Test.Common.ModelState.RandomModelState(rnd);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            Madingley.Serialization.ModelState.Serialize(expected, sw);

            var json = sb.ToString();

            var sr = new StringReader(json);

            var actual = Madingley.Serialization.ModelState.Deserialize(sr);

            Assert.AreEqual(expected, actual);
        }
    }
}
