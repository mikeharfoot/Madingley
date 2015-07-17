
using NUnit.Framework;

namespace Madingley.Test.Serialization
{
    public class Environment
    {
        [Test]
        public void TestEnvironmentSerialization()
        {
            var modelSetupRoot = "Model setup for tests";
            var environmentDataRoot = "Data for tests";

            Madingley.Test.Common.Environment.CreateDirectories(environmentDataRoot);
            Madingley.Test.Common.Environment.CreateFiles(environmentDataRoot, true);

            var expected = Madingley.Environment.Loader.Load(environmentDataRoot, modelSetupRoot);

            var json = Madingley.Serialization.Environment.Serialize(expected);

            var actual = Madingley.Serialization.Environment.Deserialize(json);

            Assert.AreEqual(expected, actual);
        }
    }
}
