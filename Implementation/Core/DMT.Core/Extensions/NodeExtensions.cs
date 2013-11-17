using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core.Extensions
{
    internal static class NodeExtensions
    {
        /// <summary>
        /// Union of inbound and outbound edges.
        /// </summary>
        /// <returns>An enumerable containing the inbound and outbound edges of the node.</returns>
        public static IEnumerable<IEdge> AllEdges(this INode self)
        {
            return self.InboundEdges.Union(self.OutboundEdges);
        }

        /// <summary>
        /// Find all adjacent nodes.
        /// </summary>
        /// <returns>a list of adjacent nodes including both inbound and outbound</returns>
        public static IEnumerable<INode> AdjacentNodes(this INode self)
        {
            List<INode> nodeList = new List<INode>();

            foreach (var edge in self.InboundEdges)
            {
                nodeList.Add(edge.Source);
            }

            foreach (var edge in self.OutboundEdges)
            {
                nodeList.Add(edge.Target);
            }

            return nodeList;
        }
    }
}
