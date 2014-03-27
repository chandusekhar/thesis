using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.VIR.Data
{
    public class SemesterValuation : VirNode
    {
        public Semester Semester { get; set; }
        public int Score { get; set; }
        public string State { get; set; }

        public SemesterValuation(IEntityFactory factory)
            : base(factory)
        {

        }

        public override void Serialize(System.Xml.XmlWriter writer)
        {
            base.Serialize(writer);

            writer.WriteStartElement("Semester");
            Semester.Serialize(writer);
            writer.WriteEndElement();

            writer.WriteElementString("Score", this.Score.ToString());
            writer.WriteElementString("State", this.State);
        }

    }
}
