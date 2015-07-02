using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Research.Science.Data;


namespace Madingley
{
    /// <summary>
    /// Initialization information for Madingley model simulations
    /// </summary>
    public class MadingleyModelInitialisation
    {
        /// <summary>
        /// The size of cells to be used in the model grid
        /// </summary>
        private double _CellSize;
        /// <summary>
        /// Get and set the size of cells to be used in the model grid
        /// </summary>
        public double CellSize
        {
            get { return _CellSize; }
            set { _CellSize = value; }
        }

        /// <summary>
        /// The lowest extent of the model grid in degrees
        /// </summary>
        private float _BottomLatitude;
        /// <summary>
        /// Get and set the lowest extent of the model grid in degrees
        /// </summary>
        public float BottomLatitude
        {
            get { return _BottomLatitude; }
            set { _BottomLatitude = value; }
        }

        /// <summary>
        /// The uppermost extent of the model grid in degrees
        /// </summary>
        private float _TopLatitude;
        /// <summary>
        /// Get and set the uppermost extent of the model grid in degrees
        /// </summary>
        public float TopLatitude
        {
            get { return _TopLatitude; }
            set { _TopLatitude = value; }
        }

        /// <summary>
        /// The leftmost extent of the model grid in degrees
        /// </summary>
        private float _LeftmostLongitude;
        /// <summary>
        /// Get and set the leftmost extent of the model grid in degrees
        /// </summary>
        public float LeftmostLongitude
        {
            get { return _LeftmostLongitude; }
            set { _LeftmostLongitude = value; }
        }

        /// <summary>
        /// The rightmost extent of the model grid in degrees
        /// </summary>
        private float _RightmostLongitude;
        /// <summary>
        /// Get and set the rightmost extent of the model grid in degrees
        /// </summary>
        public float RightmostLongitude
        {
            get { return _RightmostLongitude; }
            set { _RightmostLongitude = value; }
        }

        /// <summary>
        /// The environmental layers for use in the model
        /// </summary>
        private SortedList<string, EnviroData> _EnviroStack = new SortedList<string, EnviroData>();
        /// <summary>
        /// Get and set the environmental layers for use in the model
        /// </summary>
        public SortedList<string, EnviroData> EnviroStack
        {
            get { return _EnviroStack; }
            set { _EnviroStack = value; }
        }

        /// <summary>
        /// The string values for the units of each environmental data layer
        /// </summary>
        private SortedList<string, string> _Units = new SortedList<string, string>();
        /// <summary>
        /// Get and set the unit strings
        /// </summary>
        public SortedList<string, string> Units
        {
            get { return _Units; }
            set { _Units = value; }
        }

        /// <summary>
        /// Pairs of longitude and latitude indices for all active cells in the model grid
        /// </summary>
        private List<uint[]> _CellList;
        public List<uint[]> CellList
        {
            get { return _CellList; }
            set { _CellList = value; }
        }

        //Indicates if specific locations have been specified
        private Boolean _SpecificLocations = false;

