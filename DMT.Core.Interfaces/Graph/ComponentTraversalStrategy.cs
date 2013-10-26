using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Graph
{
    public enum ComponentTraversalStrategy
    {
        /// <summary>
        /// Breadth-first search
        /// </summary>
        BFS,

        /// <summary>
        /// Depth-first search
        /// </summary>
        DFS
    }
}
