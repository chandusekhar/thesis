using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Interfaces.Core
{
    /// <summary>
    /// General interface for the edge elements in the graph.
    /// 
    /// An edge always has two nodes. In the partitioning environment the end node
    /// is always an excerpt node. In the transformation environment a node can be 
    /// a full blown node with all the data, or a proxy node which indicates that
    /// the actual node is in another partition.
    /// 
    /// An edge is always directed. It goes from start to end. To build an undirected
    /// graph add edges for both directions on every node.
    /// </summary>
    public interface IEdge : IIdentity
    {
        /// <summary>
        /// Gets or sets the start node of the edge.
        /// </summary>
        INode Start { get; set; }

        /// <summary>
        /// Gets or sets the end node of the edge.
        /// </summary>
        INode End { get; set; }
    }
}
