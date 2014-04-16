using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Data
{
    public abstract class VirNode : Node, IMatchNode
    {
        const string TypeAttr = "type";

        public VirNode(IEntityFactory factory)
            : base(factory)
        {

        }

        public static IEqualityComparer<INode> EqualityComparer()
        {
            return new VirNodeComparer();
        }

        public void SortEdges()
        {
            this.edges.Sort(new Comparison<IEdge>(CompareEdgesBasedOnRemoteness));
        }

        // sort edges in a way that remote edges end up toward the end of list
        private int CompareEdgesBasedOnRemoteness(IEdge e1, IEdge e2)
        {
            IMatchEdge me1 = (IMatchEdge)e1;
            IMatchEdge me2 = (IMatchEdge)e2;

            bool bothRemote = me1.IsRemote && me2.IsRemote;
            bool bothLocal = !me1.IsRemote && !me2.IsRemote;

            if (bothLocal || bothRemote)
            {
                return 0;
            }

            if (me1.IsRemote)
            {
                return 1;
            }

            if (me2.IsRemote)
            {
                return -1;
            }

            throw new InvalidOperationException("Cannot decide which edge comes first...");
        }

        private class VirNodeComparer : IEqualityComparer<INode>
        {
            public bool Equals(INode x, INode y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                if ((x == null && y != null) || (x != null && y == null))
                {
                    return false;
                }

                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(INode obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
