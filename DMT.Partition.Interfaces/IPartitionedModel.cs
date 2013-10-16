using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// The result of partitioning a model. It can have many paritions.
    /// </summary>
    public interface IPartitionedModel
    {
        /// <summary>
        /// Gets the partitions of the model.
        /// </summary>
        IEnumerable<IPartition> Partitions { get; }
    }
}
