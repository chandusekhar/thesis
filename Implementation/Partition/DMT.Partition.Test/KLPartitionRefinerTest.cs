using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Test.Utils;
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
            var n1 = EntityHelper.CreateNode();
            var n2 = EntityHelper.CreateNode();

            n1.ConnectTo(n2, EdgeDirection.Both);
            p1.Nodes.Add(n1);

            n2.ConnectTo(n1, EdgeDirection.Both);
            p2.Nodes.Add(n2);

            Assert.Equal(2, refiner.ComputeInitialInterPartitionCost(p1, p2));
        }

        [Fact]
        public void ComputeExternalCostThrowsExceptionIfNodeIsInThePartition()
        {
            var n1 = EntityHelper.CreateNode();
            p1.Nodes.Add(n1);
            Assert.Throws(typeof(ArgumentException), () => refiner.ComputeExternalCost(n1, p1));
        }

        [Fact]
        public void ComputeInternalCostThrowsExceptionIfNodeIsNotInThePartition()
        {
            Assert.Throws(typeof(ArgumentException), () => refiner.ComputeInternalCost(EntityHelper.CreateNode(), p1));
        }

        [Fact]
        public void ComputeExternalCostForNode()
        {
            var n1 = EntityHelper.CreateNode();
            var n2 = EntityHelper.CreateNode();
            n1.ConnectTo(n2, EdgeDirection.Both);
            p1.Nodes.Add(n2);

            Assert.Equal(1, refiner.ComputeExternalCost(n1, p1));
        }

        [Fact]
        public void ComputeInternalCostForNodeWhenOnlyHasInternalEdges()
        {
            var n1 = EntityHelper.CreateNode();
            var n2 = EntityHelper.CreateNode();
            n1.ConnectTo(n2, EdgeDirection.Both);
            p1.Nodes.Add(n1);
            p1.Nodes.Add(n2);

            Assert.Equal(1, refiner.ComputeInternalCost(n1, p1));
        }

        [Fact]
        public void ComputeInternalCostForNodeWhenItHasInternalAndExternalEdges()
        {
            var n1 = EntityHelper.CreateNode();
            var n2 = EntityHelper.CreateNode();
            var n3 = EntityHelper.CreateNode();

            n1.ConnectTo(n2, EdgeDirection.Both);
            n1.ConnectTo(n3, EdgeDirection.Both);

            // n3 an e node
            p1.Nodes.Add(n1);
            p1.Nodes.Add(n2);

            Assert.Equal(1, refiner.ComputeInternalCost(n1, p1));
        }

        [Fact]
        public void PickNPicksExactlyNPairs()
        {
            List<Tuple<IPartition, IPartition>> list = new List<Tuple<IPartition, IPartition>>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(Tuple.Create((IPartition)PartitionTest.P(), (IPartition)PartitionTest.P()));
            }

            Assert.Equal(5, refiner.PickN(5, list).Count());
        }
    }
}
