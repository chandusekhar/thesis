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

        /// <summary>
        /// Creates an ISuperNode object and wraps the <c>node</c> in it.
        /// </summary>
        /// <param name="node">The node to wrap.</param>
        /// <returns></returns>
        ISuperNode CreateSuperNode(INode node);
    }
}
