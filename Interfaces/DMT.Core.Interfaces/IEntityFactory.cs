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
        /// Parse the given string into an id.
        /// </summary>
        /// <param name="idStr">sring representation of the id</param>
        /// <returns></returns>
        IId ParseId(string idStr);

        /// <summary>
        /// Creates a new node instance.
        /// </summary>
        /// <returns>The new (and empty) node instance.</returns>
        INode CreateNode();

        /// <summary>
        /// Creates a new node with the suplied type info.
        /// </summary>
        /// <param name="typeInfo">a string which represents some kind of type information</param>
        /// <returns>The new (and empty) node instance.</returns>
        INode CreateNode(string typeInfo);

        /// <summary>
        /// Creates a remote node.
        /// </summary>
        /// <returns></returns>
        IRemoteNode CreateRemoteNode(IId id);

        /// <summary>
        /// Creates a new edge instance.
        /// </summary>
        /// <returns>The new (and empty) edge instance.</returns>
        IEdge CreateEdge();

        /// <summary>
        /// Creates a new edge instance with nodeA and nodeB nodes.
        ///
        /// It sets up all the necessary connections between the objects.
        /// </summary>
        /// <param name="nodeA">nodeA node</param>
        /// <param name="nodeB">nodeB node</param>
        /// <param name="direction">the direction of the edge</param>
        /// <returns>The new edge object.</returns>
        IEdge CreateEdge(INode nodeA, INode nodeB, EdgeDirection direction);
    }
}
