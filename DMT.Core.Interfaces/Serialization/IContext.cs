using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.Core.Interfaces.Serialization
{
    public interface IContext
    {
        Dictionary<IId, INode> Nodes { get; }
    }
}
