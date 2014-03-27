using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.VIR.Data.Parser
{
    class GroupWrapper : EntityWrapper<Group>
    {
        public string Name { get; set; }

        public override Group CreateEntity()
        {
            return new Group(factory) { Name = this.Name };
        }
    }
}
