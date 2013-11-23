using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition
{
    /// <summary>
    /// This implementation sequentially fills up partitions with nodes like filling up buckets.
    /// It does not take edges between nodes into account.
    /// </summary>
    [Export(typeof(IPartitioner))]
    internal class BucketPartitioner : IPartitioner
    {
        private IPartitionEntityFactory factory;

        public int NumberOfPartitions { get; set; }

        public int NumberOfNodesInPartition { get; set; }

        [ImportingConstructor]
        public BucketPartitioner(IPartitionEntityFactory factory)
        {
            this.NumberOfNodesInPartition = 1000;
            this.factory = factory;
        }

        public IEnumerable<IPartition> Partition(IEnumerable<INode> nodes)
        {
            List<IPartition> partitions = new List<IPartition>();

            IPartition current = factory.CreatePartition();
            partitions.Add(current);

            int count = 0,
                size = 0;
            ISuperNode sn;

            foreach (var node in nodes)
            {
                sn = node as ISuperNode;
                if (sn != null)
                {
                    size = sn.Size;
                }
                else
                {
                    size = 1;
                }

                
                if (count + size <= this.NumberOfNodesInPartition)
                {
                    // if the node fits into the current partition, increase the node count in partition
                    count += size;
                }
                else
                {
                    // when this node does not fit into the current partition, create a new partition and add it
                    // to the partition collection
                    count = size;
                    partitions.Add(current);
                    current = this.factory.CreatePartition();
                }
                current.Nodes.Add(node);

            }

            return partitions;
        }
    }
}