        public Boolean SpecificLocations
        {
            get { return _SpecificLocations; }
            set { _SpecificLocations = value; }
        }

#if true
        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="environmentDataRoot">The path to folder which contains the data inputs</param>
        /// <param name="inputPath">The path to folder which contains the model inputs</param>
        public MadingleyModelInitialisation(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath, string environmentDataRoot, string inputPath)
        {
            // Write to console
            Console.WriteLine("Initializing model...\n");

            // Read the intialisation files and copy them to the output directory
            ReadAndCopyInitialisationFiles(simulationInitialisationFilename, definitionsFilename, outputsFilename, outputPath, environmentDataRoot, inputPath);
        }
#else
        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        public MadingleyModelInitialisation(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath)
        {
            // Write to console
            Console.WriteLine("Initializing model...\n");

            // Initialize the mass bins to be used during the model run
            _ModelMassBins = new MassBinsHandler();

            // Read the intialisation files and copy them to the output directory
            ReadAndCopyInitialisationFiles(simulationInitialisationFilename, definitionsFilename, outputsFilename, outputPath);

            // Copy parameter values to an output file
            //Don't do this now as the parameter values are read in from file and this file is copied to the output directory
            //CopyParameterValues(outputPath);



        }
#endif

#if true
        /// <summary>
        /// Reads in all initialisation files and copies them to the output directory for future reference
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="environmentDataRoot">The path to folder which contains the data inputs</param>
        /// <param name="inputPath">The path to folder which contains the model inputs</param>
        /// <todo>Need to adjust this file to deal with incorrect inputs, extra columns etc by throwing an error</todo>
        /// <todo>Also need to strip leading spaces</todo>
        public void ReadAndCopyInitialisationFiles(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath, string environmentDataRoot, string inputPath)
        {
            // Construct file names
            string SimulationFileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, simulationInitialisationFilename) + "&openMode=readOnly";
            string DefinitionsFileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, definitionsFilename) + "&openMode=readOnly";
#else
        /// <summary>
        /// Reads in all initialisation files and copies them to the output directory for future reference
        /// </summary>
        /// <param name="initialisationFile">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        /// <todo>Need to adjust this file to deal with incorrect inputs, extra columns etc by throwing an error</todo>
        /// <todo>Also need to strip leading spaces</todo>
        public void ReadAndCopyInitialisationFiles(string simulationInitialisationFilename, string definitionsFilename, string outputsFilename, string outputPath)
        {
            // Construct file names
            string SimulationFileString = "msds:csv?file=input/Model setup/" + simulationInitialisationFilename + "&openMode=readOnly";
            string DefinitionsFileString = "msds:csv?file=input/Model setup/" + definitionsFilename + "&openMode=readOnly";
            string OutputsFileString = "msds:csv?file=input/Model setup/" + outputsFilename + "&openMode=readOnly";

            // Copy the initialisation files to the output directory
            System.IO.File.Copy("input/Model setup/" + simulationInitialisationFilename, outputPath + simulationInitialisationFilename, true);
            System.IO.File.Copy("input/Model setup/" + definitionsFilename, outputPath + definitionsFilename, true);
            System.IO.File.Copy("input/Model setup/" + outputsFilename, outputPath + outputsFilename, true);
#endif

            // Read in the simulation data
            DataSet InternalData = DataSet.Open(SimulationFileString);

            // Get the names of parameters in the initialization file
            var VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            var VarValues = InternalData.Variables[0].GetData();

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "grid cell size":
                        _CellSize = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "bottom latitude":
                        _BottomLatitude = Convert.ToSingle(VarValues.GetValue(row));
                        break;
                    case "top latitude":
                        _TopLatitude = Convert.ToSingle(VarValues.GetValue(row));
                        break;
                    case "leftmost longitude":
                        _LeftmostLongitude = Convert.ToSingle(VarValues.GetValue(row));
                        break;
                    case "rightmost longitude":
                        _RightmostLongitude = Convert.ToSingle(VarValues.GetValue(row));
                        break;
                    case "specific location file":
                        if (VarValues.GetValue(row).ToString() != "")
                        {
                            _SpecificLocations = true;
                            // Copy the initialisation file to the output directory
#if true
                            ReadSpecificLocations(VarValues.GetValue(row).ToString(), outputPath, inputPath);
#else
                            System.IO.File.Copy("input/Model setup/Initial Model State Setup/" + _InitialisationFileStrings["Locations"], outputPath + _InitialisationFileStrings["Locations"], true);
                            ReadSpecificLocations(_InitialisationFileStrings["Locations"], outputPath);
#endif
                        }
                        break;
                }
            }


            InternalData.Dispose();

            // Read in the definitions data
            InternalData = DataSet.Open(DefinitionsFileString);

            // Get the names of parameters in the initialization file
            VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            VarValues = InternalData.Variables[0].GetData();

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "environmental data file":
                        // Read environmental data layers
#if true
                        this.ReadEnvironmentalLayers(VarValues.GetValue(row).ToString(), outputPath, environmentDataRoot, inputPath);
#else
                        this.ReadEnvironmentalLayers(VarValues.GetValue(row).ToString(), outputPath);
#endif
                        break;
                }
            }

            InternalData.Dispose();
        }

#if true
        /// <summary>
        /// Read in the specified locations in which to run the model
        /// </summary>
        /// <param name="specificLocationsFile">The name of the file with specific locations information</param>
        /// <param name="outputPath">The path to the output folder in which to copy the specific locations file</param>
        public void ReadSpecificLocations(string specificLocationsFile, string outputPath, string inputPath)
