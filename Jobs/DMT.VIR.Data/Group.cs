using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;

namespace DMT.VIR.Data
{
    public class Group : VirNode
    {
        const string NameTag = "Name";

        public string Name { get; set; }

        public Group(IEntityFactory factory)
            : base(factory)
        {
        }

        public override void Serialize(XmlWriter writer)
        {
            base.Serialize(writer);
            writer.WriteElementString(NameTag, this.Name);
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            base.Deserialize(reader, context);
            reader.ReadToFollowing(NameTag);
            this.Name = reader.Value;
        }
    }
}
