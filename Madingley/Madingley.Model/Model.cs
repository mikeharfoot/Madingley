using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Madingley
{
    public class ProgressReporter : IProgress<double>
    {
        public void Report(double progress)
        {
            Console.WriteLine("Progress {0}%", Math.Round(progress * 100.0, 1));
        }
    }

    public static class Model
    {
        public static IEnumerable<Tuple<Madingley.Common.RunState, ReturnType>> Run<ReturnType>(
            Madingley.Common.RunState state,
            Madingley.Common.Configuration configuration,
            Madingley.Common.Environment environment,
            Func<Madingley.Common.RunState, Madingley.Common.Configuration, Madingley.Common.Environment, Madingley.Common.ModelState, Madingley.Common.IOutput> factory,
            IProgress<double> progress,
            CancellationToken cancellation)
        {
            var startTimeStep = state != null ? (uint)state.ModelState.TimestepsComplete : (uint)0;

            var modelState = state != null ? state.ModelState : null;

            var madingleyModel = new MadingleyModel(
                modelState,
                configuration,
                environment);

            var beginRunModelStateData = madingleyModel.CreateModelStateData(startTimeStep);

            var output = factory.Invoke(state, configuration, environment, beginRunModelStateData);

            // Run the simulation
            return madingleyModel.Initialise<ReturnType>(startTimeStep, output, progress, cancellation);
        }

        public static void RunTraditional(
            Madingley.Common.Configuration configuration,
            Madingley.Common.Environment environment,
            Func<Madingley.Common.RunState, Madingley.Common.Configuration, Madingley.Common.Environment, Madingley.Common.ModelState, Madingley.Common.IOutput> factory)
        {
            var progress = new ProgressReporter();
            var cancellationToken = CancellationToken.None;

            Run<Object>(null, configuration, environment, factory, progress, cancellationToken).Last();
        }
    }
}
