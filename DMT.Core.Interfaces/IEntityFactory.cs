using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Interface for entity creation.
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// Create an id that implements the <c>IId</c> interface.
        /// </summary>
        /// <returns>A new id.</returns>
        IId CreateId();

        /// <summary>
        /// Creates a new node instance.
        /// </summary>
        /// <returns>The new (and empty) node instance.</returns>
        INode CreateNode();

        /// <summary>
        /// Creates a new edge instance.
        /// </summary>
        /// <returns>The new (and empty) edge instance.</returns>
        IEdge CreateEdge();

        /// <summary>
        /// Creates a new edge instance with start and end nodes.
        ///
        /// It sets up all the necessary connections between the objects.
        /// </summary>
        /// <param name="start">start node</param>
        /// <param name="end">end node</param>
        /// <returns>The new edge object.</returns>
        IEdge CreateEdge(INode start, INode end);
    }
}