#else
        /// <summary>
        /// Read in the specified locations in which to run the model
        /// </summary>
        /// <param name="specificLocationsFile">The name of the file with specific locations information</param>
        /// <param name="outputPath">The path to the output folder in which to copy the specific locations file</param>
        public void ReadSpecificLocations(string specificLocationsFile, string outputPath)
#endif
        {
            _CellList = new List<uint[]>();

            List<double> LatitudeList = new List<double>();
            List<double> LongitudeList = new List<double>();

            Console.WriteLine("Reading in specific location data");
            Console.WriteLine("");

            // construct file name
#if true
            string FileString = "msds:csv?file=" + System.IO.Path.Combine(inputPath, "Initial Model State Setup", specificLocationsFile) + "&openMode=readOnly";
#else
            string FileString = "msds:csv?file=input/Model setup/Initial Model State Setup/" + specificLocationsFile + "&openMode=readOnly";
#endif

            // Read in the data
            DataSet InternalData = DataSet.Open(FileString);

            foreach (Variable v in InternalData.Variables)
            {
                //Get the name of the variable currently referenced in the dataset
                string HeaderName = v.Name;
                //Copy the values for this variable into an array
                var TempValues = v.GetData();

                switch (HeaderName.ToLower())
                {
                    // Add the latitude and longitude values to the appropriate list
                    case "latitude":
                        for (int ii = 0; ii < TempValues.Length; ii++) LatitudeList.Add(Convert.ToDouble(TempValues.GetValue(ii).ToString()));
                        break;
                    case "longitude":
                        for (int ii = 0; ii < TempValues.Length; ii++) LongitudeList.Add(Convert.ToDouble(TempValues.GetValue(ii).ToString()));
                        break;
                    default:
                        Console.WriteLine("Variable defined in the specific location file but not processed: ", HeaderName);
                        break;
                }
            }

            // Loop over cells defined in the specific locations file
            for (int ii = 0; ii < LatitudeList.Count; ii++)
            {
                // Define a vector to hold the longitude and latitude index for this cell
                uint[] cellIndices = new uint[2];

                // Get the longitude and latitude indices for the current grid cell
                cellIndices[0] = (uint)Math.Floor((LatitudeList.ElementAt(ii) - BottomLatitude) / CellSize);
                cellIndices[1] = (uint)Math.Floor((LongitudeList.ElementAt(ii) - LeftmostLongitude) / CellSize);

                // Add these indices to the list of active cells
                _CellList.Add(cellIndices);
            }


        }

#if true
        /// <summary>
        /// Reads the environmental layers listed in the specified file containing a list of environmental layers
        /// </summary>
        /// <param name="environmentalLayerFile">The name of the file containing the list of environmental layers</param>
        /// <param name="environmentDataRoot">The path to folder which contains the data inputs</param>
        /// <param name="inputPath">The path to folder which contains the model inputs</param>
        public void ReadEnvironmentalLayers(string environmentalLayerFile, string outputPath, string environmentDataRoot, string inputPath)
#else
        /// <summary>
        /// Reads the environmental layers listed in the specified file containing a list of environmental layers
        /// </summary>
        /// <param name="environmentalLayerFile">The name of the file containing the list of environmental layers</param>
        /// <param name="outputPath">The path to folder in which outputs will be stored</param>
        public void ReadEnvironmentalLayers(string environmentalLayerFile, string outputPath)
