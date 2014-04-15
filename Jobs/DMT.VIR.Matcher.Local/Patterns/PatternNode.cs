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
        private List<IMatchEdge> remoteEdgesList;

        public INode MatchedNode { get; set; }

        public bool IsMatched
        {
            get { return this.MatchedNode != null; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEnumerable<IMatchEdge> RemoteEdges
        {
            get { return this.remoteEdgesList; }
        }

        public PatternNode(IEntityFactory factory)
            : this(null, factory)
        {

        }

        public PatternNode(string name, IEntityFactory factory)
            : base(factory)
        {
            this.name = name;
            this.remoteEdgesList = new List<IMatchEdge>();
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
            if (IsMatched)
            {
                writer.WriteAttributeString("type", MatchedNode.GetType().Name);
                MatchedNode.Serialize(writer);
            }
            writer.WriteEndElement();

            // TODO serialize remote edges

            writer.WriteStartElement("RemoteEdges");
            foreach (var edge in this.remoteEdgesList)
            {
                writer.WriteStartElement("RemoteEdge");
                edge.Serialize(writer);
                writer.WriteEndElement();
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

            if (reader.Name != "RemoteEdges") { reader.ReadToFollowing("RemoteEdges"); }
            if (!reader.IsEmptyElement)
            {
                while (reader.ReadToFollowing("RemoteEdge"))
                {
                    IMatchEdge e = (IMatchEdge)context.EntityFactory.CreateEdge();
                    e.Deserialize(reader, context);
                    this.remoteEdgesList.Add(e);
                }
            }
        }

        public void CopyRemoteEdgesFrom(INode node)
        {
            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                if (edge.IsRemote)
                {
                    this.remoteEdgesList.Add(edge);
                }
            }
        }

        public void Reset()
        {
            this.MatchedNode = null;
            this.remoteEdgesList.Clear();
        }
    }
}
