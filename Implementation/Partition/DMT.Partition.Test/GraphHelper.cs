using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Extensions;
using DMT.Core.Interfaces;
using DMT.Core.Test.Utils;

namespace DMT.Partition.Test
{
    public static class GraphHelper
    {
        public static List<INode> Circle4()
        {
            INode n1 = EntityHelper.CreateNode();
            INode n2 = EntityHelper.CreateNode();
            INode n3 = EntityHelper.CreateNode();
            INode n4 = EntityHelper.CreateNode();

            n1.ConnectTo(n2, EdgeDirection.Both);
            n2.ConnectTo(n3, EdgeDirection.Both);
            n3.ConnectTo(n4, EdgeDirection.Both);
            n4.ConnectTo(n1, EdgeDirection.Both);

            List<INode> nodes = new List<INode> { n1, n2, n3, n4 };
            return nodes;
        }

        public static IEnumerable<IEdge> GetEdges(IEnumerable<INode> nodes)
        {
            HashSet<IEdge> edges = new HashSet<IEdge>();
            foreach (var node in nodes)
            {
                edges.AddAll(node.Edges);
            }

            return edges;
        }
    }
}
