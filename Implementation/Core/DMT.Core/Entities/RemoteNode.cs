using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core.Entities
{
    public class RemoteNode : Node, IRemoteNode
    {
        public RemoteNode(IId id, IEntityFactory factory)
            : base(factory)
        {
            this.Id = id;
        }
    }
}
