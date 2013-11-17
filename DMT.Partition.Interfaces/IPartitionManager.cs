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
        /// Gets coarsener.
        /// </summary>
        ICoarsener Coarsener { get; }

        /// <summary>
        /// Gets the partition refiner.
        /// </summary>
        IPartitionRefiner Refiner { get; }
        
        /// <summary>
        /// Partitions the model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IEnumerable<IPartition> PartitionModel(IModel model);

    }
}
