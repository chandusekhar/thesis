using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Partition;
using DMT.Partition.Interfaces;
using Xunit;

namespace DMT.Partition.Test
{
    public class PartitionTest
    {
        const int count = 10;

        [Fact]
        public void InflatingPartitionWithOnlySimpleNodes()
        {
            var p = CreatePartitionWithChildren(count, _ => new Node());
            var n = p.Nodes;
            p.Inflate();
            Assert.NotSame(n, p.Nodes);
            Assert.Equal(count, p.Nodes.Count);
        }

        [Fact]
        public void InflatePartitionWithSuperNodes()
        {
            var p = CreatePartitionWithChildren(count, _ => SuperNodeTestFactory.CreateWithChildren(count, f => new Node()));
            var n = p.Nodes;
            p.Inflate();

            Assert.NotSame(n, p.Nodes);
            Assert.Equal(count * count, p.Nodes.Count);
        }

        [Fact]
        public void InflatePartitionWithMixedNodes()
        {
            var p = CreatePartitionWithChildren(count, _ => SuperNodeTestFactory.CreateWithChildren(count, f => new Node()));
            for (int i = 0; i < count; i++)
            {
                p.Nodes.Add(new Node());
            }
            var n = p.Nodes;

            p.Inflate();

            Assert.Equal(count * count + count, p.Nodes.Count);
        }

        [Fact]
        public void GetExternalEdgesForEmptyPartition()
        {
            var p = P();

            Assert.Empty(p.GetExternalEdges());
        }

        [Fact]
        public void GetExternalEdgesForNonEmptyPartition()
        {
            var n1 = new Node();
            var n2 = new Node();
            var n3 = new Node();

            n1.ConnectTo(n2, EdgeDirection.Outbound);
            n1.ConnectTo(n3, EdgeDirection.Outbound);

            var p1 = P();
            p1.Nodes.Add(n1);
            p1.Nodes.Add(n2);

            var edges = p1.GetExternalEdges();
            Assert.Single(edges);
            Assert.Contains(edges.Single(), n1.OutboundEdges);
        }

        [Fact]
        public void GetExternalEdgesShouldBeEmptyWhenNoExternalEdges()
        {
            Node n1 = new Node(), n2 = new Node();
            n1.ConnectTo(n2, EdgeDirection.Outbound);

            var p = P();
            p.Nodes.Add(n1);
            p.Nodes.Add(n2);

            Assert.Empty(p.GetExternalEdges());
        }

        [Fact]
        public void HasNodeForNullReturnsFalse()
        {
            var p = P();
            p.Nodes.Add(new Node());
            Assert.False(p.HasNode(null));
        }

        [Fact]
        public void EmptyPartitionReturnsFalseForHasNode()
        {
            Assert.False(P().HasNode(null));
            Assert.False(P().HasNode(new Node()));
        }

        [Fact]
        public void HasNodeForActualNodeInPartitionReturnsTrue()
        {
            var p = P();
            var n1 = new Node();
            p.Nodes.Add(n1);
            Assert.True(p.HasNode(n1));
        }

        [Fact]
        public void NoEdgesBetweenEmptyPartitions()
        {
            var p1 = P();
            var p2 = P();

            Assert.Empty(p1.GetEdgesBetween(p2));
        }

        [Fact]
        public void NoEdgesBetweenTheSamePartition()
        {
            var p = P();
            Assert.Empty(p.GetEdgesBetween(p));
        }

        [Fact]
        public void GetEdgesBetweenThrowsForNullArgument()
        {
            Assert.Throws(typeof(ArgumentNullException), () => P().GetEdgesBetween(null));
        }

        [Fact]
        public void GetEdgesBetweenPartitionWhenThereAreEdges()
        {
            var p1 = P();
            var p2 = P();
            var n1 = new Node();
            var n2 = new Node();
            var edge = n1.ConnectTo(n2, EdgeDirection.Outbound);
            p1.Nodes.Add(n1);
            p2.Nodes.Add(n2);

            Assert.Contains(edge, p1.GetEdgesBetween(p2));
        }

        /// <summary>
        /// Shortcut to instatiate a empty otherPartition.
        /// </summary>
        /// <returns></returns>
        internal static DMT.Core.Partition.Partition P()
        {
            return new DMT.Core.Partition.Partition(new CoreEntityFactory());
        }

        private DMT.Core.Partition.Partition CreatePartitionWithChildren(int count, Func<IEntityFactory, INode> createFunc)
        {
            var p = new DMT.Core.Partition.Partition(new CoreEntityFactory());
            IEntityFactory f = new CoreEntityFactory();
            for (int i = 0; i < count; i++)
            {
                p.Nodes.Add(createFunc(f));
            }

            return p;
        }
    }
}
