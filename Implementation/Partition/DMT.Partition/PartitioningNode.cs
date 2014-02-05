using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition
{
    internal class PartitioningNode
    {
        public INode Node { get; set; }
        public IPartition Partition { get; set; }
        public int D { get; set; }
    }

    internal class PartitioningNodePair
    {
        public PartitioningNode NodeA { get; private set; }
        public PartitioningNode NodeB { get; private set; }
        public int Gain { get; private set; }

        public PartitioningNodePair(PartitioningNode nodeA, PartitioningNode nodeB)
        {
            this.NodeA = nodeA;
            this.NodeB = nodeB;
            this.Gain = this.NodeA.D + this.NodeB.D - 2 * Weight(NodeA.Node, NodeB.Node);
        }

        /// <summary>
        /// Every edge's weight is 1. If there is no edge between the the nodes, the weight is 0.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private int Weight(INode n1, INode n2)
        {
            if (n1.IsNeighbour(n2))
            {
                return 1;
            }
            return 0;
        }
    }
}
