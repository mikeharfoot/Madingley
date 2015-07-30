﻿using System;

namespace Madingley
{

    /// <summary>
    /// The entry point for the model
    /// <todoM>Write model output to an output file</todoM>
    /// </summary>
    class Program
    {
        /// <summary>
        /// Starts a model run or set of model runs
        /// </summary>
        static void Main()
        {
            // Write out model details to the console
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Madingley model v. 0.3333333\n");
            Console.ForegroundColor = ConsoleColor.White;

            var modelSetupRoot = "../../../Model setup";
            var environmentDataRoot = "../../../Data/Original";

            var beginTime = DateTime.Now;

            var configuration = Madingley.Configuration.Loader.Load(modelSetupRoot);
            var environment = Madingley.Environment.Loader.Load(environmentDataRoot, modelSetupRoot);

            var modelTime = DateTime.Now;

            Madingley.Model.RunTraditional(configuration, environment, Madingley.Output.Factory.Create);

            var endTime = DateTime.Now;
            var modelInterval = endTime - modelTime;

            Console.WriteLine("Model run finished");
            Console.WriteLine("Total elapsed time was {0} seconds", (endTime - beginTime).TotalSeconds);
            Console.WriteLine("Model setup time was {0} seconds", (modelTime - beginTime).TotalSeconds);
            Console.WriteLine("Model run time was {0} seconds", (endTime - modelTime).TotalSeconds);

            Console.ReadKey();
        }
    }
}