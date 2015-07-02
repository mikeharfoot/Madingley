using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Research.Science.Data;

namespace Madingley.Output
{
    public class MadingleyModelOutputDataSets
    {
        public DataSet Global { get; set; }

        public DataSet[] Cells { get; set; }

        public DataSet Dispersal { get; set; }

        public DataSet Grid { get; set; }

        public DataSet NPP { get; set; }
    }

    /// <summary>
    /// The ecosystem model
    /// </summary>
    internal class MadingleyModelOutput : Madingley.Common.IOutput
    {
        public MadingleyModel model;

        public IList<IEnumerable<Madingley.Common.IProcessTracker>> processTrackers;

        public IEnumerable<Madingley.Common.IGlobalProcessTracker> globalProcessTrackers;

        public IEnumerable<Madingley.Common.ICrossCellProcessTracker> crossCellProcessTrackers;

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

            this.processTrackers = new Madingley.Common.IProcessTracker[environment.FocusCells.Count()][];
            for (var cellIndex = 0; cellIndex < environment.FocusCells.Count(); cellIndex++)
            {
                var processTracker = new GEMProcessTracker(cellIndex, this);

                this.processTrackers[cellIndex] = new Madingley.Common.IProcessTracker[] { processTracker };
            }

            var globalProcessTracker = new GEMGlobalProcessTracker(this);
            this.globalProcessTrackers = new Madingley.Common.IGlobalProcessTracker[] { globalProcessTracker };

            var crossCellProcessTracker = new GEMCrossCellProcessTracker(this);
            this.crossCellProcessTrackers = new Madingley.Common.ICrossCellProcessTracker[] { crossCellProcessTracker };

            this.model = new MadingleyModel(
                modelInitialisation,
                outputFilesSuffix,
                configuration.Simulation,
                modelState);
        }

        public IList<IEnumerable<Madingley.Common.IProcessTracker>> ProcessTracker
        {
            get
            {
                return this.processTrackers;
            }
        }

        public IEnumerable<Madingley.Common.IGlobalProcessTracker> GlobalProcessTracker
        {
            get
            {
                return this.globalProcessTrackers;
            }
        }

        public IEnumerable<Madingley.Common.ICrossCellProcessTracker> CrossCellProcessTracker
        {
            get
            {
                return this.crossCellProcessTrackers;
            }
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
