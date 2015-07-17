
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

            var json = Madingley.Serialization.ModelState.Serialize(expected);

            var actual = Madingley.Serialization.ModelState.Deserialize(json);

            Assert.AreEqual(expected, actual);
        }
    }
}
