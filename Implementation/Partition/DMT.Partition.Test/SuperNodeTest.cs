using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Partition;
using Xunit;
using DMT.Common.Extensions;
using DMT.Core.Interfaces;

namespace DMT.Partition.Test
{
    public class SuperNodeTest
    {
        const int Count = 5;

        [Fact]
        public void SizeOfSuperNodeWithOnlyNormalNodes()
        {
            var sn = CreateWithChildren(Count, f => new Node(f));
            Assert.Equal(Count, sn.Size);
        }

        [Fact]
        public void SizeOfSuperNodeWhenContainingOtherSuperNodes()
        {
            var sn = CreateWithChildren(Count, _ => { return CreateWithChildren(Count, a => new Node(a)); });
            Assert.Equal(Count * Count, sn.Size);
        }

        // This should not happen in a regural setting
        [Fact]
        public void SizeOfSuperNodeWhenMixingContainingNodes()
        {
            var sn = CreateWithChildren(Count, f => new Node(f));
            sn.Nodes.Add(CreateWithChildren(Count, f => new Node(f)));
            Assert.Equal(Count+Count, sn.Size);
        }

        private SuperNode CreateWithChildren(int count, Func<IEntityFactory, INode> createFunc)
        {
            IEntityFactory f = new CoreEntityFactory();
            SuperNode sn = new SuperNode(f);

            for (int i = 0; i < count; i++)
            {
                sn.Nodes.Add(createFunc(f));
            }

            return sn;
        }
    }
}
