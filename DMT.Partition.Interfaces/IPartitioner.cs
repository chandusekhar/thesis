using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    public interface IPartitioner
    {
        IPartitionedModel Partition(ICollection<INode> nodes);

        // TODO: incremental update for partitions
    }
}
