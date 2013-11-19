using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using Xunit;

namespace DMT.Core.Test
{
    public class NodeTest
    {
        [Fact]
        public void ConnectNodesAsOutbound()
        {
            var n1 = new Node(new CoreEntityFactory());
            var edge = n1.ConnectTo(new Node(), EdgeDirection.Outbound);

            Assert.Equal(1, n1.OutboundEdges.Count);
            Assert.Same(edge.Source, n1);
        }

        [Fact]
        public void ConnectNodesAsInbound()
        {
            var n = new Node(new CoreEntityFactory());
            var edge = n.ConnectTo(new Node(), EdgeDirection.Inbound);

            Assert.Same(edge.Target, n);
        }


        [Fact]
        public void ConnectToNullNodeThrowsException()
        {
            var n1 = new Node();
            Assert.Throws(typeof(ArgumentNullException), () => n1.ConnectTo(null, EdgeDirection.Outbound));
        }

        [Fact]
        public void GetAdjacentNodes()
        {
            Node n1 = new Node();
            Edge e1 = new Edge(n1, new Node());
            Edge e2 = new Edge(n1, new Node());

            var neighbours = n1.GetAdjacentNodes();

            Assert.Contains(e1.Target, neighbours);
            Assert.Contains(e1.Target, neighbours);
        }

        [Fact]
        public void DegreeOfNode()
        {
            // degree of node is the number of in and out edges
            var n1 = new Node(new CoreEntityFactory());
            n1.ConnectTo(new Node(), EdgeDirection.Outbound);
            n1.ConnectTo(new Node(), EdgeDirection.Outbound);

            Assert.Equal(2, n1.Degree);
        }

    }
}
