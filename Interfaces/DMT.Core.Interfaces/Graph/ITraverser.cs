using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Graph
{
    /// <summary>
    /// Graph traversal. It can handle not connected graphs.
    /// </summary>
    public interface ITraverser
    {
        /// <summary>
        /// Fired when a node is being visited by the traversal algorithm.
        /// </summary>
        event VisitingNodeEventHandler VisitingNode;

        /// <summary>
        /// Fired when every node has been visited in a graph component.
        /// If a node found the run of the traversal will be aborted and this event may not be fired.
        /// </summary>
        event VisitedComponentEventHandler VisitedComponent;

        /// <summary>
        /// Traverses the graph and fires the <c>VisitingNode</c> event when a new node found and
        /// being visted.
        /// </summary>
        /// <param name="roots">The collection of all nodes in the gragh.</param>
        /// <param name="strategy">The strategy used to visit all nodes of a graph component.</param>
        /// <returns>The found node or null if nothing was found.</returns>
        INode Traverse(IEnumerable<INode> roots, ComponentTraversalStrategy strategy);
    }

    public delegate void VisitingNodeEventHandler(object sender, VisitingNodeEventArgs e);

    public delegate void VisitedComponentEventHandler(object sender, VisitedComponentEventArgs e);

    public class VisitingNodeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the node being visited.
        /// </summary>
        public INode Node { get; private set; }

        /// <summary>
        /// Gets or sets if the node has been found.
        ///
        /// If set to true in the event handler, the traverser will quit after processing this node.
        /// </summary>
        public bool Found { get; set; }

        public VisitingNodeEventArgs(INode node)
        {
            this.Node = node;
            this.Found = false;
        }
    }

    public class VisitedComponentEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the root node from which the traversal has been started.
        /// </summary>
        public INode RootNode { get; private set; }

        public VisitedComponentEventArgs(INode root)
        {
            this.RootNode = root;
        }
    }
}
