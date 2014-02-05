using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Core.Test
{
    public class NodeTest
    {
        [Fact]
        public void ConnectNodes()
        {
            var n1 = EntityHelper.CreateNode();
            var edge = EntityHelper.ConnectNewNodeTo(n1);

            Assert.Equal(1, n1.Edges.Count());
            Assert.Same(edge.EndA, n1);
        }

        [Fact]
        public void ConnectToNullNodeThrowsException()
        {
            var n1 = EntityHelper.CreateNode();
            Assert.Throws(typeof(ArgumentNullException), () => n1.ConnectTo(null, EdgeDirection.Both));
        }

        [Fact]
        public void ConnectNodesWithGivenEdge()
        {
            var n1 = EntityHelper.CreateNode();
            var n2 = EntityHelper.CreateNode();

            IEdge e = new Edge(n1, n2, EdgeDirection.Both, new CoreEntityFactory());
            IEdge e2 = n1.ConnectTo(n2, e);

            Assert.Same(e2, e);
            Assert.Contains(e, n1.Edges);
            Assert.Contains(e, n2.Edges);
        }

        [Fact]
        public void GetAdjacentNodes()
        {
            Node n1 = EntityHelper.CreateNode();
            IEdge e1 = EntityHelper.ConnectNewNodeTo(n1);
            IEdge e2 = EntityHelper.ConnectNewNodeTo(n1);

            var neighbours = n1.GetAdjacentNodes();

            Assert.Contains(e1.EndB, neighbours);
            Assert.Contains(e2.EndB, neighbours);
        }

        [Fact]
        public void DegreeOfNodeEqualsNodesEdgeCount()
        {
            // degree of node is the number of in and out edges
            Node n1 = EntityHelper.CreateNode();
            EntityHelper.ConnectNewNodeTo(n1);
            EntityHelper.ConnectNewNodeTo(n1);

            Assert.Equal(2, n1.Degree);
        }

        [Fact]
        public void IsNeighbourShouldReturnTrueForNeighbouringNodes()
        {
            var n = EntityHelper.CreateNode();
            var neighbour = EntityHelper.CreateNode();
            n.ConnectTo(neighbour, EdgeDirection.Both);

            Assert.True(n.IsNeighbour(neighbour));
        }

        [Fact]
        public void IsNeighbourShouldReturnFalseForNotNeighbouringNodes()
        {
            var n = EntityHelper.CreateNode();
            EntityHelper.ConnectNewNodeTo(n);
            var n2 = EntityHelper.CreateNode();

            Assert.False(n.IsNeighbour(n2));
        }

    }
}