#endif
        {
            Console.WriteLine("Reading in environmental data:");

            // Declare lists to hold the information required to read the environmental layers
            List<string> Sources = new List<string>();
            List<string> Folders = new List<string>();
            List<string> Filenames = new List<string>();
            List<string> DatasetNames = new List<string>();
            List<string> FileTypes = new List<string>();
            List<string> LayerName = new List<string>();
            List<string> StaticLayer = new List<string>();
            List<string> Extensions = new List<string>();
            List<string> Resolutions = new List<string>();
            List<string> MethodUnits = new List<string>();

            // Variable to store the file name of the environmental data files
            string TempFilename;

#if true
            var inputEnvironmentalLayerFileName = System.IO.Path.Combine(inputPath, "Environmental Data Layer List", environmentalLayerFile);

            // Construct the full URI for the file  containing the list of environmental layers
            string FileString = "msds:csv?file=" + inputEnvironmentalLayerFileName + "&openMode=readOnly";

            StreamReader r_env = new StreamReader(inputEnvironmentalLayerFileName);
#else
            // Construct the full URI for the file  containing the list of environmental layers
            string FileString = "msds:csv?file=input/Model setup/Environmental data layer list/" + environmentalLayerFile + "&openMode=readOnly";

            //Copy the file containing the list of environmental layers to the output directory
            System.IO.File.Copy("input/Model setup/Environmental data layer list/" + environmentalLayerFile, outputPath + environmentalLayerFile, true);

            StreamReader r_env = new StreamReader("input/Model setup/Environmental data layer list/" + environmentalLayerFile);
#endif
            string l;
            char[] comma = ",".ToCharArray();

            string[] f;
            int col;
            // Read in the data
            DataSet InternalData = DataSet.Open(FileString);
            l = r_env.ReadLine();
            while (!r_env.EndOfStream)
            {
                l = r_env.ReadLine();
                // Split fields by commas
                f = l.Split(comma);
                //zero the column index
                col = 0;
                // Lists of the different fields
                Sources.Add(f[col++]);
                Folders.Add(f[col++]);
                Filenames.Add(f[col++]);
                Extensions.Add(f[col++]);
                DatasetNames.Add(f[col++]);
                FileTypes.Add(f[col++]);
                LayerName.Add(f[col++]);
                StaticLayer.Add(f[col++]);
                Resolutions.Add(f[col++]);
                MethodUnits.Add(f[col++]);
            }


            for (int ii = 0; ii < MethodUnits.Count; ii++)
            {
                Units.Add(LayerName[ii], MethodUnits[ii]);
            }

            // Check that there are the same number of values for all parameters
            Debug.Assert(Folders.Count() == Filenames.Count() && Filenames.Count() == DatasetNames.Count() && DatasetNames.Count() == FileTypes.Count() && FileTypes.Count() == LayerName.Count(),
                "Error in Environmental Data Layer import lists - unequal number of filenames, dataset names, filetypes and datalayer names");

            // Loop over parameter values
            for (int ii = 0; ii < Filenames.Count(); ii++)
            {
                Console.Write("\r{0} Variable {1} of {2}: {3}\n", Sources[ii], ii + 1, Filenames.Count, Filenames[ii]);
                // If the layers are not static, then suffix the file name with '1' - not currently implemented
                if (StaticLayer[ii].ToLower().Equals("n"))
                {
                    Debug.Fail("This option is currently not supported");
                    Filenames[ii] = Filenames[ii] + "1";
                }
                if (Sources[ii].ToLower().Equals("local"))
                {
                    // For layers where the file format is ESRI ASCII grid, the dataset name is the same as the file name
                    if (FileTypes[ii].ToLower().Equals("esriasciigrid"))
                    {
                        DatasetNames[ii] = Filenames[ii];
                    }
                    // Generate the appropriate file name for the environmental data layer
#if true
                    TempFilename = System.IO.Path.Combine(environmentDataRoot, Folders[ii], Filenames[ii]);

                    Filenames[ii] = System.IO.Path.ChangeExtension(TempFilename, Extensions[ii]);
#else
                    if (Folders[ii].ToLower().Equals("input"))
                    {
                        TempFilename = "input/Data/" + Filenames[ii];
                    }
                    else
                    {
                        TempFilename = Folders[ii] + "/" + Filenames[ii];
                    }
                    Filenames[ii] = TempFilename + Extensions[ii];
#endif
                    // Read in and store the environmental data
                    EnviroStack.Add(LayerName[ii], new EnviroData(Filenames[ii], DatasetNames[ii], FileTypes[ii], Resolutions[ii], MethodUnits[ii]));
                }
                else if (Sources[ii].ToLower().Equals("fetchclimate"))
                {

                    if (!EnviroStack.ContainsKey(LayerName[ii]))
                        if (_SpecificLocations)
                        {
                            EnviroStack.Add(LayerName[ii], new EnviroData(DatasetNames[ii], Resolutions[ii], (double)BottomLatitude, (double)LeftmostLongitude, (double)TopLatitude, (double)RightmostLongitude, (double)CellSize, _CellList, EnvironmentalDataSource.ANY));
                        }
                        else
                        {
                            EnviroStack.Add(LayerName[ii], new EnviroData(DatasetNames[ii], Resolutions[ii], (double)BottomLatitude, (double)LeftmostLongitude, (double)TopLatitude, (double)RightmostLongitude, (double)CellSize, EnvironmentalDataSource.ANY));
                        }

                }
            }
            Console.WriteLine("\n\n");
        }

    }



}
