using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// The main interface for graph partitioning.
    ///
    /// Partitioning produces disjoint sets of nodes. The main goal is to minimize the
    /// coupling between these sets. The meaning of minimization is implementation dependent.
    /// </summary>
    public interface IPartitioner
    {
        /// <summary>
        /// Entry point for the partitioning process.
        /// </summary>
        /// <param name="nodes">Nodes of the graph to partition.</param>
        /// <returns>A list of partitions.</returns>
        IEnumerable<IPartition> Partition(IEnumerable<INode> nodes);
    }
}
