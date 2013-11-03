# Distributed model transformations (DMT)

Thesis project at Budapest University of Technology and Economics. 

2013, Tamas Michelberger, tomi.michel@gmail.com

## Overview

What is the goal of the project, etc.

## Project architecture overview

Master-slave...

### Sub-modules:

Broken into three big subcategories. Complete separation of interface and
implementation achieved by cutting out the interfaces into separate assemblies.

Interface modules can only reference other interface modules. Implementation
modules have two categories. One that cannot use anything other than the
interface and the common library. The other category is for internal usage. Its
aim is to extend implementation, reuse code, and to support OO features like
inheritance.

To preserve full consistency during dependency injection **service** interfaces
cannot extend other interfaces from other assemblies. For example there is an
`IPartitionEntityFactory` interface for instantiating partition specific
entities. If it had extended the `IEntityFactory` it could have caused
inconsistency when `IEntityFactory` is redefined by a plug-in, but
`IPartitionEntityFactory` is not.

#### Interfaces

* __DMT.Core.Interfaces__
	* Shared interfaces between Partition and Matcher.
	* E.g. `INode`, `IEdge`
* __DMT.Partition.Interfaces__
	* Interfaces for partitioning the model-graph.
	* Extension point: by implementing these interfaces a custom partitioner can be introduced.
* __DMT.Matcher.Interfaces__
	* Interfaces for matching and transformation.
	* It is intended to be a gray-box. There will be some predefined extension point in this assembly which can be used to control the matching and transformation process.

#### Implementation

* __DMT.Core__: implementation for the core interfaces
* __DMT.Partition.Core__
	* partial implementation of DMT.Partition.Interface
	* extends DMT.Core
	* for internal usage
* __DMT.Partition__
	* Default implementation of the partitioner interfaces. It is completely transparent. It only uses interfaces all the instantiation happen though dependency injection.
* __DMT.Matcher__
	* Default implementation for the (model) matcher and transformer
	* Runnable
* __DMT.Common__: shared utility classes

#### Application

* __DMT.Master.Module__
	* Master module
		* Coordinates the matchers.
		* Initiates the model transformation.
		* Collects information about the matching process.
	* (Web interface as UI.)
	* Contains everything for Master module, but separate so it can be reused in different executable environments (console app, azure worker role, etc)
* __DMT.Master.App__: merely a facade for DMT.Master.Module, a console application
* __DMT.Slave.Module__
	* Slave module
	* Administrative tasks (communicating between instances, reporting back to master module, etc.).
* __DMT.Slave.App__:  merely a facade for DMT.Slave.Module, a console application
