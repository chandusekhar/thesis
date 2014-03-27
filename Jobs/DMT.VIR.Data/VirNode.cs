using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Entities;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data
{
    public abstract class VirNode : Node
    {
        const string TypeAttr = "type";

        public VirNode(IEntityFactory factory)
            : base(factory)
        {

        }

        public override void Serialize(XmlWriter writer)
        {
            writer.WriteAttributeString(TypeAttr, this.GetType().Name);
            base.Serialize(writer);
        }
    }
}
