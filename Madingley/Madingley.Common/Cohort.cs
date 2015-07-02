using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class Cohort
    {
        public int BirthTimeStep { get; set; }

        public int MaturityTimeStep { get; set; }

        public IEnumerable<int> CohortID { get; set; }

        public double JuvenileMass { get; set; }

        public double AdultMass { get; set; }

        public double IndividualBodyMass { get; set; }

        public double IndividualReproductivePotentialMass { get; set; }

        public double MaximumAchievedBodyMass { get; set; }

        public double Abundance { get; set; }

        public bool Merged { get; set; }

        public double ProportionTimeActive { get; set; }

        public double TrophicIndex { get; set; }

        public double LogOptimalPreyBodySizeRatio { get; set; }

        public Cohort(
            int birthTimeStep,
            int maturityTimeStep,
            IEnumerable<int> cohortID,
            double juvenileMass,
            double adultMass,
            double individualBodyMass,
            double individualReproductivePotentialMass,
            double maximumAchievedBodyMass,
            double abundance,
            bool merged,
            double proportionTimeActive,
            double trophicIndex,
            double logOptimalPreyBodySizeRatio)
        {
            this.BirthTimeStep = birthTimeStep;
            this.MaturityTimeStep = maturityTimeStep;
            this.CohortID = cohortID.ToArray();
            this.JuvenileMass = juvenileMass;
            this.AdultMass = adultMass;
            this.IndividualBodyMass = individualBodyMass;
            this.IndividualReproductivePotentialMass = individualReproductivePotentialMass;
            this.MaximumAchievedBodyMass = maximumAchievedBodyMass;
            this.Abundance = abundance;
            this.Merged = merged;
            this.ProportionTimeActive = proportionTimeActive;
            this.TrophicIndex = trophicIndex;
            this.LogOptimalPreyBodySizeRatio = logOptimalPreyBodySizeRatio;
        }

        public Cohort(Cohort c)
        {
            this.BirthTimeStep = c.BirthTimeStep;
            this.MaturityTimeStep = c.MaturityTimeStep;
            this.CohortID = c.CohortID.ToArray();
            this.JuvenileMass = c.JuvenileMass;
            this.AdultMass = c.AdultMass;
            this.IndividualBodyMass = c.IndividualBodyMass;
            this.IndividualReproductivePotentialMass = c.IndividualReproductivePotentialMass;
            this.MaximumAchievedBodyMass = c.MaximumAchievedBodyMass;
            this.Abundance = c.Abundance;
            this.Merged = c.Merged;
            this.ProportionTimeActive = c.ProportionTimeActive;
            this.TrophicIndex = c.TrophicIndex;
            this.LogOptimalPreyBodySizeRatio = c.LogOptimalPreyBodySizeRatio;
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as Cohort;
            if ((Object)y == null) return false;

            return
                this.BirthTimeStep.Equals(y.BirthTimeStep) &&
                this.MaturityTimeStep.Equals(y.MaturityTimeStep) &&
                this.CohortID.SequenceEqual(y.CohortID) &&
                this.JuvenileMass.Equals(y.JuvenileMass) &&
                this.AdultMass.Equals(y.AdultMass) &&
                this.IndividualBodyMass.Equals(y.IndividualBodyMass) &&
                this.IndividualReproductivePotentialMass.Equals(y.IndividualReproductivePotentialMass) &&
                this.MaximumAchievedBodyMass.Equals(y.MaximumAchievedBodyMass) &&
                this.Abundance.Equals(y.Abundance) &&
                this.Merged.Equals(y.Merged) &&
                this.ProportionTimeActive.Equals(y.ProportionTimeActive) &&
                this.TrophicIndex.Equals(y.TrophicIndex) &&
                this.LogOptimalPreyBodySizeRatio.Equals(y.LogOptimalPreyBodySizeRatio);
        }

        public override int GetHashCode()
        {
            return
                this.BirthTimeStep.GetHashCode() ^
                this.MaturityTimeStep.GetHashCode() ^
                this.CohortID.GetHashCode() ^
                this.JuvenileMass.GetHashCode() ^
                this.AdultMass.GetHashCode() ^
                this.IndividualBodyMass.GetHashCode() ^
                this.IndividualReproductivePotentialMass.GetHashCode() ^
                this.MaximumAchievedBodyMass.GetHashCode() ^
                this.Abundance.GetHashCode() ^
                this.Merged.GetHashCode() ^
                this.ProportionTimeActive.GetHashCode() ^
                this.LogOptimalPreyBodySizeRatio.GetHashCode();
        }
    }
}
