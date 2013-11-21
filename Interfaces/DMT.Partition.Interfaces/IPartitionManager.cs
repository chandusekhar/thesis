using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// An interface to orchastrate the partitioning process.
    /// </summary>
    public interface IPartitionManager
    {
        /// <summary>
        /// Partitions the model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IEnumerable<IPartition> PartitionModel(IModel model);
    }
}
