DMT.Partition
=============

Default implementation of the graph partitioning algorithm. Graph partitioning
is needed to cut up a big model into smaller pieces so the matcher modules can
work their magic on it.

The partitioning process can be split up to three big phases:

1. Coarsen graph.
2. Partition coarsened graph.
3. Uncoarsen graph and refine partitions on the way.

In this higher-level algorithm every phase is separated and visible only by its
interface, so it can be easily re-implemented and swapped out.

Usage
-----

The partitioning algorithm has one main entry point. This is the
`IPartitionManager` interface. Import this into your class to use it. At the
moment it's sole responsibility is to start the partitioning.

Extending parts of the algorithm
--------------------------------

There are three interfaces responsible for the three distinct tasks in the
algorithm. These are:

* `ICoarsener`
* `IPartitioner`
* `IPartitionRefiner`

Implement any of these and mark them with an `Export` attribute. Copy the
resulting dll into the specified folder (see config options for details) and it's
done.

To have **full control** over the partitioning, implement the `IPartitionManager`
interface.
