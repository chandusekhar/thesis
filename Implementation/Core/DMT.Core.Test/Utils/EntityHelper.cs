using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;

namespace DMT.Core.Test.Utils
{
    public static class EntityHelper
    {
        public static Node CreateNode()
        {
            return new Node(new CoreEntityFactory());
        }

        public static Node CreateNodeWithNeighbours(int neighbourCount)
        {
            Node n = CreateNode();
            for (int i = 0; i < neighbourCount; i++)
            {
                ConnectNewNodeTo(n);
            }

            return n;
        }

        public static IEdge ConnectNewNodeTo(Node node)
        {
            return node.ConnectTo(CreateNode(), EdgeDirection.Both);
        }

        public static Edge CreateEdgeConnectingNodes()
        {
            Node n1 = CreateNode();
            Node n2 = CreateNode();
            Edge e = new Edge(n1, n2, EdgeDirection.Both, new CoreEntityFactory());

            n1.ConnectTo(n2, e);

            return e;
        }
    }
}
