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
    /// An edge always has two nodes. The direction of the edge is stored in 
    /// the <c>Direction</c> property. An edge is immutable after it has been connected
    /// to the graph.
    ///
    /// If <c>EndA</c> or <c>EndB</c> nodes are <c>null</c> that means that
    /// the edge has not been added (connected) to the graph.
    /// </summary>
    public interface IEdge : IEntity, IWeighted
    {
        /// <summary>
        /// Gets the nodeA node of the edge.
        /// </summary>
        INode EndA { get; }

        /// <summary>
        /// Gets the nodeB node of the edge.
        /// </summary>
        INode EndB { get; }

        /// <summary>
        /// Gets the direction of the edge.
        /// </summary>
        EdgeDirection Direction { get; }

        /// <summary>
        /// Gets the the other node of the edge.
        /// </summary>
        /// <param name="node">one end of the edge</param>
        /// <returns></returns>
        /// <exception cref="InvalidNodeException">When the provided node is not connected by the edge.</exception>
        INode GetOtherNode(INode node);
    }
}
