using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Core.Partition
{
    internal class MultiNode : Node, IMultiNode
    {
        List<INode> nodes = new List<INode>();

        public ICollection<INode> Nodes
        {
            get { return nodes; }
        }

        public int Size
        {
            get { return nodes.Count; }
        }
    }
}
