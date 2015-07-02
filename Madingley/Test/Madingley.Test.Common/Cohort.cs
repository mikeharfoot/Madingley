using System;
using System.Linq;

namespace Madingley.Test.Common
{
    public static class Cohort
    {
        public static Madingley.Common.Cohort RandomCohort(Random rnd)
        {
            var birthTimeStep = rnd.Next();
            var maturityTimeStep = rnd.Next();
            var cohortID = Enumerable.Range(0, 3).Select(i => rnd.Next()).ToArray();
            var juvenileMass = rnd.NextDouble();
            var adultMass = rnd.NextDouble();
            var individualBodyMass = rnd.NextDouble();
            var individualReproductivePotentialMass = rnd.NextDouble();
            var maximumAchievedBodyMass = rnd.NextDouble();
            var abundance = rnd.NextDouble();
            var merged = Common.RandomBool(rnd);
            var proportionTimeActive = rnd.NextDouble();
            var trophicIndex = rnd.NextDouble() * 5.0;
            var logOptimalPreyBodySizeRatio = rnd.NextDouble();

            return new Madingley.Common.Cohort(
                birthTimeStep,
                maturityTimeStep,
                cohortID,
                juvenileMass,
                adultMass,
                individualBodyMass,
                individualReproductivePotentialMass,
                maximumAchievedBodyMass,
                abundance,
                merged,
                proportionTimeActive,
                trophicIndex,
                logOptimalPreyBodySizeRatio);
        }
    }
}
