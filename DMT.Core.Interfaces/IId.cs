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
    public interface IId : ISerializable, IEquatable<IId>
    {
    }
}
