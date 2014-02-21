DMT.Matcher.Module
==================

This is the _slave_ module. It is responsible for following things:

* Receiving the partitions
* Finding a match for a given pattern
* Reporting back to the partitioning (master) module

This is just a class library and separated from the actual executable. Although
it is not directly excutable, it is self-contained and runs all the necessary
services that it needs.