using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    public interface IPartitionEdge : IEdge
    {
        /// <summary>
        /// Determines whether the edge is an inner edge or not.
        /// An inner edge does not lead to another partition.
        /// </summary>
        bool IsInner { get; }

        /// <summary>
        /// Gets the other partition from the edge.
        /// </summary>
        /// <param name="partition">Partition on one side of the edge.</param>
        /// <exception cref="InvalidOperationException">if non of the partitions equal with the given partition</exception>
        IPartition GetOtherPartition(IPartition partition);

        /// <summary>
        /// Gets the other partition from the edge.
        /// </summary>
        /// <param name="partition">Node on one side of the edge.</param>
        /// <exception cref="InvalidOperationException">if non of the partitions equal with the given partition</exception>
        IPartition GetOtherPartition(IPartitionNode node);
    }
}
