using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Graph;

namespace DMT.Core.Graph
{
    public abstract class ComponentTraverser
    {
        public event VisitedNodeEventHandler VisitedNode;
        public event VisitingNodeEventHandler VisitingNode;

        /// <summary>
        /// Traverses the graph component
        /// </summary>
        /// <param name="root"></param>
        /// <returns>the node that we looked for, or null</returns>
        public abstract INode Traverse(INode root);

        protected void OnVisitedNode(INode node)
        {
            var handler = this.VisitedNode;
            if (handler != null)
            {
                handler(this, new VisitedNodeEventArgs(node));
            }
        }

        protected void OnVisitingNode(VisitingNodeEventArgs e)
        {
            var handler = this.VisitingNode;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public delegate void VisitedNodeEventHandler(object sender, VisitedNodeEventArgs e);

    public class VisitedNodeEventArgs : EventArgs
    {
        public INode Node { get; private set; }

        public VisitedNodeEventArgs(INode node)
        {
            this.Node = node;
        }
    }
}
