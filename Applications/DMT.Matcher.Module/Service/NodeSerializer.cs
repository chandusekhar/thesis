using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using DMT.Common.Composition;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Module.Service
{
    internal class NodeSerializer
    {
        private const string NodeTag = "Node";
        private const string TypeAttr = "type";

        private Stream stream;

        [Import]
        private IEntityFactory factory;

        public NodeSerializer(Stream stream)
        {
            CompositionService.Default.InjectOnce(this);
            this.stream = stream;
        }

        public NodeSerializer(Stream stream, IEntityFactory factory)
        {
            this.stream = stream;
            this.factory = factory;
        }

        public void Serialize(INode node)
        {
            using (XmlWriter writer = XmlWriter.Create(this.stream, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement(NodeTag);
                writer.WriteAttributeString(TypeAttr, node.GetType().Name);

                node.Serialize(writer);

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
            // HACK: use factory for context creation
            //  null migth not be sufficient in the future
            node.Deserialize(reader, null);

            return node;
        }
    }
}
