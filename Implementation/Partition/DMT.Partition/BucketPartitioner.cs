using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;
using NLog;

namespace DMT.Partition
{
    /// <summary>
    /// This implementation sequentially fills up partitions with nodes like filling up buckets.
    /// It does not take edges between nodes into account.
    /// </summary>
    [Export(typeof(IPartitioner))]
    internal class BucketPartitioner : IPartitioner
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
            logger.Info("Starting partitioning, number of nodes: {0}", nodes.Count());
            List<IPartition> partitions = new List<IPartition>();

            IPartition current = factory.CreatePartition();
            partitions.Add(current);

            int count = 0,
                size = 0;
            ISuperNode sn;

            foreach (var node in nodes)
            {
                sn = node as ISuperNode;
                size = sn != null ? sn.Size : 1;
                
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

            logger.Info("Partition done. Produced {0} partitions.", partitions.Count);
            return partitions;
        }
    }
}
