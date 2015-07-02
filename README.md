# Madingley
The Madingley Model
===================

This is the source code and data for the Madingley Model [link](http://www.madingleymodel.org/).

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

Input data comes from either the included netCDF files or **FetchClimate** [link](http://research.microsoft.com/en-us/projects/fetchclimate/) and model output is written to netCDF and TSV files.
This solution includes a set of projects called **SDSLite** to manipulate these file types. It is a subset of **Scientific DataSet** [link](http://research.microsoft.com/en-us/projects/sds/) and
[link](https://sds.codeplex.com/)

SDSLite requires a platform dependent library available from [link](http://www.unidata.ucar.edu/software/netcdf/docs/getting_and_building_netcdf.html)

### Windows

For Windows go to [link](http://www.unidata.ucar.edu/software/netcdf/docs/winbin.html) and download the version of netCDF4 (without DAP) corresponding to your machine, either 32 or 64 bit. These are
currently: [link](http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-32.exe) or [link](http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-64.exe)
When you install this library select the option to add its location to your system PATH, so that SDSLite can find it.

### Linux

For Linux install pre-built netCDF-C libraries. For example on Ubuntu:

`sudo apt-get install libnetcdf-dev`

Running the Model
-----------------

This solution contains sufficient configuration (in the "Model setup" folder) and data (in the "Data" folder) files to run the model, although it does require an internet connection to access FetchClimate to get additional climate data.
It is set to run a single cell for one year. 

The Madingley project is a simple command line application that loads the configuration, environment data, passes them to the model and stores the output. Select this project as the start-up project and run it to run the model.

Configuring the Model
---------------------

See the CSV files in "Model setup".
