using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Data.Interfaces;
using DMT.VIR.Data;
using Xunit;

namespace DMT.Vir.Data.Test
{
    public class VirNodeTest
    {
        [Fact]
        public void TestSortingEdges()
        {
            var factory = new VirEntityFactory();

            Person node = new Person(factory);
            for (int i = 0; i < 5; i++)
            {
                IMatchEdge edge = (IMatchEdge)node.ConnectTo(new Person(factory), Core.Interfaces.EdgeDirection.Both);
                if (i % 2 == 0)
                {
                    edge.RemotePartitionId = factory.CreateId();
                }
            }

            // 3 out of 5 nodes are remote
            node.SortEdges();
            var list = node.Edges.Cast<IMatchEdge>().ToList();

            // first two are local
            Assert.False(list[0].IsRemote);
            Assert.False(list[1].IsRemote);

            // rest is remote
            Assert.True(list.Skip(2).All(e => e.IsRemote));
            
        }
    }
}
