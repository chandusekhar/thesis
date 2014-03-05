using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Partition.Interfaces;

namespace DMT.Partition
{
    /// <summary>
    /// This class serializes a partition into XML. It produces a stream.
    /// 
    /// This implementation heavily depends on the xml format defined by <c>XmlDataSource</c>.
    /// </summary>
    [Export(typeof(IPartitionSerializer))]
    internal class PartitionSerializer : IPartitionSerializer
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string CrossingEdgesTag = "CrossingEdges";
        private const string EdgeTag = "Edge";
        private const string EdgesTag = "Edge";
        private const string IdTag = "Id";
        private const string PartitionIdTag = "PartitionId";
        private const string PartitionTag = "Partition";
        private const string NodeTag = "Node";
        private const string NodesTag = "Nodes";

        private IEntityFactory entityFactory;

        [ImportingConstructor]
        public PartitionSerializer(IEntityFactory entityFactory)
        {
            this.entityFactory = entityFactory;
        }

        public void Serialize(IPartition partition, Stream source, Stream dest)
        {
            using (var reader = XmlReader.Create(source))
            using (var writer = XmlWriter.Create(dest, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(PartitionSerializer.PartitionTag);

                WriteCrossingEdges(partition, writer);

                // nodes
                writer.WriteStartElement(PartitionSerializer.NodesTag);
                CopyNodes(partition, reader, writer);
                writer.WriteEndElement();

                //edges
                writer.WriteStartElement(PartitionSerializer.EdgesTag);
                CopyEdges(partition, reader, writer);
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void WriteCrossingEdges(IPartition partition, XmlWriter writer)
        {
            writer.WriteStartElement(PartitionSerializer.CrossingEdgesTag);

            IPartitionEdge edge;
            foreach (var item in partition.GetExternalEdges())
            {
                edge = (IPartitionEdge)item;

                // edge id
                writer.WriteStartElement(PartitionSerializer.EdgeTag);
                edge.Id.Serialize(writer);
                writer.WriteEndElement();

                // id of the 'remote' partition
                writer.WriteStartElement(PartitionSerializer.PartitionIdTag);
                edge.GetOtherPartition(partition).Id.Serialize(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void CopyNodes(IPartition partition, XmlReader reader, XmlWriter writer)
        {
            HashSet<IId> nodeIds = new HashSet<IId>(partition.Nodes.Select(n => n.Id));

            while (reader.ReadToFollowing(PartitionSerializer.NodeTag))
            {
                var node = (XElement)XElement.ReadFrom(reader.ReadSubtree());
                var id = ParseId(node);
                if (nodeIds.Contains(id))
                {
                    writer.WriteNode(node.CreateReader(), false);
                }
            }
        }

        private void CopyEdges(IPartition partition, XmlReader reader, XmlWriter writer)
        {
            HashSet<IId> edgeIds = new HashSet<IId>(partition.CollectEdges().Select(e => e.Id));

            while (reader.ReadToFollowing(PartitionSerializer.EdgeTag))
            {
                var edge = (XElement)XElement.ReadFrom(reader.ReadSubtree());
                var id = ParseId(edge);
                if (edgeIds.Contains(id))
                {
                    writer.WriteNode(edge.CreateReader(), false);
                }
            }
        }

        private IId ParseId(XElement element)
        {
            var idElement = element.Element(PartitionSerializer.IdTag);
            if (idElement == null)
            {
                throw new ModelXmlFormatException("No id found for node.");
            }

            IId id = entityFactory.CreateId();
            id.Deserialize(idElement.CreateReader(), null);

            return id;

        }
    }
}
