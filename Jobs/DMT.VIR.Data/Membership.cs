using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data
{
    public class Membership : VirNode
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public String[] Posts { get; set; }

        public bool IsActive
        {
            get { return this.End == DateTime.MinValue; }
        }

        public Membership(IEntityFactory factory)
            : base(factory)
        {

        }

        public override void Serialize(System.Xml.XmlWriter writer)
        {
            base.Serialize(writer);

            writer.WriteElementString("Start", this.Start.ToString());
            writer.WriteElementString("End", this.End.ToString());
            writer.WriteElementString("Posts", string.Join(":", this.Posts));
        }

        public override void Deserialize(System.Xml.XmlReader reader, Core.Interfaces.Serialization.IContext context)
        {
            base.Deserialize(reader, context);

            this.Start = DateTime.Parse(reader.ReadElementContentAsString());
            this.End = DateTime.Parse(reader.ReadElementContentAsString());
            this.Posts = reader.ReadElementContentAsString().Split(':');
        }
    }
}
