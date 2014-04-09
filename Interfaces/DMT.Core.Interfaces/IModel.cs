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
    /// component.
    ///
    /// This is not an entity! This will not be persisted. It does not need an id. It is merely
    /// an encapsulation for model specific properties and methods.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Gets the nodes of the graph.
        /// </summary>
        ICollection<INode> Nodes { get; }

        /// <summary>
        /// Gets the nodes in a dictionary where the key is the node id.
        /// </summary>
        /// <returns></returns>
        IDictionary<IId, INode> GetNodeDictionary();
    }
}
