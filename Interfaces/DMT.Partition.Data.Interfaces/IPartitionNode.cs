using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// Interface to specialize a node for partitioning.
    /// 
    /// Holds extra information about the relation between the node and the
    /// partitions.
    /// </summary>
    public interface IPartitionNode : INode
    {
        /// <summary>
        /// Gets or sets the partition which the node belongs to.
        /// </summary>
        IPartition Partition { get; set; }
    }
}
