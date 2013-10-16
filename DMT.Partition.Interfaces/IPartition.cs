using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// General interface for graph partitions.
    /// </summary>
    public interface IPartition : IIdentity
    {
        /// <summary>
        /// Gets the collection of nodes in the partition.
        /// </summary>
        ICollection<INode> Nodes { get; }

        /// <summary>
        /// Gets or sets the host machine descriptor of this partition.
        /// </summary>
        IHost Host { get; set; }

        /// <summary>
        /// Sends the partition and its corresponding nodes (with all the available data)
        /// to the host specfied by the <c>Host property</c>.
        /// </summary>
        /// <returns>The result of the send process.</returns>
        Task<ISendPartitionResponse> SendToHost();
    }
}
