using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Module.Common;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionSerializer : InjectableBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IModelXmlSerializer modelSerializer;

        public void Serialize(IPartition partition, XmlReader reader, Stream dest)
        {
            using (var writer = XmlWriter.Create(dest, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(PartitionSerializerTags.PartitionTag);

                WriteCrossingEdges(partition, writer);
                WriteNodesAndEdges(partition, reader, writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void WriteCrossingEdges(IPartition partition, XmlWriter writer)
        {
            writer.WriteStartElement(PartitionSerializerTags.CrossingEdgesTag);

            IPartitionEdge edge;
            foreach (var item in partition.GetExternalEdges())
            {
                writer.WriteStartElement(PartitionSerializerTags.CrossingEdgeTag);

                edge = (IPartitionEdge)item;

                // edge id
                writer.WriteStartElement(PartitionSerializerTags.IdTag);
                edge.Id.Serialize(writer);
                writer.WriteEndElement();

                // id of the 'remote' partition
                writer.WriteStartElement(PartitionSerializerTags.PartitionIdTag);
                edge.GetOtherPartition(partition).Id.Serialize(writer);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WriteNodesAndEdges(IPartition partition, XmlReader reader, XmlWriter writer)
        {
            writer.WriteStartElement(PartitionSerializerTags.ModelTag);

            var nodeIds = new HashSet<IId>(partition.Nodes.Select(n => n.Id));
            this.modelSerializer.CopyNodes(reader, writer, nodeIds);

            var edgeIds = new HashSet<IId>(partition.CollectEdges().Select(e => e.Id));
            this.modelSerializer.CopyEdges(reader, writer, edgeIds);

            writer.WriteEndElement();
        }

    }
}
