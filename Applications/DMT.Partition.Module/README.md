DMT.Partition.Module
=================

This is the master module. It is responsible for the following things:

* loading the model from the persistent store
* partitioning the model
* spin up (remote) instances for the slave modules (such as matcher)
* initialize slave modules and send data (~partition)
* receive data from slave modules, process and visualize

This is the single entry point for the whole application. It has a small web
interface, so a new partitioning or model transformation can be started.

This is also a class library with a single entry point: `PartitionModule#Start`. It
is separated from the actual executable library, so it can be used in different
scenarios. For example as a console application or an Azure Worker Role.

