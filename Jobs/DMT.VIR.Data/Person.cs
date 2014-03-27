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
    public class Person : VirNode
    {
        const string FirstNameTag = "FirstName";
        const string LastNameTag = "LastName";

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", this.FirstName, this.LastName); }
        }

        public Person(IEntityFactory factory)
            : base(factory)
        {

        }

        public override void Serialize(XmlWriter writer)
        {
            base.Serialize(writer);

            writer.WriteElementString(FirstNameTag, this.FirstName);
            writer.WriteElementString(LastNameTag, this.LastName);
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            base.Deserialize(reader, context);

            // reader is on the firstname element
            this.FirstName = reader.ReadElementContentAsString();
            // reader is on the lastname element
            this.LastName = reader.ReadElementContentAsString();
        }
    }
}
