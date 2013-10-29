using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// A node containing multiple other nodes. 
    /// </summary>
    public interface IMultiNode : INode
    {
        /// <summary>
        /// Gets the nodes that belongs to this multinode.
        /// </summary>
        ICollection<INode> Nodes { get; }

        /// <summary>
        /// Gets the size of the multinode, which is the current number of
        /// nodes that it contains.
        /// </summary>
        int Size { get; }

    }
}
