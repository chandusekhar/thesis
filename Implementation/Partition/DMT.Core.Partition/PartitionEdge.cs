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
    internal class PartitionEdge : Edge, IPartitionEdge
    {
        private bool? isInnerCache = null;

        public bool IsInner
        {
            get
            {
                if (!this.isInnerCache.HasValue)
                {
                    this.isInnerCache = this.IsInnerEdge();
                }

                return this.isInnerCache.Value;
            }
        }

        public PartitionEdge(IEntityFactory factory) : base(factory) { }

        public PartitionEdge(INode nodeA, INode nodeB, EdgeDirection direction, IEntityFactory factory) : base(nodeA, nodeB, direction, factory) { }

        public IPartition GetOtherPartition(IPartition partition)
        {
            if (partition == null)
            {
                throw new ArgumentNullException("partition");
            }

            var nodeA = (IPartitionNode)this.EndA;
            var nodeB = (IPartitionNode)this.EndB;

            if (nodeA.Partition == partition)
            {
                return nodeB.Partition;
            }

            return nodeA.Partition;
            
        }

        public IPartition GetOtherPartition(IPartitionNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return this.GetOtherPartition(node.Partition);
        }

        private bool IsInnerEdge()
        {
            var nodeA = this.EndA as IPartitionNode;
            var nodeB = this.EndB as IPartitionNode;

            if (nodeA == null || nodeB == null)
            {
                throw new InvalidOperationException("EndA or EndB is not a IPartitionNode");
            }

            // NOTE: maybe an overridden Equals would be more suitable?
            return nodeA.Partition == nodeB.Partition;
        }
    }
}
