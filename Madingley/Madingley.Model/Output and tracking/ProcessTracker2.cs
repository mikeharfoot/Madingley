using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley
{
    public class ProcessTracker
    {
        public Boolean TrackProcesses { get; set; }

        public Madingley.Common.IProcessTracker[] ProcessTrackers { get; set; }

        public ProcessTracker(IEnumerable<Madingley.Common.IProcessTracker> processTrackers)
        {
            this.TrackProcesses = processTrackers.Count() > 0;
            this.ProcessTrackers = processTrackers.ToArray();
        }

        /// <summary>
        /// Record a new cohort in the reproduction tracker
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="timestep">The current model time step</param>
        /// <param name="offspringCohortAbundance">The number of individuals in the new cohort</param>
        /// <param name="parentCohortAdultMass">The adult body mass of the parent cohort</param>
        /// <param name="functionalGroup">The functional group that the parent and offspring cohorts belong to</param>
        /// <param name="parentCohortIDs">All cohort IDs associated with the acting parent cohort</param>
        /// <param name="offspringCohortID">The cohort ID that has been assigned to the produced offspring cohort</param>
        public void RecordNewCohort(uint latitudeIndex, uint longitudeIndex, uint timestep, double offspringCohortAbundance, double parentCohortAdultMass, int functionalGroup, List<uint> parentCohortIDs, uint offspringCohortID)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.RecordNewCohort((int)latitudeIndex, (int)longitudeIndex, (int)timestep, offspringCohortAbundance, parentCohortAdultMass, functionalGroup, parentCohortIDs.Select(id => (int)id), (int)offspringCohortID);
            }
        }

        /// <summary>
        /// Track the maturity of a cohort in the reproduction tracker
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="timestep">The current model time step</param>
        /// <param name="birthTimestep">The birth time step of the cohort reaching maturity</param>
        /// <param name="juvenileMass">The juvenile mass of the cohort reaching maturity</param>
        /// <param name="adultMass">The adult mass of the cohort reaching maturity</param>
        /// <param name="functionalGroup">The functional group of the cohort reaching maturity</param>
        public void TrackMaturity(uint latitudeIndex, uint longitudeIndex, uint timestep, uint birthTimestep, double juvenileMass, double adultMass, int functionalGroup)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.TrackMaturity((int)latitudeIndex, (int)longitudeIndex, (int)timestep, (int)birthTimestep, juvenileMass, adultMass, functionalGroup);
            }
        }

        /// <summary>
        /// Track the flow of mass between trophic levels during a predation event
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="fromFunctionalGroup">The index of the functional group being eaten</param>
        /// <param name="toFunctionalGroup">The index of the functional group that the predator belongs to</param>
        /// <param name="cohortFunctionalGroupDefinitions">The functional group definitions of cohorts in the model</param>
        /// <param name="massEaten">The mass eaten during the predation event</param>
        /// <param name="predatorBodyMass">The body mass of the predator doing the eating</param>
        /// <param name="preyBodyMass">The body mass of the prey doing the eating</param>
        /// <param name="initialisation">The Madingley Model initialisation</param>
        /// <param name="marineCell">Whether the current cell is a marine cell</param>
        public void TrackPredationTrophicFlow(uint latitudeIndex, uint longitudeIndex, int fromFunctionalGroup, int toFunctionalGroup,
            FunctionalGroupDefinitions cohortFunctionalGroupDefinitions, double massEaten, double predatorBodyMass, double preyBodyMass,
            MadingleyModelInitialisation initialisation, Boolean marineCell)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.TrackPredationTrophicFlow((int)latitudeIndex, (int)longitudeIndex, fromFunctionalGroup, toFunctionalGroup, massEaten, predatorBodyMass, preyBodyMass, marineCell);
            }
        }

        /// <summary>
        /// Track the flow of mass between trophic levels during a herbivory event
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="toFunctionalGroup">The index of the functional group that the predator belongs to</param>
        /// <param name="cohortFunctionalGroupDefinitions">The functional group definitions of cohorts in the model</param>
        /// <param name="massEaten">The mass eaten during the herbivory event</param>
        /// <param name="predatorBodyMass">The body mass of the predator doing the eating</param>
        /// <param name="initialisation">The Madingley Model initialisation</param>
        /// <param name="marineCell">Whether the current cell is a marine cell</param>
        public void TrackHerbivoryTrophicFlow(uint latitudeIndex, uint longitudeIndex, int toFunctionalGroup,
            FunctionalGroupDefinitions cohortFunctionalGroupDefinitions, double massEaten, double predatorBodyMass,
            MadingleyModelInitialisation initialisation, Boolean marineCell)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.TrackHerbivoryTrophicFlow((int)latitudeIndex, (int)longitudeIndex, toFunctionalGroup, massEaten, predatorBodyMass, marineCell);
            }
        }

        /// <summary>
        /// Track the flow of mass between trophic levels during primary production of autotrophs
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="massEaten">The mass gained through primary production</param>
        public void TrackPrimaryProductionTrophicFlow(uint latitudeIndex, uint longitudeIndex, double massEaten)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.TrackPrimaryProductionTrophicFlow((int)latitudeIndex, (int)longitudeIndex, massEaten);
            }
        }

        /// <summary>
        /// Track growth of individuals in a cohort using the growth tracker
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="timeStep">The current model time step</param>
        /// <param name="currentBodyMass">The current body mass of individuals in the cohort</param>
        /// <param name="functionalGroup">The funcitonal group of the cohort being tracked</param>
        /// <param name="netGrowth">The net growth of individuals in the cohort this time step</param>
        /// <param name="metabolism">The mass lost to indivduals in the cohort through metabolism</param>
        /// <param name="predation">The mass gained by individuals in the cohort through predation</param>
        /// <param name="herbivory">The mass gained by individuals in the cohort through herbivory</param>
        public void TrackTimestepGrowth(uint latitudeIndex, uint longitudeIndex, uint timeStep, double currentBodyMass, int functionalGroup, double netGrowth, double metabolism, double predation, double herbivory)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.TrackTimestepGrowth((int)latitudeIndex, (int)longitudeIndex, (int)timeStep, currentBodyMass, functionalGroup, netGrowth, metabolism, predation, herbivory);
            }
        }

        /// <summary>
        /// Records the flow of mass between a prey and its predator during a predation event
        /// </summary>
        /// <param name="currentTimeStep">The current model time step</param>
        /// <param name="preyBodyMass">The individual body mass of the prey</param>
        /// <param name="predatorBodyMass">The individual body mass of the predator</param>
        /// <param name="massFlow">The flow of mass between predator and prey</param>
        public void RecordPredationMassFlow(uint currentTimeStep, double preyBodyMass, double predatorBodyMass, double massFlow)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.RecordPredationMassFlow((int)currentTimeStep, preyBodyMass, predatorBodyMass, massFlow);
            }
        }

        /// <summary>
        /// Records the flow of mass between primary producers and herbivores during a herbivory event
        /// </summary>
        /// <param name="currentTimeStep">The current model time step</param>
        /// <param name="herbivoreBodyMass">The individual body mass of the herbivore</param>
        /// <param name="massFlow">The flow of mass between the primary producer and the herbivore</param>
        public void RecordHerbivoryMassFlow(uint currentTimeStep, double herbivoreBodyMass, double massFlow)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.RecordHerbivoryMassFlow((int)currentTimeStep, herbivoreBodyMass, massFlow);
            }
        }

        /// <summary>
        /// Record an instance of mortality in the output file
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="birthTimeStep">The time step in which this cohort was born</param>
        /// <param name="timeStep">The current model time step</param>
        /// <param name="currentMass">The current body mass of individuals in the cohort</param>
        /// <param name="adultMass">The adult mass of individuals in the cohort</param>
        /// <param name="functionalGroup">The functional group of the cohort suffering mortality</param>
        /// <param name="cohortID">The ID of the cohort suffering mortality</param>
        /// <param name="numberDied">The number of individuals dying in this mortality event</param>
        /// <param name="mortalitySource">The type of mortality causing the individuals to die</param>
        public void RecordMortality(uint latitudeIndex, uint longitudeIndex, uint birthTimeStep, uint timeStep, double currentMass, double adultMass, uint functionalGroup, uint cohortID, double numberDied, string mortalitySource)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.RecordMortality((int)latitudeIndex, (int)longitudeIndex, (int)birthTimeStep, (int)timeStep, currentMass, adultMass, (int)functionalGroup, (int)cohortID, numberDied, mortalitySource);
            }
        }

        /// <summary>
        /// Output the mortality profile of a cohort becoming extinct
        /// </summary>
        /// <param name="cohortID">The ID of the cohort becoming extinct</param>
        public void OutputMortalityProfile(uint cohortID)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.OutputMortalityProfile((int)cohortID);
            }
        }

        /// <summary>
        /// Record the extinction of a cohort
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="currentTimeStep">THe current time step</param>
        /// <param name="merged">Whether the cohort becoming extinct has ever been merged</param>
        /// <param name="cohortIDs">The IDs of all cohorts that have contributed individuals to the cohort going extinct</param>
        public void RecordExtinction(uint latitudeIndex, uint longitudeIndex, uint currentTimeStep, bool merged, List<uint> cohortIDs)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.RecordExtinction((int)latitudeIndex, (int)longitudeIndex, (int)currentTimeStep, merged, cohortIDs.Select(id => (int)id));
            }
        }

        /// <summary>
        /// Tracks the mass lost by individuals in a cohort in a time step through metabolism
        /// </summary>
        /// <param name="latitudeIndex">The latitudinal index of the current grid cell</param>
        /// <param name="longitudeIndex">The longitudinal index of the current grid cell</param>
        /// <param name="timeStep">The current model time step</param>
        /// <param name="currentBodyMass">The body mass of individuals in the acting cohort</param>
        /// <param name="functionalGroup">The functional group index of the acting cohort</param>
        /// <param name="temperature">The ambient temperature in the grid cell</param>
        /// <param name="metabolicLoss">The mass lost by individuals through metabolism</param>
        public void TrackTimestepMetabolism(uint latitudeIndex, uint longitudeIndex, uint timeStep, double currentBodyMass, int functionalGroup, double temperature, double metabolicLoss)
        {
            foreach (var o in this.ProcessTrackers)
            {
                o.TrackTimestepMetabolism((int)latitudeIndex, (int)longitudeIndex, (int)timeStep, currentBodyMass, functionalGroup, temperature, metabolicLoss);
            }
        }

        public void EndTimeStepPredationTracking(uint timeStep)
        {
        }

        public void EndTimeStepHerbvioryTracking(uint timeStep)
        {
        }
    }
}
