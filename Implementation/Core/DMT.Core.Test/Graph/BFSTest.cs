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

            Edge e = new Edge(new Node(), new Node());

            t.VisitingNode += (s, ee) => visitedNodes.Add(ee.Node);
            t.Traverse(ComponentTraversalStrategy.BFS, new INode[] { e.Source, e.Target });

            Assert.Equal(new INode[] { e.Source, e.Target }, visitedNodes);
        }

        [Fact]
        public void TraverseMultipleComponentGraph()
        {
            List<INode> visitedNodes = new List<INode>();
            Edge e1 = new Edge(new Node(), new Node());
            Edge e2 = new Edge(new Node(), new Node());
            t.VisitingNode += (s, e) => visitedNodes.Add(e.Node);

            t.Traverse(ComponentTraversalStrategy.BFS, new INode[] { e1.Source, e1.Target, e2.Source, e2.Target });

            Assert.Equal(new INode[] { e1.Source, e1.Target, e2.Source, e2.Target }, visitedNodes);
        }

        [Fact]
        public void TraverseAndSearchForNode()
        {
            VisitedNodeCollection visited = new VisitedNodeCollection();
            Node n1 = new Node();
            Edge e1 = new Edge(n1, new Node());
            Edge e2 = new Edge(n1, new Node());

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

            Node n1, n2;
            Edge e1 = new Edge(n1 = new Node(), new Node());
            Edge e2 = new Edge(n2 = new Node(), new Node());
            t.VisitedComponent += (s, e) => componentRoots.Add(e.RootNode);

            t.Traverse(ComponentTraversalStrategy.BFS, e1.Source, e1.Target, e2.Source, e2.Target);

            Assert.Equal(new INode[] { n1, n2 }, componentRoots);
        }

    }
}
