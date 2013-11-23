﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Partition;
using Xunit;

namespace DMT.Partition.Test
{
    public class BucketPartitionerTest
    {
        const int count = 1000;
        const int numberOfNodesInPartition = 100;

        BucketPartitioner partitioner;

        public BucketPartitionerTest()
        {
            this.partitioner = new BucketPartitioner(new PartitionEntityFactory(new CoreEntityFactory()));
            this.partitioner.NumberOfNodesInPartition = numberOfNodesInPartition;
        }

        [Fact]
        public void PartitioningSimpleNodes()
        {
            List<INode> nodes = new List<INode>();
            for (int i = 0; i < count; i++)
            {
                nodes.Add(new Node());
            }

            var partitions = partitioner.Partition(nodes);

            Assert.Equal(count / numberOfNodesInPartition, partitions.Count());
            foreach (var p in partitions)
            {
                Assert.Equal(numberOfNodesInPartition, p.Nodes.Count);
            }
        }

        [Fact]
        public void PartitionSuperNodes()
        {
            const int nodeCount = 75;
            const int innerNodeCount = 50;
            List<INode> nodes = new List<INode>();

            for (int i = 0; i < nodeCount; i++)
            {
                nodes.Add(SuperNodeTestFactory.CreateWithChildren(innerNodeCount, f => new Node()));
            }

            var ps = partitioner.Partition(nodes);
            var expected = (int) Math.Ceiling(nodes.Sum(n => ((SuperNode)n).Size) / (double)numberOfNodesInPartition);
            Assert.Equal(expected, ps.Count());
        }
    }
}
