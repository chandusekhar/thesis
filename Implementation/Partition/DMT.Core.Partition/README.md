DMT.Core.Partition
==================

Extension of `DMT.Core` for `DMT.Partition`. This lib is DMT specific, a plugin should not depend on it.
In the implementation chain, this lib should be the last one. It is used by `DMT.Partition` which has some
assumptions about how the interfaces work. This library should not be used directly, only via dependency
injection. Every class should be `internal` in this library.