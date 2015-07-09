using System.Collections.Generic;
using System.Linq;

namespace Madingley.Output
{
    internal class GEMProcessTracker : Madingley.Common.IProcessTracker
    {
        private int cellIndex;

        private MadingleyModelOutput output;

        public GEMProcessTracker(int cellIndex, MadingleyModelOutput output)
        {
            this.cellIndex = cellIndex;
            this.output = output;
        }

        public void RecordNewCohort(int latitudeIndex, int longitudeIndex, int timeStep, double offspringCohortAbundance, double parentCohortAdultMass, int functionalGroup, IEnumerable<int> parentCohortIDs, int offspringCohortID)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordNewCohort((uint)latitudeIndex, (uint)longitudeIndex, (uint)timeStep, offspringCohortAbundance, parentCohortAdultMass, functionalGroup, parentCohortIDs.Select(id => (uint)id).ToList(), (uint)offspringCohortID);
            }
        }

        public void TrackMaturity(int latitudeIndex, int longitudeIndex, int timeStep, int birthTimestep, double juvenileMass, double adultMass, int functionalGroup)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackMaturity((uint)latitudeIndex, (uint)longitudeIndex, (uint)timeStep, (uint)birthTimestep, juvenileMass, adultMass, functionalGroup);
            }
        }

        public void TrackPredationTrophicFlow(int latitudeIndex, int longitudeIndex, int fromFunctionalGroup, int toFunctionalGroup, double massEaten, double predatorBodyMass, double preyBodyMass, bool marineCell)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackPredationTrophicFlow((uint)latitudeIndex, (uint)longitudeIndex, fromFunctionalGroup, toFunctionalGroup, this.output.model.ModelInitialisation.CohortFunctionalGroupDefinitions, massEaten, predatorBodyMass, preyBodyMass, this.output.model.ModelInitialisation, marineCell);
            }
        }

        public void TrackHerbivoryTrophicFlow(int latitudeIndex, int longitudeIndex, int toFunctionalGroup, double massEaten, double predatorBodyMass, bool marineCell)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackHerbivoryTrophicFlow((uint)latitudeIndex, (uint)longitudeIndex, toFunctionalGroup, this.output.model.ModelInitialisation.CohortFunctionalGroupDefinitions, massEaten, predatorBodyMass, this.output.model.ModelInitialisation, marineCell);
            }
        }

        public void TrackPrimaryProductionTrophicFlow(int latitudeIndex, int longitudeIndex, double massEaten)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackPrimaryProductionTrophicFlow((uint)latitudeIndex, (uint)longitudeIndex, massEaten);
            }
        }

        public void TrackTimestepGrowth(int latitudeIndex, int longitudeIndex, int timeStep, double currentBodyMass, int functionalGroup, double netGrowth, double metabolism, double predation, double herbivory)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackTimestepGrowth((uint)latitudeIndex, (uint)longitudeIndex, (uint)timeStep, currentBodyMass, functionalGroup, netGrowth, metabolism, predation, herbivory);
            }
        }

        public void RecordPredationMassFlow(int currentTimeStep, double preyBodyMass, double predatorBodyMass, double massFlow)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordPredationMassFlow((uint)currentTimeStep, preyBodyMass, predatorBodyMass, massFlow);
            }
        }

        public void RecordHerbivoryMassFlow(int currentTimeStep, double herbivoreBodyMass, double massFlow)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordHerbivoryMassFlow((uint)currentTimeStep, herbivoreBodyMass, massFlow);
            }
        }

        public void RecordMortality(int latitudeIndex, int longitudeIndex, int birthTimeStep, int timeStep, double currentMass, double adultMass, int functionalGroup, int cohortID, double numberDied, string mortalitySource)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordMortality((uint)latitudeIndex, (uint)longitudeIndex, (uint)birthTimeStep, (uint)timeStep, currentMass, adultMass, (uint)functionalGroup, (uint)cohortID, numberDied, mortalitySource);
            }
        }

        public void OutputMortalityProfile(int cohortID)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.OutputMortalityProfile((uint)cohortID);
            }
        }

        public void RecordExtinction(int latitudeIndex, int longitudeIndex, int currentTimeStep, bool merged, IEnumerable<int> cohortIDs)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordExtinction((uint)latitudeIndex, (uint)longitudeIndex, (uint)currentTimeStep, merged, cohortIDs.Select(id => (uint)id).ToList());
            }
        }

        public void TrackTimestepMetabolism(int latitudeIndex, int longitudeIndex, int timeStep, double currentBodyMass, int functionalGroup, double temperature, double metabolicLoss)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackTimestepMetabolism((uint)latitudeIndex, (uint)longitudeIndex, (uint)timeStep, currentBodyMass, functionalGroup, temperature, metabolicLoss);
            }
        }

        public ProcessTracker GetProcessTracker()
        {
            return this.output.model.GetProcessTracker(this.cellIndex);
        }
    }
}
