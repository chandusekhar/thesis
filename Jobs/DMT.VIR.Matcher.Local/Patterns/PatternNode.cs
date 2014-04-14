using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Matcher.Local.Patterns
{
    public class PatternNode : Node
    {
        private string name;

        public INode MatchedNode { get; set; }

        public bool IsMatched
        {
            get { return this.MatchedNode != null; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public PatternNode(IEntityFactory factory)
            : base(factory)
        {

        }

        public PatternNode(string name, IEntityFactory factory)
            : base(factory)
        {
            this.name = name;
        }

        public PatternNode Copy()
        {
            var pn = new PatternNode(this.name, this.factory);
            pn.MatchedNode = this.MatchedNode;
            return pn;
        }

        public override string ToString()
        {
            return string.Format("PatternNode: {0}, IsMatched: {1}", this.Name, this.IsMatched);
        }

        public override void Serialize(XmlWriter writer)
        {
            base.Serialize(writer);

            writer.WriteElementString("Name", name);
            writer.WriteStartElement("MatchedNode");
            if (IsMatched) {

                writer.WriteAttributeString("type", MatchedNode.GetType().Name);
                MatchedNode.Serialize(writer);
            }
            writer.WriteEndElement();
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            base.Deserialize(reader, context);

            this.name = reader.ReadElementContentAsString();
            if (!reader.IsEmptyElement)
            {
                var type = reader.GetAttribute("type");
                this.MatchedNode = this.factory.CreateNode(type);
                this.MatchedNode.Deserialize(reader.ReadSubtree(), context);
            }
        }
    }
}
