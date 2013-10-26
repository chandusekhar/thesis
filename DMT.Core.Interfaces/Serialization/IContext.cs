using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.Core.Interfaces.Serialization
{
    /// <summary>
    /// Context for the deserialization process.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        IEntityFactory EntityFactory { get; }

        /// <summary>
        /// Gets a node with a specfied id.
        /// </summary>
        /// <param name="id">The node id to look for.</param>
        /// <returns>The node with the specfied id or <c>null</c> if the id is not found.</returns>
        INode GetNode(IId id);

        /// <summary>
        /// Adds a node the context.
        /// </summary>
        /// <param name="node">The node to add.</param>
        void AddNode(INode node);
    }
}
