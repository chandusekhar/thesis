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
using DMT.Core.Interfaces.Serialization;
using DMT.Partition.Interfaces;

namespace DMT.Module.Common
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

        private IModelXmlSerializer modelSerializer;

        [ImportingConstructor]
        public PartitionSerializer(IModelXmlSerializer serializer)
        {
            this.modelSerializer = serializer;
        }

        public void Serialize(IPartition partition, XmlReader reader, Stream dest)
        {
            using (var writer = XmlWriter.Create(dest, new XmlWriterSettings { CloseOutput = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(PartitionSerializer.PartitionTag);

                WriteCrossingEdges(partition, writer);

                var nodeIds = new HashSet<IId>(partition.Nodes.Select(n => n.Id));
                this.modelSerializer.CopyNodes(reader, writer, nodeIds);

                var edgeIds = new HashSet<IId>(partition.CollectEdges().Select(e => e.Id));
                this.modelSerializer.CopyEdges(reader, writer, edgeIds);

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
    }
}
