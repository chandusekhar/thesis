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
        /// Gets the degree of the node. Including in and out edges.
        /// </summary>
        int Degree { get; }

        /// <summary>
        /// Connects this node with the specified node. It should create a new edge
        /// between the two nodes. The new edge is an outbound edge for <c>this</c>
        /// node.
        /// </summary>
        /// <param name="node">A node to connecto to.</param>
        /// <param name="direction">
        /// The direction of the edge relative to the node. Inbound means that it will be an
        /// edge that is inbound to the receiver node.
        /// </param>
        /// <returns>The newly created edge.</returns>
        IEdge ConnectTo(INode node, EdgeDirection direction);

        /// <summary>
        /// Gets all (in- and outbound) edges in one collection.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEdge> GetAllEdges();

        /// <summary>
        /// Gets the adjacent nodes (including every direction)
        /// </summary>
        /// <returns></returns>
        IEnumerable<INode> GetAdjacentNodes();
    }
}
