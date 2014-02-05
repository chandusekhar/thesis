using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Entity interface.
    /// </summary>
    public interface IEntity : IIdentifiable, IEquatable<IEntity>
    {
        /// <summary>
        /// Remove an entity from its context.
        /// 
        /// Eg for an <c>IEdge</c> cuts the connection between the <c>nodeA</c> and <c>nodeB</c> nodes.
        /// </summary>
        /// <returns>true only if the remove was successful, false otherwise</returns>
        bool Remove();
    }
}
