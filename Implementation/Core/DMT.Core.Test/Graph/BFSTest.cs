using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Graph;
using DMT.Core.Interfaces;
using Xunit;
using DMT.Core.Test.Utils;
using DMT.Core.Interfaces.Graph;
using DMT.Core.Entities;

namespace DMT.Core.Test.Graph
{
    /// <summary>
    /// Test for BFS graph traversal
    /// </summary>
    public class BFSTest
    {
        private Traverser t = new Traverser();

        [Fact]
        public void TraverseSingleComponentGraph()
        {
            List<INode> visitedNodes = new List<INode>();

            Edge e = EntityHelper.CreateEdgeConnectingNodes();

            t.VisitingNode += (s, ee) => visitedNodes.Add(ee.Node);
            t.Traverse(ComponentTraversalStrategy.BFS, new INode[] { e.EndA, e.EndB });

            Assert.Equal(new INode[] { e.EndA, e.EndB }, visitedNodes);
        }

        [Fact]
        public void TraverseMultipleComponentGraph()
        {
            List<INode> visitedNodes = new List<INode>();
            Edge e1 = EntityHelper.CreateEdgeConnectingNodes();
            Edge e2 = EntityHelper.CreateEdgeConnectingNodes();
            t.VisitingNode += (s, e) => visitedNodes.Add(e.Node);

            t.Traverse(ComponentTraversalStrategy.BFS, new INode[] { e1.EndA, e1.EndB, e2.EndA, e2.EndB });

            Assert.Equal(new INode[] { e1.EndA, e1.EndB, e2.EndA, e2.EndB }, visitedNodes);
        }

        [Fact]
        public void TraverseAndSearchForNode()
        {
            VisitedNodeCollection visited = new VisitedNodeCollection();
            Node n1 = EntityHelper.CreateNodeWithNeighbours(2);

            t.VisitingNode += (s, ee) =>
            {
                ee.Found = ee.Node.Id.Equals(n1.Id);
            };

            // it works with partial node lis as well. :)
            var found = t.Traverse(n1.GetAdjacentNodes(), ComponentTraversalStrategy.BFS);

            Assert.Equal(n1, found);
        }

        [Fact]
        public void CollectRootsOfComponents()
        {
            var componentRoots = new List<INode>();

            Node n1 = EntityHelper.CreateNode(), n2 = EntityHelper.CreateNode();
            IEdge e1 = EntityHelper.ConnectNewNodeTo(n1);
            IEdge e2 = EntityHelper.ConnectNewNodeTo(n2);
            t.VisitedComponent += (s, e) => componentRoots.Add(e.RootNode);
            
            t.Traverse(ComponentTraversalStrategy.BFS, e1.EndA, e1.EndB, e2.EndA, e2.EndB);

            Assert.Equal(new INode[] { n1, n2 }, componentRoots);
        }

    }
}
