using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Interfaces;
using DMT.Partition.Module.Remote;

namespace DMT.Partition.Module
{
    internal class PartitionSelector
    {
        private MatcherRegistry matcherRegistry;
        private PartitionRegistry partitionRegistry;

        public PartitionSelector(MatcherRegistry matcherRegistry, PartitionRegistry partitionRegistry)
        {
            this.matcherRegistry = matcherRegistry;
            this.partitionRegistry = partitionRegistry;
        }

        /// <summary>
        /// Select a partition from the registry.
        /// </summary>
        /// <param name="matcherId">the matcher id to select the partition for</param>
        /// <returns>the partition</returns>
        /// <exception cref="InvalidOperationException">when the matcher already has a partition</exception>
        public IPartition Select(Guid matcherId)
        {
            var matcher = this.matcherRegistry.GetById(matcherId);
            var partition = this.partitionRegistry.GetNextPartition();

            if (matcher.Partition != null)
            {
                throw new InvalidOperationException(
                    string.Format("A partition ({0}) was already sent to the matcher ({1}).", matcher.Partition.Id, matcher.Id));
            }

            // set the partition for the matcher
            matcher.Partition = partition;

            return partition;
        }

    }
}
