using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Module.Service
{
    internal class NodeSerializer
    {
        private const string RemoteTag = "Remote";
        private const string NodeTag = "Node";
        private const string TypeAttr = "type";
        private const string EdgesTag = "Edges";
        private const string EdgeTag = "Edge";

        private Stream stream;

        [Import]
        private IEntityFactory factory;

        [Import]
        private IContextFactory contextFactory;

        public NodeSerializer(Stream stream)
        {
            CompositionService.Default.InjectOnce(this);
            this.stream = stream;
        }

        public NodeSerializer(Stream stream, IEntityFactory factory, IContextFactory cFactory)
        {
            this.stream = stream;
            this.factory = factory;
            this.contextFactory = cFactory;
        }

        public void Serialize(INode node)
        {
            using (XmlWriter writer = XmlWriter.Create(this.stream, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(RemoteTag);

                // write node
                writer.WriteStartElement(NodeTag);
                writer.WriteAttributeString(TypeAttr, node.GetType().Name);
                node.Serialize(writer);
                writer.WriteEndElement();

                // write edges
                writer.WriteStartElement(EdgesTag);
                foreach (var edge in node.Edges.Cast<IMatchEdge>())
                {
                    new EdgeWrapper(edge, node).WriteWithName(writer, EdgeTag);
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public INode Deserialize()
        {
            XmlReader reader = XmlReader.Create(this.stream);
            if (!reader.ReadToFollowing(NodeTag))
            {
                throw new InvalidDataException("No Node tag was found.");
            }

            string type = reader.GetAttribute(TypeAttr);
            var node = factory.CreateNode(type);
            node.Deserialize(reader, this.contextFactory.CreateContext());

            while (reader.ReadToFollowing(EdgeTag))
            {
                new EdgeWrapper(node, factory).Read(reader.ReadSubtree());
            }

            return node;
        }

        private class EdgeWrapper
        {
            private INode node;
            private IEntityFactory factory;
            public IMatchEdge Edge { get; set; }

            public EdgeWrapper(INode node, IEntityFactory factory)
            {
                this.node = node;
                this.factory = factory;
            }

            public EdgeWrapper(IMatchEdge edge, INode node)
            {
                this.Edge = edge;
                this.node = node;
            }

            public void WriteWithName(XmlWriter writer, string tag)
            {
                writer.WriteStartElement(tag);

                writer.WriteStartElement("Neighbour");
                this.Edge.GetOtherNode(node).Id.Serialize(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("Id");
                this.Edge.Id.Serialize(writer);
                writer.WriteEndElement();

                writer.WriteStartElement("Remote");
                writer.WriteValue(this.Edge.IsRemote);
                writer.WriteEndElement();

                if (this.Edge.IsRemote)
                {
                    writer.WriteStartElement("Partition");
                    this.Edge.RemotePartitionId.Serialize(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            public void Read(XmlReader xmlReader)
            {
                xmlReader.ReadToFollowing("Neighbour");
                var nid = this.factory.CreateId();
                nid.Deserialize(xmlReader, null);
                var remoteNode = factory.CreateRemoteNode(nid);
                // HACK: direction might be important. :(
                this.Edge = (IMatchEdge)this.node.ConnectTo(remoteNode, EdgeDirection.Both);

                this.Edge.Id.Deserialize(xmlReader, null);

                bool remote = xmlReader.ReadElementContentAsBoolean();
                if (remote)
                {
                    this.Edge.RemotePartitionId = factory.CreateId();
                    this.Edge.RemotePartitionId.Deserialize(xmlReader, null);
                }

            }
        }
    }
}
