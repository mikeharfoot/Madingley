The Madingley Model
===================

This is the source code and data for the Madingley Model [http://www.madingleymodel.org/](http://www.madingleymodel.org/).

What's New
----------

The code has been developed from the original. It has been split into 5 modules, representing:
  * common data structures
  * configuration
  * environment
  * the model itself
  * output formatting

Other changes include:
  * Configuration files are now in separate folders
  * Previously hard-coded ecological process parameters are now stored in a configuration file
  * More impacts
  * Revised dispersal methods
  * Some unit tests representing small studies, 1 or 4 cells for 1 or 10 years
  * Cross platform library for manipulating netCDF, CSV and TSV files

Installation
------------

Input data comes from either the included netCDF files or **FetchClimate** [http://research.microsoft.com/en-us/projects/fetchclimate/](http://research.microsoft.com/en-us/projects/fetchclimate/) and model output is written to netCDF and TSV files.
This solution includes a set of projects called **SDSLite** to manipulate these file types. It is a subset of **Scientific DataSet** [http://research.microsoft.com/en-us/projects/sds/](http://research.microsoft.com/en-us/projects/sds/) and
[https://sds.codeplex.com/](https://sds.codeplex.com/)

SDSLite requires a platform dependent library available from [http://www.unidata.ucar.edu/software/netcdf/docs/getting_and_building_netcdf.html](http://www.unidata.ucar.edu/software/netcdf/docs/getting_and_building_netcdf.html)

### Windows

For Windows go to [http://www.unidata.ucar.edu/software/netcdf/docs/winbin.html](http://www.unidata.ucar.edu/software/netcdf/docs/winbin.html) and download the version of netCDF4 (without DAP) corresponding to your machine, either 32 or 64 bit. These are
currently: [http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-32.exe](http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-32.exe) or [http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-64.exe](http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-64.exe)
When you install this library select the option to add its location to your system PATH, so that SDSLite can find it.

### Linux

For Linux install pre-built netCDF-C libraries. For example on Ubuntu:

`sudo apt-get install libnetcdf-dev`

### MacOS

TBD.

Running the Model
-----------------

This solution contains sufficient configuration (in the "Model setup" folder) and data (in the "Data" folder) files to run the model, although it does require an internet connection to access FetchClimate to get additional climate data.
It is set to run a single cell for one year. 

The Madingley project is a simple command line application that loads the configuration, environment data, passes them to the model and stores the output. Select this project as the start-up project and run it to run the model.

Configuring the Model
---------------------

This is done via the CSV files in "Model setup".

### FileLocationParameters.csv

This file identifies other file names to use for different aspects of the configuration.

Parameter | Value | Comment
--------- | ----- | -------
Mass Bin Filename | MassBinDefinitions.csv | Only used for output
Environmental Data File | EnvironmentalDataLayers.csv | File containing environment layers (in "Environmental Data Layer List" folder)
Cohort Functional Group Definitions File | CohortFunctionalGroupDefinitions.csv | File containing cohort functional group definitions (in "Ecological Definition Files" folder)
Stock Functional Group Definitions File | StockFunctionalGroupDefinitions.csv | File containing stock functional group definitions (in "Ecological Definition Files" folder)
Ecological parameters file | EcologicalParameters.csv | File containing Ecological parameters (in "Ecological Definition Files" folder)

### SimulationControlParameters.csv

The file specifies most of the changeable information needed by the model to run. It should be modified depending on the experimental purpose of the simulation, along with its counterpart the ‘Scenarios’ file.

Parameter | Value | Comment
------------- | ------------- | -------------
Timestep Units | Month | The temporal resolution at which the model runs; the length of time one round of calculations represents.  Valid timestep options are "day", "month" or "year" - however, the model has been calibrated primary for a timestep of "month".
Length of simulation (years) | 10 | The length of time the simulation represents; the timescale of the model outputs.
Burn-in (years) | 0 | 
Impact duration (years) | 0 | 
Recovery duration (years) | 0 | 
Bottom Latitude | -65 | The southernmost boundary of the grid to be modelled. Input unit is decimal degrees.
Top Latitude | 65 | The northernmost boundary of the grid to be modelled. Input unit is decimal degrees.
Leftmost Longitude | -180 | The westernmost boundary of the grid to be modelled. Input unit is decimal degrees.
Rightmost Longitude | 180 | The easternmost boundary of the grid to be modelled. Input unit is decimal degrees.
Grid Cell Size | 1 | The length of one side of the square cells which will make up the model grid. A smaller value means a higher spatial resolution, and more cells within the grid. For example, a 1x1 degree grid with cell size 0.1 will be a 100 cell grid; with cell size 0.05 it will be a 400 cell grid. Decreasing cell size makes the model considerably more computationally expensive.
Specific Location File | SpecificLocations.csv | File name to use to run model at specific grid cells, leave blank to run for whole grid.
Plankton size threshold | 0.01 | 
Draw Randomly | no | Distributes the initial stock and cohort seeding randomly across the grid, and randomises the order in which cohorts act within a timestep introducing a degree of stochasticity to the simulation. Input as ‘yes’ or ‘no’. Generally left as yes.
Extinction Threshold | 1 | The number of individuals remaining in a cohort for it to be considered extant.
Maximum Number Of Cohorts | 1000 | The maximum number of cohorts allowed within a single cell, after which the model merges the most similar cohorts into single larger approximations. Necessary for computational reasons: increasing the maximum number of cohorts reduces uncertainty and increases the power of the model at the cost of increased computational expense.
Run Cells In Parallel | no | Assigns different cells to different processor cores when running a simulation. Input as ‘yes’ or ’no’.
Run Simulations In Parallel | no | Assigns different simulations to different processor cores when running multiple simulations (either different scenarios or repeats of the same). Input as ‘yes’ or ‘no’. 
Run Single Realm |  | Prevents the model from running both marine and terrestrial biomes in the same simulation. Input as ‘marine’ or ‘terrestrial’ for only marine or terrestrial realm respectively, leave blank to run both realms.
Impact Cell Index |  | 
Dispersal only | no | Runs only the dispersal aspects of the model, ignoring ecological processes. Rarely used for experimentation.
Dispersal only type | advection | Specifies the method of dispersal allowed when ‘Dispersal Only’ is set to ‘yes’. Rarely used.

### Ecological Definition Files/CohortFuctionalGroupDefinitions.csv

Defines the cohort specific functional groups to be initialised in the model.

Column | Comment
------ | -------
DEFINITION_Heterotroph/Autotroph | Defines whether this functional group is heterotrophic or autotrophic - cohorts are all "heterotroph" currently
DEFINITION_Nutrition source | The feeding mode of organisms in this functional group, values can be: "Herbivore", "Omnivore" or "Carnivore"
DEFINITION_Diet | allspecial for filter-feeding baleen whales only, blank for all else.
DEFINITION_Realm | The realm in which the functional group is found, values are "Marine" or "Terrestrial"
DEFINITION_Mobility | What is the mobility of the organisms in each functional group: currently permissible values "planktonic" or "mobile"
DEFINITION_Reproductive strategy | Is the functional group "Iteroparous" or "Semelparous"
DEFINITION_Endo/Ectotherm | Metabolic pathway: "Ectotherm" or "Endotherm"
PROPERTY_Herbivory assimilation | The proportion of plant matter ingested that is assimilated by organisms in each functional group (unitless)
PROPERTY_Carnivory assimilation | The proportion of heterotroph prey biomass ingested that is assimilated by organisms in each functional group (unitless)
PROPERTY_Proportion suitable time active | The proportion of the environmentally suitable time in each timestep that organisms are active (unitless)
PROPERTY_Minimum mass | The minimum mass for any organism in each functional group (g)
PROPERTY_Maximum mass | The maximum mass for any organism in each functional group (g)
PROPERTY_Initial number of GridCellCohorts | The number of cohorts initialised into the model at the start of each simulation
NOTES_group description | A description of what each functional group represents

### Ecological Definition Files/StockFuctionalGroupDefinitions.csv

Defines stock specific functional groups to be initialised in the model.

Column | Comment
------ | -------
DEFINITION_Heterotroph/Autotroph | Defines whether this functional group is heterotrophic or autotrophic - stocks are all "autotroph" currently
DEFINITION_Nutrition source | Where the organisms obtain their nutrition from: currently "photosynthesis" for all stocks
DEFINITION_Diet | Not used
DEFINITION_Realm | The realm in which the functional group is found, values are "Marine" or "Terrestrial"
DEFINITION_Mobility | What is the mobility of the organisms in each functional group: currently permissible values "planktonic" or "sessile"
DEFINITION_Leaf strategy | The leaf strategy for functional groups - current values are "deciduous" or "evergreen" and apply to terrestrial stocks only
PROPERTY_Herbivory assimilation | Not used
PROPERTY_Carnivory assimilation | Not used
PROPERTY_Proportion herbivory | Not used
PROPERTY_Individual mass | The mass of an individual organism in each functional group (currently only applied to phytoplantkton)

### Ecological Definition Files/EcologicalParameters.csv

Parameters for ecological processes.

### Environmental Data Layer List/EnvironmentalDataLayers.csv

Lists the exogenous environmental datasets to be used in the model.

Column | Type | Comment
------ | ---- | -------
Source | "Local"/"FetchClimate" | Data set is a local file or a call to the FetchClimate service
Folder | string or blank | The  folder in which the dataset is stored (relative to the directory in which the model executable is running)
Filename | string or blank | The filename of the dataset
Extension | string or blank | The file-extension of the dataset
Dataset Name | string | The name of the variable inside the data file that is to be used
Filetype | "nc"/"csv" | The file-type of the data set file "nc" for NetCDF, "csv" for comma-separated values
Internal Layer Name | string | The name with which the environmental data will be referred to inside the model
Static | Y | Is the environmental layer to be used as a constant (e.g. an annual average) or does it vary through time? Currently only Y
Resolution | "year"/"month" | The temporal resolution of the data to be imported
Units | string | The units of the environmental variable

Examples:

```
Source,Folder,Filename,Extension,Dataset Name,Filetype,Internal Layer Name,Static,Resolution,Units
Local,,LandSeaMask,.nc,land_sea_mask,nc,LandSeaMask,Y,year,
Local,Land,hanpp_2005,.nc,HANPP,nc,HANPP,Y,year,gC/m2/year
Local,Land,AvailableWaterCapacity,.nc,AWC,nc,AWC,Y,year,mm
Local,Land,NPP,.nc,NPP,nc,LandNPP,Y,month,gC/m2/day
Local,Ocean,averaged_u_50y_top100m_monthly,.nc,uVel,nc,uVel,Y,month,cm/s
Local,Ocean,averaged_v_50y_top100m_monthly,.nc,vVel,nc,vVel,Y,month,cm/s
FetchClimate,,,,temperature,,Temperature,Y,month,degC
FetchClimate,,,,land_dtr,,LandDTR,Y,month,degC
FetchClimate,,,,temperature_ocean,,OceanTemp,Y,month,degC
FetchClimate,,,,precipitation,,Precipitation,Y,month,degC
FetchClimate,,,,frost,,FrostDays,Y,month,degC
```

### Initial Model State Setup/Scenarios.csv

Is one of the files to be edited depending on the purpose of the experiment being run. This is where the scenarios to be simulated by the model are defined, and forms the counterpart to the ‘Ecosystem Model Initialisation’ file. Currently, it is used to define impact scenarios and repeated simulations.

Column | Comment
------ | -------
label | Label this scenario
npp | NPP settings, "no 0.0 0" for no NPP impacts.
temperature | Temperature settings
harvesting | Harvesting settings
simulation number | Number of simulations to run. ‘0’ will mean that scenario is not run, while ‘10’ means the scenario will be repeated ten times. Each repeat will be different due to the randomness built into the initial model seeding (this can be turned off in the Initialisation File). Each repeated simulation has its own unique output file, which is denoted by an index number in the output filename.

Example:

    NI,no 0.0 0,no 0.0,no 0.0,1

### Initial Model State Setup/SpecificLocations.csv

If a file name is specified in SimulationControlParameters.csv for "Specific Location File", this is a list of cell centre coordinates of specific grid cells for which model simulations are to be performed.

Column | Comment
------ | -------
Latitude | The latitude of the cell centre (Decimal degrees)
Longitude | The longitude of the cell centre (Decimal degrees)
