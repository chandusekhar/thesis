using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Interface for loading nodes from raw source.
    /// </summary>
    public interface IDataSource : IDisposable
    {
        /// <summary>
        /// Loads the next node from the raw data source. It returns a node stub,
        /// which is not connected to the rest of the graph.
        /// </summary>
        /// <returns>A node stub.</returns>
        INode NextNode();
    }
}
