using System.Collections.Generic;

namespace Madingley.Common
{
    /// <summary>
    /// Interface for tracking intra cell processes.
    /// </summary>
    public interface IProcessTracker
    {
        /// <summary>
        /// Record a new cohort.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="offspringCohortAbundance">Number of individuals in the new cohort.</param>
        /// <param name="parentCohortAdultMass">Adult body mass of the parent cohort.</param>
        /// <param name="functionalGroup">Functional group that the parent and offspring cohorts belong to.</param>
        /// <param name="parentCohortIDs">All cohort IDs associated with the acting parent cohort.</param>
        /// <param name="offspringCohortID">Cohort ID that has been assigned to the produced offspring cohort.</param>
        void RecordNewCohort(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            double offspringCohortAbundance,
            double parentCohortAdultMass,
            int functionalGroup,
            IEnumerable<int> parentCohortIDs,
            int offspringCohortID);

        /// <summary>
        /// Track the maturity of a cohort.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="birthTimestep">Birth time step of the cohort reaching maturity.</param>
        /// <param name="juvenileMass">Juvenile mass of the cohort reaching maturity.</param>
        /// <param name="adultMass">Adult mass of the cohort reaching maturity.</param>
        /// <param name="functionalGroup">Functional group of the cohort reaching maturity.</param>
        void TrackMaturity(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            int birthTimestep,
            double juvenileMass,
            double adultMass,
            int functionalGroup);

        /// <summary>
        /// Track the flow of mass between trophic levels during a predation event.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="fromFunctionalGroup">Index of the functional group being eaten.</param>
        /// <param name="toFunctionalGroup">Index of the functional group that the predator belongs to.</param>
        /// <param name="massEaten">Mass eaten during the predation event.</param>
        /// <param name="predatorBodyMass">Body mass of the predator doing the eating.</param>
        /// <param name="preyBodyMass">Body mass of the prey being eaten.</param>
        /// <param name="marineCell">True if this is a marine cell, False otherwise.</param>
        void TrackPredationTrophicFlow(
            int latitudeIndex,
            int longitudeIndex,
            int fromFunctionalGroup,
            int toFunctionalGroup,
            double massEaten,
            double predatorBodyMass,
            double preyBodyMass,
            bool marineCell);

        /// <summary>
        /// Track the flow of mass between trophic levels during a herbivory event.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="toFunctionalGroup">Index of the functional group that the herbivore belongs to.</param>
        /// <param name="massEaten">Mass eaten during the herbivory event.</param>
        /// <param name="predatorBodyMass">Body mass of the herbivore doing the eating.</param>
        /// <param name="marineCell">True if this is a marine cell, False otherwise.</param>
        void TrackHerbivoryTrophicFlow(
            int latitudeIndex,
            int longitudeIndex,
            int toFunctionalGroup,
            double massEaten,
            double predatorBodyMass,
            bool marineCell);

        /// <summary>
        /// Track the flow of mass between trophic levels during primary production of autotrophs.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="massEaten">Mass gained through primary production.</param>
        void TrackPrimaryProductionTrophicFlow(
            int latitudeIndex,
            int longitudeIndex,
            double massEaten);

        /// <summary>
        /// Track growth of individuals in a cohort.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="currentBodyMass">Current body mass of individuals in the cohort.</param>
        /// <param name="functionalGroup">Functional group of the cohort being tracked.</param>
        /// <param name="netGrowth">Net growth of individuals in the cohort this time step.</param>
        /// <param name="metabolism">Mass lost to indivduals in the cohort through metabolism.</param>
        /// <param name="predation">Mass gained by individuals in the cohort through predation.</param>
        /// <param name="herbivory">Mass gained by individuals in the cohort through herbivory.</param>
        void TrackTimestepGrowth(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            double currentBodyMass,
            int functionalGroup,
            double netGrowth,
            double metabolism,
            double predation,
            double herbivory);

        /// <summary>
        /// Records the flow of mass between a prey and its predator during a predation event.
        /// </summary>
        /// <param name="timeStep">Time step.</param>
        /// <param name="preyBodyMass">Individual body mass of the prey.</param>
        /// <param name="predatorBodyMass">Individual body mass of the predator.</param>
        /// <param name="massFlow">Flow of mass between predator and prey.</param>
        void RecordPredationMassFlow(
            int timeStep,
            double preyBodyMass,
            double predatorBodyMass,
            double massFlow);

        /// <summary>
        /// Records the flow of mass between primary producers and herbivores during a herbivory event.
        /// </summary>
        /// <param name="timeStep">Time step.</param>
        /// <param name="herbivoreBodyMass">Individual body mass of the herbivore.</param>
        /// <param name="massFlow">Flow of mass between the primary producer and the herbivore.</param>
        void RecordHerbivoryMassFlow(
            int timeStep,
            double herbivoreBodyMass,
            double massFlow);

        /// <summary>
        /// Record an instance of mortality.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="birthTimeStep">Time step in which this cohort was born.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="currentMass">Current body mass of individuals in the cohort.</param>
        /// <param name="adultMass">Adult mass of individuals in the cohort.</param>
        /// <param name="functionalGroup">Functional group of the cohort suffering mortality.</param>
        /// <param name="cohortID">ID of the cohort suffering mortality.</param>
        /// <param name="numberDied">Number of individuals dying in this mortality event.</param>
        /// <param name="mortalitySource">Type of mortality causing the individuals to die.</param>
        void RecordMortality(
            int latitudeIndex,
            int longitudeIndex,
            int birthTimeStep,
            int timeStep,
            double currentMass,
            double adultMass,
            int functionalGroup,
            int cohortID,
            double numberDied,
            string mortalitySource);

        /// <summary>
        /// Output the mortality profile of a cohort becoming extinct.
        /// </summary>
        /// <param name="cohortID">ID of the cohort becoming extinct.</param>
        void OutputMortalityProfile(
            int cohortID);

        /// <summary>
        /// Record the extinction of a cohort.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="merged">Whether the cohort becoming extinct has ever been merged?</param>
        /// <param name="cohortIDs">IDs of all cohorts that have contributed individuals to the cohort going extinct.</param>
        void RecordExtinction(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            bool merged,
            IEnumerable<int> cohortIDs);

        /// <summary>
        /// Tracks the mass lost by individuals in a cohort in a time step through metabolism.
        /// </summary>
        /// <param name="latitudeIndex">Latitude index in grid.</param>
        /// <param name="longitudeIndex">Longitude index in grid.</param>
        /// <param name="timeStep">Time step.</param>
        /// <param name="currentBodyMass">Body mass of individuals in the cohort.</param>
        /// <param name="functionalGroup">Functional group index of the cohort.</param>
        /// <param name="temperature">Ambient temperature in the grid cell.</param>
        /// <param name="metabolicLoss">Mass lost by individuals through metabolism.</param>
        void TrackTimestepMetabolism(
            int latitudeIndex,
            int longitudeIndex,
            int timeStep,
            double currentBodyMass,
            int functionalGroup,
            double temperature,
            double metabolicLoss);
    }
}
