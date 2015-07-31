using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Research.Science.Data;

namespace Madingley.Output
{
    public class MadingleyModelOutputDataSets
    {
        public string Global { get; set; }

        public string[] Cells { get; set; }

        public string Dispersal { get; set; }

        public string Grid { get; set; }

        public string NPP { get; set; }
    }

    /// <summary>
    /// The ecosystem model
    /// </summary>
    internal class MadingleyModelOutput : Madingley.Common.IOutput
    {
        public MadingleyModel model;

        public IList<IEnumerable<Madingley.Common.IProcessTracker>> ProcessTracker { get; private set; }

        public IEnumerable<Madingley.Common.IGlobalProcessTracker> GlobalProcessTracker { get; private set; }

        public IEnumerable<Madingley.Common.ICrossCellProcessTracker> CrossCellProcessTracker { get; private set; }

        public MadingleyModelOutput(
            string outputFilesSuffix,
            Madingley.Common.Environment environment,
            Madingley.Common.Configuration configuration,
            MadingleyModelInitialisation outputSettings,
            Madingley.Common.ModelState modelState)
        {
            var modelInitialisation = Converters.ConvertInitialisation(
                outputSettings,
                configuration,
                environment);

            this.ProcessTracker = new Madingley.Common.IProcessTracker[environment.FocusCells.Count()][];
            for (var cellIndex = 0; cellIndex < environment.FocusCells.Count(); cellIndex++)
            {
                var processTracker = new GEMProcessTracker(cellIndex, this);

                this.ProcessTracker[cellIndex] = new Madingley.Common.IProcessTracker[] { processTracker };
            }

            var globalProcessTracker = new GEMGlobalProcessTracker(this);
            this.GlobalProcessTracker = new Madingley.Common.IGlobalProcessTracker[] { globalProcessTracker };

            var crossCellProcessTracker = new GEMCrossCellProcessTracker(this);
            this.CrossCellProcessTracker = new Madingley.Common.ICrossCellProcessTracker[] { crossCellProcessTracker };

            this.model = new MadingleyModel(
                modelInitialisation,
                outputFilesSuffix,
                configuration.Simulation,
                modelState);
        }

        public void BeginTimestep(int timestep)
        {
            this.model.BeginTimestep(timestep);
        }

        public void EndTimestep(
            int currentTimestep,
            Madingley.Common.ModelState modelState)
        {
            this.model.EndTimestep(currentTimestep, modelState);
        }

        public void SaveTimestep(int currentTimestep)
        {
            this.model.SaveTimestep(currentTimestep);
        }

        public Object EndYear(int year)
        {
            return this.model.EndYear(year);
        }

        public void EndRun()
        {
            this.model.EndRun();
        }
    }
}
