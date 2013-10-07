using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Interface for loading model (sub)graphs.
    /// </summary>
    public interface IModelLoader
    {
        /// <summary>
        /// Load model nodes for the given datasource. It builds a graph from input data.
        /// There is no guarantee that the graph will be a connected graph.
        /// </summary>
        /// <param name="source">Data source.</param>
        /// <returns>A collection of root nodes. Every node is a "handle" for a graph component.</returns>
        IEnumerable<INode> Load(IDataSource source);
    }
}
