using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
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
    ///
    /// If <c>Start</c> or <c>End</c> nodes are null that means that
    /// the edge has not been added (connected) to the graph.
    /// </summary>
    public interface IEdge : IEntity
    {
        /// <summary>
        /// Gets or sets the start node of the edge.
        /// </summary>
        INode Start { get; }

        /// <summary>
        /// Gets or sets the end node of the edge.
        /// </summary>
        INode End { get; }

        /// <summary>
        /// Sets the <c>start</c> and <c>end</c> nodes of this edge and sets up connections
        /// which means that <c>this</c> edge will be added to the <c>start</c> node's
        /// outbound edges collection and the the <c>end</c> node's inbound edges collections.
        /// </summary>
        /// <param name="start">start node of the relationship</param>
        /// <param name="end">end node of the relationship</param>
        void ConnectNodes(INode start, INode end);
    }
}
