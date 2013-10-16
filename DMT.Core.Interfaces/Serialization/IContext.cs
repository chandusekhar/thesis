using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Core.Interfaces;

namespace DMT.Core.Serialization
{
    public interface IContext
    {
        Dictionary<IId, INode> Nodes { get; }
    }
}
