using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Research.Science.Data;

using System.IO;

namespace Madingley
{   
    /// <summary>
    /// The ecosystem model
    /// </summary>
    public class MadingleyModel
    {
        /// <summary>
        /// A list of environmental data layers
        /// </summary>
        private SortedList<string, EnviroData> EnviroStack = new SortedList<string, EnviroData>();

        /// <summary>
        /// An instance of ModelGrid to hold the grid to be used in this model
        /// </summary>
#if true
        public ModelGrid EcosystemModelGrid;
#else
        private ModelGrid EcosystemModelGrid;
#endif

        /// <summary>
        /// The lowest latitude for the model grid
        /// </summary>
        private float BottomLatitude;
        /// <summary>
        /// The upper latitude for the model grid
        /// </summary>
        private float TopLatitude;
        /// <summary>
        /// The left-most longitude for the model grid
        /// </summary>
        private float LeftmostLongitude;
        /// <summary>
        /// The right-most longitude for the model grid
        /// </summary>
        private float RightmostLongitude;
        
        /// <summary>
        /// The size of the grid cells in degrees
        /// </summary>
        private float CellSize;

        /// <summary>
        /// Pairs of longitude and latitude indices for all active cells in the model grid
        /// </summary>
#if true
        public List<uint[]> _CellList;
#else
        private List<uint[]> _CellList;
#endif

        /// <summary>
        /// Whether the model will be run for specific locations, instead of for the whole model grid
        /// </summary>
        public Boolean SpecificLocations;

        /// <summary>
        /// A sorted list of strings for environmental data units
        /// </summary>
        SortedList<string, string> EnvironmentalDataUnits = new SortedList<string, string>();

        /// <summary>
        /// Initializes the ecosystem model
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario being run</param>
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
        /// <param name="globalModelTimeStepUnit">The time step unit used in the model</param>
        /// <param name="simulation">The index of the simulation being run</param>
#if true
        public MadingleyModel(MadingleyModelInitialisation initialisation)
        {
            // Assign the properties for this model run
            AssignModelRunProperties(initialisation);

            // Set up the model grid
            SetUpModelGrid(initialisation);
        }
#else
        public MadingleyModel(MadingleyModelInitialisation initialisation, ScenarioParameterInitialisation scenarioParameters, int scenarioIndex,
            string outputFilesSuffix, string globalModelTimeStepUnit, int simulation)
        {         
            // Assign the properties for this model run
            AssignModelRunProperties(initialisation, scenarioParameters, scenarioIndex, outputFilesSuffix);

            // Set up list of global diagnostics
            SetUpGlobalDiagnosticsList();

            // Set up the model grid
            SetUpModelGrid(initialisation, scenarioParameters, scenarioIndex, simulation);
        }
#endif

        /// <summary>
        /// Assigns the properties of the current model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario that this model is to run</param>
        /// <param name="outputFilesSuffix">The suffix to be applied to all outputs from this model run</param>
#if true
        public void AssignModelRunProperties(MadingleyModelInitialisation initialisation)

#else
        public void AssignModelRunProperties(MadingleyModelInitialisation initialisation, 
            ScenarioParameterInitialisation scenarioParameters, int scenarioIndex,
            string outputFilesSuffix)
#endif
        {
            // Assign the properties of this model run from the same properties in the specified model initialisation
            CellSize = (float)initialisation.CellSize;
            _CellList = initialisation.CellList;
            BottomLatitude = initialisation.BottomLatitude;
            TopLatitude = initialisation.TopLatitude;
            LeftmostLongitude = initialisation.LeftmostLongitude;
            RightmostLongitude = initialisation.RightmostLongitude;
            EnviroStack = initialisation.EnviroStack;
            SpecificLocations = initialisation.SpecificLocations;
        }

        /// <summary>
        /// Sets up the model grid within a Madingley model run
        /// </summary>
        /// <param name="initialisation">An instance of the model initialisation class</param> 
        /// <param name="scenarioParameters">The parameters for the scenarios to run</param>
        /// <param name="scenarioIndex">The index of the scenario that this model is to run</param>
#if true
        public void SetUpModelGrid(MadingleyModelInitialisation initialisation)

#else
        public void SetUpModelGrid(MadingleyModelInitialisation initialisation,
            ScenarioParameterInitialisation scenarioParameters, int scenarioIndex, int simulation)
#endif
        {
            // If the intialisation file contains a column pointing to another file of specific locations, and if this column is not blank then read the 
            // file indicated
            if (SpecificLocations)
            {
                // Set up the model grid using these locations
#if true
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack,
                    SpecificLocations);
#else
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations,RunGridCellsInParallel);
#endif

            }
            else
            {
                _CellList = new List<uint[]>();
                //Switched order so we create cell list first then initialise cells using list rather than grid.

                uint NumLatCells = (uint)((TopLatitude - BottomLatitude) / CellSize);
                uint NumLonCells = (uint)((RightmostLongitude - LeftmostLongitude) / CellSize);

                // Loop over all cells in the model
                for (uint ii = 0; ii < NumLatCells; ii += 1)
                {
                    for (uint jj = 0; jj < NumLonCells; jj += 1)
                    {
                        // Define a vector to hold the pair of latitude and longitude indices for this grid cell
                        uint[] cellIndices = new uint[2];

                        // Add the latitude and longitude indices to this vector
                        cellIndices[0] = ii;
                        cellIndices[1] = jj;

                        // Add the vector to the list of all active grid cells
                        _CellList.Add(cellIndices);

                    }
                }

                // Set up a full model grid (i.e. not for specific locations)
                // Set up the model grid using these locations
#if true
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack,
                    SpecificLocations);
#else
                EcosystemModelGrid = new ModelGrid(BottomLatitude, LeftmostLongitude, TopLatitude, RightmostLongitude,
                    CellSize, CellSize, _CellList, EnviroStack, CohortFunctionalGroupDefinitions, StockFunctionalGroupDefinitions,
                    GlobalDiagnosticVariables, initialisation.TrackProcesses, SpecificLocations, RunGridCellsInParallel);
#endif
            }
        }

    }

}