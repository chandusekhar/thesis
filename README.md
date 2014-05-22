# Distributed model transformations (DMT)

Thesis project at Budapest University of Technology and Economics.

2013-2014, Tamas Michelberger, tomi.michel [at] gmail.com

The thesis itself can be found in the [docs](docs/) folder. If you (re)use anything from
this project, please drop me a line.

## Overview

A proof-of-concept implementation of distributed pattern matching written in C#.

## Setup and run

1. Run `setup-http-permission.ps1` in an elevated power shell terminal

		$ Set-ExecutionPolicy RemoteSigned
		# or
		$ Set-ExecutionPolicy Unrestricted
		# setup permisson to listen on localhost
		$ .\setup-http-permission.ps1

2. Open the solution (it is a VS 2012 solution).
3. Set up nuget to download dependencies during build.
4. Build the solution.

Before running the application, edit the App.config files for the `DMT.Partition.App` and `DMT.Matcher.App` executables.

To run the application start `DMT.Partition.App`:

	$ DMT.Partition.App.exe
		-m \path\to\model.xml
		-j \path\to\DMT\Jobs\DMT.VIR.Matcher.Local\bin\Debug\DMT.VIR.Matcher.Local.dll
		--mode=FirstOnly

## Model format

The model is stored in an XML file. The exact format can be seen in the *.xml files under the `test-data` directory.
vir-model.xml contains all the types of nodes with their properties, but it is **not** a valid model!