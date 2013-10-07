using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    public interface IIdentity
    {
        /// <summary>
        /// Gets the unique identifier for the entity.
        /// </summary>
        uint Id { get; }
    }
}
