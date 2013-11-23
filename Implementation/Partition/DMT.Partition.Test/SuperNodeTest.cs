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
            var sn = SuperNodeTestFactory.CreateWithChildren(Count, f => new Node(f));
            Assert.Equal(Count, sn.Size);
        }

        [Fact]
        public void SizeOfSuperNodeWhenContainingOtherSuperNodes()
        {
            var sn = SuperNodeTestFactory.CreateWithChildren(Count, _ => { return SuperNodeTestFactory.CreateWithChildren(Count, a => new Node(a)); });
            Assert.Equal(Count * Count, sn.Size);
        }

        // This should not happen in a regural setting
        [Fact]
        public void SizeOfSuperNodeWhenMixingContainingNodes()
        {
            var sn = SuperNodeTestFactory.CreateWithChildren(Count, f => new Node(f));
            sn.Nodes.Add(SuperNodeTestFactory.CreateWithChildren(Count, f => new Node(f)));
            Assert.Equal(Count+Count, sn.Size);
        }


    }
}
