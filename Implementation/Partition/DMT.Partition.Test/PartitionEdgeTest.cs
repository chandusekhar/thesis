using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Partition;
using DMT.Partition.Interfaces;
using Xunit;

namespace DMT.Partition.Test
{
    public class PartitionEdgeTest
    {
        [Fact]
        public void IsInnerReturnsTrueOnlyForInnerEdges()
        {
            var factory = new PartitionEntityFactory();
            IPartition partition = new DMT.Core.Partition.Partition(factory);
            PartitionNode nodeA = new PartitionNode(factory);
            PartitionNode nodeB = new PartitionNode(factory);
            nodeA.Partition = partition;
            nodeB.Partition = partition;

            PartitionEdge edge = new PartitionEdge(nodeA, nodeB, Core.Interfaces.EdgeDirection.Both, factory);
            nodeA.ConnectTo(nodeB, edge);

            Assert.True(edge.IsInner);
        }

        [Fact]
        public void GetOtherPartitionReturnsTheCorrectPartition()
        {
            var factory = new PartitionEntityFactory();
            PartitionNode nodeA = new PartitionNode(factory);
            PartitionNode nodeB = new PartitionNode(factory);
            IPartition p1, p2;
            nodeA.Partition = p1 = new DMT.Core.Partition.Partition(factory);;
            nodeB.Partition = p2 = new DMT.Core.Partition.Partition(factory);;

            PartitionEdge edge = new PartitionEdge(nodeA, nodeB, Core.Interfaces.EdgeDirection.Both, factory);
            nodeA.ConnectTo(nodeB, edge);

            Assert.Equal(p2, edge.GetOtherPartition(p1));
        }

        [Fact]
        public void GetOtherPartitionCannotAcceptNull()
        {
            var edge = new PartitionEdge(new PartitionEntityFactory());
            Assert.Throws(typeof(ArgumentNullException), () => edge.GetOtherPartition((IPartition)null));
        }
    }
}
