using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using Xunit;

namespace DMT.Core.Test
{
    public class NodeTest
    {
        [Fact]
        public void ConnectNodes()
        {
            var n1 = new Node(new CoreEntityFactory());
            n1.ConnectTo(new Node());

            Assert.Equal(1, n1.OutboundEdges.Count);
        }

        [Fact]
        public void ConnectToNullNodeThrowsException()
        {
            var n1 = new Node();
            Assert.Throws(typeof(ArgumentNullException), () => n1.ConnectTo(null));
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
    }
}
