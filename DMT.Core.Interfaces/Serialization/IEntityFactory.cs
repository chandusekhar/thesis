using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces.Serialization
{
    /// <summary>
    /// Interface for entity creation.
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// Create an id that implements the <c>IId</c> interface.
        /// </summary>
        /// <returns>A new id.</returns>
        IId CreateId();
    }
}
