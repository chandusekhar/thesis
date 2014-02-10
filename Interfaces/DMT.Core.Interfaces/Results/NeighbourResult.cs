using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Results
{
    public sealed class NeighbourResult
    {
        /// <summary>
        /// Indicates whether the query was successful or not.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the first edge that connects the two nodes.
        /// </summary>
        public IEdge Edge { get; private set; }

        public NeighbourResult(bool success, IEdge edge)
        {
            this.Success = success;
            this.Edge = edge;
        }
    }
}
