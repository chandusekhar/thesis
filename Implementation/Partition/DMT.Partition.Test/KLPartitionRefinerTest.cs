using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition;
using DMT.Partition.Interfaces;
using Xunit;

namespace DMT.Partition.Test
{
    public class KLPartitionRefinerTest
    {
        KLPartitionRefiner refiner;
        IPartition p1, p2;

        public KLPartitionRefinerTest()
        {
            this.refiner = new KLPartitionRefiner();
            this.p1 = PartitionTest.P();
            this.p2 = PartitionTest.P();
        }

        [Fact]
        public void RefineCalledWithNullThrows()
        {
            Assert.Throws(typeof(ArgumentNullException), () => refiner.Refine(null, null));
            Assert.Throws(typeof(ArgumentNullException), () => refiner.Refine(PartitionTest.P(), null));
            Assert.Throws(typeof(ArgumentNullException), () => refiner.Refine(null, PartitionTest.P()));
        }

        [Fact]
        public void InterPartitionCostForEmptyPartitionsIsZero()
        {
            Assert.Equal(0, refiner.ComputeInitialInterPartitionCost(p1, p2));
        }

        [Fact]
        public void InterPartitionCostForNotEmptyPartitions()
        {
            var n1 = new Node();
            var n2 = new Node();

            n1.ConnectTo(n2, EdgeDirection.Outbound);
            p1.Nodes.Add(n1);

            n2.ConnectTo(n1, EdgeDirection.Outbound);
            p2.Nodes.Add(n2);

            Assert.Equal(2, refiner.ComputeInitialInterPartitionCost(p1, p2));
        }

        [Fact]
        public void ComputeExternalCostThrowsExceptionIfNodeIsInThePartition()
        {
            var n1 = new Node();
            p1.Nodes.Add(n1);
            Assert.Throws(typeof(ArgumentException), () => refiner.ComputeExternalCost(n1, p1));
        }

        [Fact]
        public void ComputeInternalCostThrowsExceptionIfNodeIsNotInThePartition()
        {
            Assert.Throws(typeof(ArgumentException), () => refiner.ComputeInternalCost(new Node(), p1));
        }

        [Fact]
        public void ComputeExternalCostForNode()
        {
            var n1 = new Node();
            var n2 = new Node();
            n1.ConnectTo(n2, EdgeDirection.Outbound);
            p1.Nodes.Add(n2);

            Assert.Equal(1, refiner.ComputeExternalCost(n1, p1));
        }

        [Fact]
        public void ComputeInternalCostForNodeWhenOnlyHasInternalEdges()
        {
            var n1 = new Node();
            var n2 = new Node();
            n1.ConnectTo(n2, EdgeDirection.Outbound);
            p1.Nodes.Add(n1);
            p1.Nodes.Add(n2);

            Assert.Equal(1, refiner.ComputeInternalCost(n1, p1));
        }

        [Fact]
        public void ComputeInternalCostForNodeWhenItHasInternalAndExternalEdges()
        {
            var n1 = new Node();
            var n2 = new Node();
            var n3 = new Node();

            n1.ConnectTo(n2, EdgeDirection.Outbound);
            n1.ConnectTo(n3, EdgeDirection.Outbound);

            // n3 an e node
            p1.Nodes.Add(n1);
            p1.Nodes.Add(n2);

            Assert.Equal(1, refiner.ComputeInternalCost(n1, p1));
        }
    }
}
