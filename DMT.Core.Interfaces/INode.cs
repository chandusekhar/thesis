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
    public interface INode : IIdentity
    {
        /// <summary>
        /// Gets the collection of outbound edges.
        /// </summary>
        ICollection<IEdge> OutboundEdges { get; }

        /// <summary>
        /// Gets the collection of inbound edges.
        /// </summary>
        ICollection<IEdge> InboundEdges { get; }
    }
}
