using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Interfaces
{
    /// <summary>
    /// Entity factory with partition specific entity types.
    /// </summary>
    public interface IPartitionEntityFactory
    {
        /// <summary>
        /// Creates a ISuperNode object.
        /// </summary>
        /// <returns></returns>
        ISuperNode CreateSuperNode();
    }
}
