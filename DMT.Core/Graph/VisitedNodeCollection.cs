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
    internal class VisitedNodeCollection
    {
        // TODO: implement a better collection with O(1) check for containing an element
        Dictionary<IId, object> nodes = new Dictionary<IId, object>();

        public void Add(INode node)
        {
            nodes.Add(node.Id, null);
        }

        public bool Contains(INode node)
        {
            return nodes.ContainsKey(node.Id);
        }

        public bool NotContains(INode node)
        {
            return !this.Contains(node);
        }
    }
}
