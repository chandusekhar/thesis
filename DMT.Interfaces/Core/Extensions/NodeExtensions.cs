using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Interfaces.Core.Extensions
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Union of inbound and outbound edges.
        /// </summary>
        /// <returns>An enumerable containing the inbound and outbound edges of the node.</returns>
        public static IEnumerable<IEdge> AllEdges(this INode self)
        {
            return self.InboundEdges.Union(self.OutboundEdges);
        }
    }
}
