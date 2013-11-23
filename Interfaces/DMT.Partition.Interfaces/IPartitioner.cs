using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// Partitioning produces disjoint sets of nodes. The main goal is to minimize the
    /// coupling between these sets. The meaning of minimization is implementation dependent.
    /// </summary>
    public interface IPartitioner
    {
        /// <summary>
        /// Gets or sets the (approximate) number of partitions to be produced.
        /// </summary>
        int NumberOfPartitions { get; set; }

        /// <summary>
        /// Gets or sets the (approximate) number of nodes that will be put into one partition.
        /// </summary>
        int NumberOfNodesInPartition { get; set; }

        /// <summary>
        /// Produces a k-way partitioning of the graph. The number of partitions are controlled
        /// by the <c>NumberOfPartitions</c> property.
        /// </summary>
        /// <param name="nodes">Nodes of the graph to partition.</param>
        /// <returns>A list of partitions.</returns>
        IEnumerable<IPartition> Partition(IEnumerable<INode> nodes);
    }
}
