using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Interface for model specific properties. It encapsulates the model.
    ///
    /// The model is represented as a graph and this graph can contain more than one
    /// component. The component root nodes are kept in this object.
    ///
    /// This is not an entity! This will not be persisted. It does not need an id. It is merely
    /// an encapsulation for model specific properties and methods.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Gets the root nodes of the connected graph components.
        /// </summary>
        ICollection<INode> ComponentRoots { get; }
    }
}
