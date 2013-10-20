using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Graph;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Test.Utils
{
    public static class Extensions
    {
        public static void AddNodes(this IContext self, params INode[] nodes)
        {
            foreach (var node in nodes)
            {
                self.AddNode(node);
            }
        }

        public static INode Traverse(this Traverser self, params INode[] nodes)
        {
            return self.Traverse(nodes);
        }
    }
}
