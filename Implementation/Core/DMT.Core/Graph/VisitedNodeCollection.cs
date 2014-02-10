using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core.Graph
{
    /// <summary>
    /// Collection for visited nodes during graph traversal. It has O(1) lookup time for
    /// Contains and NotContains methods.
    /// </summary>
    public class VisitedNodeCollection
    {
        private HashSet<IId> nodes = new HashSet<IId>();

        public void Add(INode node)
        {
            nodes.Add(node.Id);
        }

        public bool Contains(INode node)
        {
            return nodes.Contains(node.Id);
        }

        public bool NotContains(INode node)
        {
            return !this.Contains(node);
        }
    }
}
