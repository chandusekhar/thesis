using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data
{
    public class CommunityScore : VirNode
    {
        public Semester Semester { get; set; }
        public int Score { get; set; }

        public CommunityScore(IEntityFactory factory)
            : base(factory)
        {

        }

        public override void Serialize(System.Xml.XmlWriter writer)
        {
            base.Serialize(writer);

            writer.WriteStartElement("Semester");
            this.Semester.Serialize(writer);
            writer.WriteEndElement();

            writer.WriteElementString("Score", this.Score.ToString());
        }

        public override void Deserialize(System.Xml.XmlReader reader, Core.Interfaces.Serialization.IContext context)
        {
            base.Deserialize(reader, context);

            this.Semester = new Semester();
            this.Semester.Deserialize(reader.ReadSubtree(), context);

            reader.ReadToNextSibling("Score");
            this.Score = reader.ReadElementContentAsInt();
        }
    }
}
