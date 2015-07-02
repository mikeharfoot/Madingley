using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madingley.Common
{
    public interface IProcessTracker
    {
        /// <summary>
        /// Record a new cohort in the reproduction tracker
        /// </summary>
        /// <param name="offspringCohortAbundance">The number of individuals in the new cohort</param>
        /// <param name="parentCohortAdultMass">The adult body mass of the parent cohort</param>
        /// <param name="functionalGroup">The functional group that the parent and offspring cohorts belong to</param>
        /// <param name="parentCohortIDs">All cohort IDs associated with the acting parent cohort</param>
        /// <param name="offspringCohortID">The cohort ID that has been assigned to the produced offspring cohort</param>
        void RecordNewCohort(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            double offspringCohortAbundance,
            double parentCohortAdultMass,
            int functionalGroup,
            uint[] parentCohortIDs,
            uint offspringCohortID);

        /// <summary>
        /// Track the maturity of a cohort in the reproduction tracker
        /// </summary>
        /// <param name="birthTimestep">The birth time step of the cohort reaching maturity</param>
        /// <param name="juvenileMass">The juvenile mass of the cohort reaching maturity</param>
        /// <param name="adultMass">The adult mass of the cohort reaching maturity</param>
        /// <param name="functionalGroup">The functional group of the cohort reaching maturity</param>
        void TrackMaturity(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            uint birthTimestep,
            double juvenileMass,
            double adultMass,
            int functionalGroup);

        /// <summary>
        /// Track the flow of mass between trophic levels during a predation event
        /// </summary>
        /// <param name="fromFunctionalGroup">The index of the functional group being eaten</param>
        /// <param name="toFunctionalGroup">The index of the functional group that the predator belongs to</param>
        /// <param name="massEaten">The mass eaten during the predation event</param>
        /// <param name="predatorBodyMass">The body mass of the predator doing the eating</param>
        /// <param name="preyBodyMass">The body mass of the prey doing the eating</param>
        void TrackPredationTrophicFlow(
            uint latIndex,
            uint lonIndex,
            int fromFunctionalGroup,
            int toFunctionalGroup,
            double massEaten,
            double predatorBodyMass,
            double preyBodyMass,
            bool marineCell);

        /// <summary>
        /// Track the flow of mass between trophic levels during a herbivory event
        /// </summary>
        /// <param name="toFunctionalGroup">The index of the functional group that the predator belongs to</param>
        /// <param name="massEaten">The mass eaten during the herbivory event</param>
        /// <param name="predatorBodyMass">The body mass of the predator doing the eating</param>
        void TrackHerbivoryTrophicFlow(
            uint latIndex,
            uint lonIndex,
            int toFunctionalGroup,
            double massEaten,
            double predatorBodyMass,
            bool marineCell);

        /// <summary>
        /// Track the flow of mass between trophic levels during primary production of autotrophs
        /// </summary>
        /// <param name="massEaten">The mass gained through primary production</param>
        void TrackPrimaryProductionTrophicFlow(
            uint latIndex,
            uint lonIndex,
            double massEaten);

        /// <summary>
        /// Track growth of individuals in a cohort using the growth tracker
        /// </summary>
        /// <param name="currentBodyMass">The current body mass of individuals in the cohort</param>
        /// <param name="functionalGroup">The funcitonal group of the cohort being tracked</param>
        /// <param name="netGrowth">The net growth of individuals in the cohort this time step</param>
        /// <param name="metabolism">The mass lost to indivduals in the cohort through metabolism</param>
        /// <param name="predation">The mass gained by individuals in the cohort through predation</param>
        /// <param name="herbivory">The mass gained by individuals in the cohort through herbivory</param>
        void TrackTimestepGrowth(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            double currentBodyMass,
            int functionalGroup,
            double netGrowth,
            double metabolism,
            double predation,
            double herbivory);

        /// <summary>
        /// Records the flow of mass between a prey and its predator during a predation event
        /// </summary>
        /// <param name="preyBodyMass">The individual body mass of the prey</param>
        /// <param name="predatorBodyMass">The individual body mass of the predator</param>
        /// <param name="massFlow">The flow of mass between predator and prey</param>
        void RecordPredationMassFlow(
            uint currentTimeStep,
            double preyBodyMass,
            double predatorBodyMass,
            double massFlow);

        /// <summary>
        /// Records the flow of mass between primary producers and herbivores during a herbivory event
        /// </summary>
        /// <param name="herbivoreBodyMass">The individual body mass of the herbivore</param>
        /// <param name="massFlow">The flow of mass between the primary producer and the herbivore</param>
        void RecordHerbivoryMassFlow(
            uint currentTimeStep,
            double herbivoreBodyMass,
            double massFlow);

        /// <summary>
        /// Record an instance of mortality in the output file
        /// </summary>
        /// <param name="birthTimeStep">The time step in which this cohort was born</param>
        /// <param name="currentMass">The current body mass of individuals in the cohort</param>
        /// <param name="adultMass">The adult mass of individuals in the cohort</param>
        /// <param name="functionalGroup">The functional group of the cohort suffering mortality</param>
        /// <param name="cohortID">The ID of the cohort suffering mortality</param>
        /// <param name="numberDied">The number of individuals dying in this mortality event</param>
        /// <param name="mortalitySource">The type of mortality causing the individuals to die</param>
        void RecordMortality(
            uint latIndex,
            uint lonIndex,
            uint birthTimeStep,
            uint timeStep,
            double currentMass,
            double adultMass,
            uint functionalGroup,
            uint cohortID,
            double numberDied,
            string mortalitySource);

        void OutputMortalityProfile(
            uint cohortID);

        /// <summary>
        /// Record the extinction of a cohort
        /// </summary>
        /// <param name="merged">Whether the cohort becoming extinct has ever been merged</param>
        /// <param name="cohortIDs">The IDs of all cohorts that have contributed individuals to the cohort going extinct</param>
        void RecordExtinction(
            uint latIndex,
            uint lonIndex,
            uint currentTimeStep,
            bool merged,
            uint[] cohortIDs);

        /// <summary>
        /// Tracks the mass lost by individuals in a cohort in a time step through metabolism
        /// </summary>
        /// <param name="currentBodyMass">The body mass of individuals in the acting cohort</param>
        /// <param name="functionalGroup">The functional group index of the acting cohort</param>
        /// <param name="temperature">The ambient temperature in the grid cell</param>
        /// <param name="metabolicLoss">The mass lost by individuals through metabolism</param>
        void TrackTimestepMetabolism(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            double currentBodyMass,
            int functionalGroup,
            double temperature,
            double metabolicLoss);
    }
}
