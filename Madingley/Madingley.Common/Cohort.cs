using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Class to hold properties of a single cohort.
    /// </summary>
    public class Cohort
    {
        /// <summary>
        /// Index of the functional group the cohort belongs to.
        /// </summary>
        public int FunctionalGroupIndex { get; set; }

        /// <summary>
        /// Time step when this cohort was generated.
        /// </summary>
        public int BirthTimeStep { get; set; }

        /// <summary>
        /// Time step at which this cohort reached maturity.
        /// </summary>
        public int MaturityTimeStep { get; set; }

        /// <summary>
        /// List of all cohort IDs ever associated with individuals in this cohort.
        /// </summary>
        public IList<int> IDs { get; set; }

        /// <summary>
        /// Mean juvenile mass of individuals in this cohort.
        /// </summary>
        public double JuvenileMass { get; set; }

        /// <summary>
        /// Mean mature adult mass of individuals in this cohort.
        /// </summary>
        public double AdultMass { get; set; }

        /// <summary>
        /// Mean body mass of an individual in this cohort.
        /// </summary>
        public double IndividualBodyMass { get; set; }

        /// <summary>
        /// Individual biomass assigned to reproductive potential.
        /// </summary>
        public double IndividualReproductivePotentialMass { get; set; }

        /// <summary>
        /// Maximum mean body mass ever achieved by individuals in this cohort.
        /// </summary>
        public double MaximumAchievedBodyMass { get; set; }

        /// <summary>
        /// Number of individuals in the cohort.
        /// </summary>
        public double Abundance { get; set; }

        /// <summary>
        /// Whether this cohort has ever been merged with another cohort.
        /// </summary>
        public bool Merged { get; set; }

        /// <summary>
        /// Proportion of the timestep for which this cohort is active.
        /// </summary>
        public double ProportionTimeActive { get; set; }

        /// <summary>
        /// Trophic index for this cohort at this time.
        /// </summary>
        public double TrophicIndex { get; set; }

        /// <summary>
        /// Optimal prey body size for individuals in this cohort.
        /// </summary>
        public double LogOptimalPreyBodySizeRatio { get; set; }

        /// <summary>
        /// Cohort default constructor
        /// </summary>
        public Cohort()
        {
            this.FunctionalGroupIndex = 0;
            this.BirthTimeStep = 0;
            this.MaturityTimeStep = 0;
            this.IDs = new int[] { };
            this.JuvenileMass = 0.0;
            this.AdultMass = 0.0;
            this.IndividualBodyMass = 0.0;
            this.IndividualReproductivePotentialMass = 0.0;
            this.MaximumAchievedBodyMass = 0.0;
            this.Abundance = 0.0;
            this.Merged = false;
            this.ProportionTimeActive = 0.0;
            this.TrophicIndex = 0.0;
            this.LogOptimalPreyBodySizeRatio = 0.0;
        }

        /// <summary>
        /// Cohort constructor.
        /// </summary>
        /// <param name="functionalGroupIndex">Index of the functional group the stock belongs to.</param>
        /// <param name="birthTimeStep">Birth time step.</param>
        /// <param name="maturityTimeStep">Maturity time step.</param>
        /// <param name="cohortID">List of cohort IDs.</param>
        /// <param name="juvenileMass">Mean juvenile body mass of individuals.</param>
        /// <param name="adultMass">Mean mature adult body mass of individuals.</param>
        /// <param name="individualBodyMass">Initial mean body mass of individuals.</param>
        /// <param name="individualReproductivePotentialMass">Individual biomass assigned to reproductive potential.</param>
        /// <param name="maximumAchievedBodyMass">Maximum mean body mass ever achieved by individuals.</param>
        /// <param name="abundance">Number of individuals.</param>
        /// <param name="merged">Whether this cohort has ever been merged with another cohort.</param>
        /// <param name="proportionTimeActive">Proportion of the timestep for which this cohort is active.</param>
        /// <param name="trophicIndex">Trophic level index.</param>
        /// <param name="logOptimalPreyBodySizeRatio">Log optimal prey body mass (as a percentage of this cohorts mass) for individuals.</param>
        public Cohort(
            int functionalGroupIndex,
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
            this.FunctionalGroupIndex = functionalGroupIndex;
            this.BirthTimeStep = birthTimeStep;
            this.MaturityTimeStep = maturityTimeStep;
            this.IDs = cohortID.ToArray();
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

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="cohort">Existing Cohort to copy.</param>
        public Cohort(Cohort cohort)
        {
            this.FunctionalGroupIndex = cohort.FunctionalGroupIndex;
            this.BirthTimeStep = cohort.BirthTimeStep;
            this.MaturityTimeStep = cohort.MaturityTimeStep;
            this.IDs = cohort.IDs.ToArray();
            this.JuvenileMass = cohort.JuvenileMass;
            this.AdultMass = cohort.AdultMass;
            this.IndividualBodyMass = cohort.IndividualBodyMass;
            this.IndividualReproductivePotentialMass = cohort.IndividualReproductivePotentialMass;
            this.MaximumAchievedBodyMass = cohort.MaximumAchievedBodyMass;
            this.Abundance = cohort.Abundance;
            this.Merged = cohort.Merged;
            this.ProportionTimeActive = cohort.ProportionTimeActive;
            this.TrophicIndex = cohort.TrophicIndex;
            this.LogOptimalPreyBodySizeRatio = cohort.LogOptimalPreyBodySizeRatio;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both Cohorts and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var cohortObj = obj as Cohort;
            if ((Object)cohortObj == null) return false;

            return
                this.FunctionalGroupIndex.Equals(cohortObj.FunctionalGroupIndex) &&
                this.BirthTimeStep.Equals(cohortObj.BirthTimeStep) &&
                this.MaturityTimeStep.Equals(cohortObj.MaturityTimeStep) &&
                this.IDs.SequenceEqual(cohortObj.IDs) &&
                this.JuvenileMass.Equals(cohortObj.JuvenileMass) &&
                this.AdultMass.Equals(cohortObj.AdultMass) &&
                this.IndividualBodyMass.Equals(cohortObj.IndividualBodyMass) &&
                this.IndividualReproductivePotentialMass.Equals(cohortObj.IndividualReproductivePotentialMass) &&
                this.MaximumAchievedBodyMass.Equals(cohortObj.MaximumAchievedBodyMass) &&
                this.Abundance.Equals(cohortObj.Abundance) &&
                this.Merged.Equals(cohortObj.Merged) &&
                this.ProportionTimeActive.Equals(cohortObj.ProportionTimeActive) &&
                this.TrophicIndex.Equals(cohortObj.TrophicIndex) &&
                this.LogOptimalPreyBodySizeRatio.Equals(cohortObj.LogOptimalPreyBodySizeRatio);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.FunctionalGroupIndex.GetHashCode() ^
                this.BirthTimeStep.GetHashCode() ^
                this.MaturityTimeStep.GetHashCode() ^
                this.IDs.GetHashCode() ^
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
