using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data
{
    public class RemoteNode : Node
    {
        public RemoteNode(IId id, IEntityFactory factory)
            : base(factory)
        {
            this.Id = id;
        }
    }
}
