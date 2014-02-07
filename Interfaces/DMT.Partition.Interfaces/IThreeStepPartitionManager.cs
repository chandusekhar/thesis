using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Interfaces.Events;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// A more specific parition orchestrator. It uses a three step algorithm:
    /// 
    /// <para>Coarsen</para>
    /// <para>Parition</para>
    /// <para>Uncoarsen and refine</para>
    /// </summary>
    public interface IThreeStepPartitionManager : IPartitionManager
    {
        /// <summary>
        /// Gets coarsener.
        /// </summary>
        ICoarsener Coarsener { get; }

        /// <summary>
        /// Gets the partition refiner.
        /// </summary>
        IPartitionRefiner Refiner { get; }

        /// <summary>
        /// Gets the partitioner object.
        /// </summary>
        IPartitioner Partitioner { get; }

        event AfterCoarseningEventHandler AfterCoarsening;
    }
}
