using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using NLog;

namespace DMT.Core.Graph
{
    /// <summary>
    /// Base class and API declaration for graph traversal.
    ///
    /// The implementation must support multiple graph segments!
    /// </summary>
    public abstract class Traverser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region static default impl

        private static Type implType = typeof(BFSGraphTraverser);

        public static Traverser GetDefault()
        {
            return (Traverser)Activator.CreateInstance(implType);
        }

        public static void OverrideDefault(Type implementingType)
        {
            if (typeof(Traverser).IsAssignableFrom(implementingType))
            {
                logger.Debug("Overriding default graph traversal implementation with {0}.", implementingType.FullName);
                Traverser.implType = implementingType;
            }
            else
            {
                logger.Warn("Could not override default graph traversal implementation with {0}. Keeping the original.",
                    implementingType.FullName);
            }
        }

        #endregion

        /// <summary>
        /// Fired when a node is being visited by the traversal algorithm.
        /// </summary>
        public event VisitingNodeEventHandler VisitingNode;

        /// <summary>
        /// Fired when every node has been visited in a graph component.
        /// If a node found the run of the traversal will be aborted and this event may not be fired.
        /// </summary>
        public event VisitedComponentEventHandler VisitedComponent;

        protected Dictionary<IId, INode> components;

        /// <summary>
        /// Traverses the graph and fires the <c>VisitingNode</c> event when a new node found and
        /// being visted.
        /// </summary>
        /// <param name="nodes">The collection of all nodes in the gragh.</param>
        /// <returns>The found node or null if nothing was found.</returns>
        public INode Traverse(IEnumerable<INode> nodes)
        {
            BuildComponentCache(nodes);

            INode result, root;
            while (this.components.Any())
            {
                root = this.components.First().Value;
                result = TraverseComponent(root);

                // in case we found something
                if (result != null)
                {
                    logger.Debug("Node found: {0}", result);
                    return result;
                }

                OnVisitedComponent(root);
            }

            return null;
        }

        protected abstract INode TraverseComponent(INode root);

        protected virtual void BuildComponentCache(IEnumerable<INode> nodes)
        {
            this.components = new Dictionary<IId, INode>();
            foreach (var node in nodes)
            {
                if (!this.components.ContainsKey(node.Id))
                {
                    this.components.Add(node.Id, node);
                }
            }
        }

        protected void OnVisitingNode(VisitingNodeEventArgs e)
        {
            var handler = this.VisitingNode;
            if (handler != null)
            {
                logger.Trace("Before firing VisitingNode event.");
                handler(this, e);
                logger.Trace("VisitingNode event fired.");
            }
        }

        private void OnVisitedComponent(INode root)
        {
            var handler = this.VisitedComponent;
            if (handler != null)
            {
                logger.Trace("Before firing VisitedComponent event.");
                handler(this, new VisitedComponentEventArgs(root));
                logger.Trace("VisitedComponent event fired.");
            }
        }
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
