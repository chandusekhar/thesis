using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition.Data;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Partition.Test
{
    public class GraphContractorTest
    {
        [Fact]
        public void ContractingWholeGraphIntoOne()
        {
            var e = EntityHelper.CreateEdgeConnectingNodes();

            GraphContractor g = CreateGraphContractor(new INode[] { e.EndA, e.EndB });
            var nodes = g.ContractEdges(new IEdge[] { e });

            Assert.Single(nodes);
            Assert.Empty(GraphHelper.GetEdges(nodes));
        }

        [Fact]
        public void ContractingCircle()
        {
            IEdge e1;
            IEdge e2;
            INode[] nodes;
            CreateCircle4(out e1, out e2, out nodes);
            var g = CreateGraphContractor(nodes);

            var newNodes = g.ContractEdges(new[] { e1, e2 });

            Assert.Equal(2, newNodes.Count());
            Assert.Single(GraphHelper.GetEdges(newNodes));
            Assert.Equal(2.0, GraphHelper.GetEdges(newNodes).Single().Weight);
        }

        private static void CreateCircle4(out IEdge e1, out IEdge e2, out INode[] nodes)
        {
            INode n1 = EntityHelper.CreateNode();
            INode n2 = EntityHelper.CreateNode();
            INode n3 = EntityHelper.CreateNode();
            INode n4 = EntityHelper.CreateNode();

            e1 = n1.ConnectTo(n2, EdgeDirection.Both);
            n2.ConnectTo(n3, EdgeDirection.Both);
            e2 = n3.ConnectTo(n4, EdgeDirection.Both);
            n4.ConnectTo(n1, EdgeDirection.Both);

            nodes = new[] { n1, n2, n3, n4 };
        }

        private GraphContractor CreateGraphContractor(IEnumerable<INode> nodes)
        {
            return new GraphContractor(nodes, new PartitionEntityFactory(new CoreEntityFactory()));
        }
    }
}
