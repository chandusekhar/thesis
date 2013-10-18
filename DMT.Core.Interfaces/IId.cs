using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Abstraction over a general ID for nodes, edges and everything.
    /// </summary>
    public interface IId : ISerializable
    {
        /// <summary>
        /// Determines whether two ids are identical or not.
        /// </summary>
        /// <param name="other">the other id</param>
        /// <returns>true only if the two id are identical, false otherwise</returns>
        bool Equals(IId other);
    }
}
