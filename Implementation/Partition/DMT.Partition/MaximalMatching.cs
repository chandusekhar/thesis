using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Extensions;
using DMT.Core.Interfaces;

namespace DMT.Partition
{
    /// <summary>
    /// This class produces a maximal matching based on a given node set.
    /// There is no guarantee that the matching is a maximum matching (containing the
    /// maximum number of edges). It uses a simple greedy algorithm:
    /// 
    /// 1. M = empty set
    /// 2.While(no more edges can be added)
    ///     2.1 Select an edge e which does not have any vertex in common with edges in M
    ///     2.2 M = M U e
    /// 3. return M
    /// </summary>
    internal class MaximalMatching
    {
        private IEnumerable<INode> nodes;
        private List<IEdge> matching = new List<IEdge>();
        private HashSet<IEdge> edges = new HashSet<IEdge>();
        private bool hasBeenProcessed = false;

        /// <summary>
        /// All the nodes that have an edge in the matching
        /// </summary>
        private HashSet<INode> markedNodes = new HashSet<INode>();

        public ReadOnlyCollection<IEdge> Matching
        {
            get
            {
                if (hasBeenProcessed)
                {
                    return new ReadOnlyCollection<IEdge>(this.matching);
                }

                throw new InvalidOperationException("Graph has not been processed yet.");
            }
        }

        public MaximalMatching(IEnumerable<INode> nodes)
        {
            this.nodes = nodes;
            CollectEdges();
        }

        public IEnumerable<IEdge> Find()
        {
            IEdge selected = null;
            while ((selected = GetNextEdge()) != null)
            {
                this.matching.Add(selected);
            }

            this.hasBeenProcessed = true;
            return this.matching;
        }

        private IEdge GetNextEdge()
        {
            IEdge selected = null;
            // contains the edges that cannot be in the matching
            List<IEdge> invalidEdges = new List<IEdge>();

            foreach (var edge in this.edges)
            {
                if (this.markedNodes.Contains(edge.EndA) || this.markedNodes.Contains(edge.EndB))
                {
                    invalidEdges.Add(edge);
                    continue;
                }

                // mark nodes
                this.markedNodes.Add(edge.EndA);
                this.markedNodes.Add(edge.EndB);
                invalidEdges.Add(edge);
                // TODO: add EndA and EndB other edges to invalid edges, issue #6
                selected = edge;
                break;
            }

            // remove edges that cannot be used
            this.edges.ExceptWith(invalidEdges);

            return selected;
        }

        private void CollectEdges()
        {
            foreach (var node in this.nodes)
            {
                edges.AddAll(node.Edges);
            }
        }
    }
}
