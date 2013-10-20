using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Core.Extensions;

namespace DMT.Core.Graph
{
    /// <summary>
    /// Traversing a graph using the breadth-first search algorithm.
    /// </summary>
    internal class BFSGraphTraverser : Traverser
    {
        protected override INode TraverseComponent(INode root)
        {
            VisitedNodeCollection visited = new VisitedNodeCollection();
            Queue<INode> nodeQueue = new Queue<INode>();

            nodeQueue.Enqueue(root);
            visited.Add(root);

            INode curr = null;
            VisitingNodeEventArgs e;
            while (nodeQueue.Any())
            {
                curr = nodeQueue.Dequeue();
                OnVisitingNode(e = new VisitingNodeEventArgs(curr));

                if (e.Found)
                {
                    // if we were looking for this particular node, return it!
                    return curr;
                }

                // remove node from component cache
                this.components.Remove(curr.Id);

                foreach (var adjacentNode in curr.AdjacentNodes())
                {
                    if (visited.NotContains(adjacentNode))
                    {
                        visited.Add(adjacentNode);
                        nodeQueue.Enqueue(adjacentNode);
                    }
                }
            }

            return null;
        }
    }
}
