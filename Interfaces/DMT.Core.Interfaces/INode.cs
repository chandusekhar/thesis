using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces.Results;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// General interface for node elements in the graph.
    ///
    /// A node has edges. All edges stored in one collection. The direction of the edge is
    /// stored in the edge itself.
    /// </summary>
    public interface INode : IEntity
    {
        /// <summary>
        /// Gets the collection of edges.
        /// </summary>
        IEnumerable<IEdge> Edges { get; }

        /// <summary>
        /// Gets the degree of the node. Including in and out edges.
        /// </summary>
        int Degree { get; }

        /// <summary>
        /// Connects this node with the specified node. It should create a new edge
        /// between the two nodes. The direction of the edge is set by the <c>direction</c>
        /// argument.
        /// </summary>
        /// <param name="otherNode">A node to connecto to.</param>
        /// <param name="direction">The direction of the edge that is to be created.</param>
        /// <returns>The newly created edge.</returns>
        IEdge ConnectTo(INode otherNode, EdgeDirection direction);

        /// <summary>
        /// Connects this node to the otherNode using the specified edge.
        /// </summary>
        /// <param name="otherNode">The node to connect to.</param>
        /// <param name="edge">The edge </param>
        /// <returns></returns>
        IEdge ConnectTo(INode otherNode, IEdge edge);

        /// <summary>
        /// Disconnects the nodes on the specified edge. It updates the other end of the edge as well.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>true if successfully disconnected, false otherwise</returns>
        bool Disconnect(IEdge edge);

        /// <summary>
        /// Gets the adjacent nodes (including every direction)
        /// </summary>
        /// <returns></returns>
        IEnumerable<INode> GetAdjacentNodes();

        /// <summary>
        /// Add an edge to the node's edges.
        /// </summary>
        /// <param name="edge">the edge to add</param>
        void AddEdge(IEdge edge);

        /// <summary>
        /// Remove an edge from the node's edges
        /// </summary>
        /// <param name="edge">the edge to remove</param>
        /// <returns>true only if the edge was removed, false otherwise</returns>
        bool RemoveEdge(IEdge edge);

        /// <summary>
        /// Determines whether the given node is neighbour of the node or not.
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <returns>true only if the given node is a neighbour</returns>
        NeighbourResult IsNeighbour(INode node);
    }
}
