using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// General interface for node elements in the graph.
    /// </summary>
    public interface INode : IEntity
    {
        /// <summary>
        /// Gets the collection of outbound edges.
        /// </summary>
        ICollection<IEdge> OutboundEdges { get; }

        /// <summary>
        /// Gets the collection of inbound edges.
        /// </summary>
        ICollection<IEdge> InboundEdges { get; }

        /// <summary>
        /// Connects this node with the specified node. It should create a new edge
        /// between the two nodes. The new edge is an outbound edge for <c>this</c>
        /// node.
        /// </summary>
        /// <param name="node">A node to connecto to.</param>
        void ConnectTo(INode node);
    }
}
