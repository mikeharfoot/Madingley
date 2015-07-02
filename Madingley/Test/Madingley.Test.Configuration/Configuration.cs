using System;
using NUnit.Framework;

namespace Madingley.Test.Configuration
{
    [TestFixture]
    public class Configuration
    {
        [Test]
        public void TestConfigurationLoad()
        {
            var pathToModelSetup = "Model setup";

            var rnd = new System.Random();

            var expected = Madingley.Test.Common.Configuration.RandomConfiguration(rnd);

            Madingley.Test.Common.Configuration.Save(pathToModelSetup, expected);

            var actual = Madingley.Configuration.Loader.Load(pathToModelSetup);

            Assert.AreEqual(expected, actual);
        }
    }
}
