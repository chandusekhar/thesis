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
