using System;

namespace Madingley.Test.Common
{
    public static class Stock
    {
        public static Madingley.Common.Stock RandomStock(Random rnd)
        {
            var individualBodyMass = rnd.NextDouble();
            var totalBiomass = rnd.NextDouble();

            return new Madingley.Common.Stock(
                individualBodyMass,
                totalBiomass);
        }
    }
}
