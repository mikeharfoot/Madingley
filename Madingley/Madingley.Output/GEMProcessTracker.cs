using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void RecordNewCohort(uint latIndex, uint lonIndex, uint timeStep, double offspringCohortAbundance, double parentCohortAdultMass, int functionalGroup, uint[] parentCohortIDs, uint offspringCohortID)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordNewCohort(latIndex, lonIndex, timeStep, offspringCohortAbundance, parentCohortAdultMass, functionalGroup, parentCohortIDs.ToList(), offspringCohortID);
            }
        }

        public void TrackMaturity(uint latIndex, uint lonIndex, uint timeStep, uint birthTimestep, double juvenileMass, double adultMass, int functionalGroup)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackMaturity(latIndex, lonIndex, timeStep, birthTimestep, juvenileMass, adultMass, functionalGroup);
            }
        }

        public void TrackPredationTrophicFlow(uint latIndex, uint lonIndex, int fromFunctionalGroup, int toFunctionalGroup, double massEaten, double predatorBodyMass, double preyBodyMass, bool marineCell)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackPredationTrophicFlow(latIndex, lonIndex, fromFunctionalGroup, toFunctionalGroup, this.output.model.ModelInitialisation.CohortFunctionalGroupDefinitions, massEaten, predatorBodyMass, preyBodyMass, this.output.model.ModelInitialisation, marineCell);
            }
        }

        public void TrackHerbivoryTrophicFlow(uint latIndex, uint lonIndex, int toFunctionalGroup, double massEaten, double predatorBodyMass, bool marineCell)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackHerbivoryTrophicFlow(latIndex, lonIndex, toFunctionalGroup, this.output.model.ModelInitialisation.CohortFunctionalGroupDefinitions, massEaten, predatorBodyMass, this.output.model.ModelInitialisation, marineCell);
            }
        }

        public void TrackPrimaryProductionTrophicFlow(uint latIndex, uint lonIndex, double massEaten)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackPrimaryProductionTrophicFlow(latIndex, lonIndex, massEaten);
            }
        }

        public void TrackTimestepGrowth(uint latIndex, uint lonIndex, uint timeStep, double currentBodyMass, int functionalGroup, double netGrowth, double metabolism, double predation, double herbivory)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackTimestepGrowth(latIndex, lonIndex, timeStep, currentBodyMass, functionalGroup, netGrowth, metabolism, predation, herbivory);
            }
        }

        public void RecordPredationMassFlow(uint currentTimeStep, double preyBodyMass, double predatorBodyMass, double massFlow)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordPredationMassFlow(currentTimeStep, preyBodyMass, predatorBodyMass, massFlow);
            }
        }

        public void RecordHerbivoryMassFlow(uint currentTimeStep, double herbivoreBodyMass, double massFlow)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordHerbivoryMassFlow(currentTimeStep, herbivoreBodyMass, massFlow);
            }
        }

        public void RecordMortality(uint latIndex, uint lonIndex, uint birthTimeStep, uint timeStep, double currentMass, double adultMass, uint functionalGroup, uint cohortID, double numberDied, string mortalitySource)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordMortality(latIndex, lonIndex, birthTimeStep, timeStep, currentMass, adultMass, functionalGroup, cohortID, numberDied, mortalitySource);
            }
        }

        public void OutputMortalityProfile(uint cohortID)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.OutputMortalityProfile(cohortID);
            }
        }

        public void RecordExtinction(uint latIndex, uint lonIndex, uint currentTimeStep, bool merged, uint[] cohortIDs)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.RecordExtinction(latIndex, lonIndex, currentTimeStep, merged, cohortIDs.ToList());
            }
        }

        public void TrackTimestepMetabolism(uint latIndex, uint lonIndex, uint timeStep, double currentBodyMass, int functionalGroup, double temperature, double metabolicLoss)
        {
            var p = this.GetProcessTracker();

            if (p != null && p.TrackProcesses)
            {
                p.TrackTimestepMetabolism(latIndex, lonIndex, timeStep, currentBodyMass, functionalGroup, temperature, metabolicLoss);
            }
        }

        public ProcessTracker GetProcessTracker()
        {
            return this.output.model.GetProcessTracker(this.cellIndex);
        }
    }
}
