using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Interfaces.Core
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

        // TODO: host property
    }
}
