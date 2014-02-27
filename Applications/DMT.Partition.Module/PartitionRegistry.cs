using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Interfaces;
using DMT.Partition.Module.Exceptions;

namespace DMT.Partition.Module
{
    class PartitionRegistry
    {
        private List<IPartition> partitions;
        private int lastSelectedPartition = -1;

        public PartitionRegistry(IEnumerable<IPartition> partitions)
        {
            this.partitions = new List<IPartition>(partitions);
        }

        /// <summary>
        /// Gets a partition that has not been sent out to any matcher.
        /// </summary>
        /// <returns>the next partition to send out</returns>
        /// <exception cref="InvalidOperationException">When every partition has been sent out.</exception>
        public IPartition GetNextPartition()
        {
            if (this.partitions.Count <= this.lastSelectedPartition + 1)
            {
                throw new NoMorePartitionException("There are no more partitions to send out.");
            }

            lock (this)
            {
                ++this.lastSelectedPartition;
                return this.partitions[lastSelectedPartition];
            }
        }
    }
}
