# Distributed model transformations (DMT)

Thesis project at Budapest University of Technology and Economics. 

2013, Tamas Michelberger, tomi.michel@gmail.com

## Overview

What is the goal of the project, etc.

## Project architecture overview

Master-slave...

### Sub-modules:

* __DMT.Common__: shared utility classes
* __DMT.Core.Interfaces__
	* Shared interfaces between Partition and Matcher.
	* E.g. `INode`, `IEdge`
* __DMT.Core__: implementation for the core interfaces
* __DMT.Partition.Interfaces__
	* Interfaces for partitioning the model-graph.
	* Extension point: by implementing these interfaces a custom partitioner can be introduced.
* __DMT.Partition__
	* Default implementation of the partitioner interfaces. It is completely transparent. It only uses interfaces all the instantiation happen though dependency injection.
	* Runnable.
	* Master module
		* Coordinates the matchers.
		* Initiates the model transformation.
		* Collects information about the matching process.
	* (Web interface as UI.)
* __DMT.Matcher.Interfaces__
	* Interfaces for matching and transformation.
	* It is intended to be a gray-box. There will be some predefined extension point in this assembly which can be used to control the matching and transformation process.
* __DMT.Matcher__
	* Default implementation for the (model) matcher and transformer
	* Runnable
	* Slave module
	* Administrative tasks (communicating between instances, reporting back to master module, etc.).
