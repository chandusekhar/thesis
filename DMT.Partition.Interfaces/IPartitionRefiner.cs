using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// Partition refiner interface. Its aim is to refine partitions by
    /// decreasing the coupling between them.
    /// </summary>
    public interface IPartitionRefiner
    {
        /// <summary>
        /// Refine two partitions by exchaning nodes. It refines the partitions
        /// in place, changing the list of nodes in the given <c>p1</c> and <c>p2</c>
        /// partitions.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        void Refine(IPartition p1, IPartition p2);

        /// <summary>
        /// Refine a list of partitions by exchaning nodes. It refines the partitions
        /// in place, changing the list of nodes in the given partitions.
        /// </summary>
        /// <param name="partitions">a list of partitions to refine</param>
        void Refine(IEnumerable<IPartition> partitions);
    }
}
