using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Graph;
using NLog;

namespace DMT.Core.Graph
{
    [Export(typeof(ITraverser))]
    public class Traverser : ITraverser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<IId, INode> components;

        #region ITraverser

        public event VisitingNodeEventHandler VisitingNode;

        public event VisitedComponentEventHandler VisitedComponent;

        public INode Traverse(IEnumerable<INode> roots, ComponentTraversalStrategy strategy)
        {
            var compTrav = GetTraverserForStrategy(strategy);
            BuildComponentCache(roots);

            INode result, root;
            while (this.components.Any())
            {
                root = this.components.First().Value;
                result = compTrav.Traverse(root);

                // in case we found something
                if (result != null)
                {
                    logger.Debug("Node found: {0}", result);
                    UnsubscribeFromComponentTraverser(compTrav);
                    return result;
                }

                OnVisitedComponent(root);
            }

            UnsubscribeFromComponentTraverser(compTrav);
            return null;
        }

        #endregion

        #region private methods

        private void BuildComponentCache(IEnumerable<INode> nodes)
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

        private void OnVisitingNode(VisitingNodeEventArgs e)
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

        private ComponentTraverser GetTraverserForStrategy(ComponentTraversalStrategy strategy)
        {
            ComponentTraverser instance;
            switch (strategy)
            {
                case ComponentTraversalStrategy.BFS:
                    instance = new BFSGraphTraverser();
                    break;
                case ComponentTraversalStrategy.DFS:
                    instance = new DFSGraphTraverser();
                    break;
                default:
                    logger.Fatal("Not supported component traversal strategy: {0}", strategy);
                    throw new InvalidOperationException("Not supported component traversal strategy.");
            }

            instance.VisitedNode += this.HandleVisitedNode;
            instance.VisitingNode += this.HandleVisitingNodeInComponent;

            return instance;
        }

        private void HandleVisitedNode(object sender, VisitedNodeEventArgs e)
        {
            // remove node from component cache
            this.components.Remove(e.Node.Id);
        }

        private void HandleVisitingNodeInComponent(object sender, VisitingNodeEventArgs e)
        {
            OnVisitingNode(e);
        }

        private void UnsubscribeFromComponentTraverser(ComponentTraverser ct)
        {
            ct.VisitingNode -= HandleVisitingNodeInComponent;
            ct.VisitedNode -= HandleVisitedNode;
        }

        #endregion
    }
}
