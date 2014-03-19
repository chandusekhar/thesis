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
using DMT.Matcher.Data.Interfaces;
using DMT.Module.Common;

namespace DMT.Matcher.Module.Partitioner
{
    /// <summary>
    /// Deserialize matcher data (partition nodes and edges) sent from partition module
    /// </summary>
    class MatcherDataDeserializer : InjectableBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IEntityFactory entityFactory;

        [Import]
        private IModelXmlSerializer modelSerializer;

        public IModel Deserialize(Stream stream)
        {
            logger.Info("Matcher data deserialization started.");
            XmlReader reader = XmlReader.Create(stream);
            Dictionary<IId, CrossingEdge> crossingEdges = null;
            IModel model = null;

            if (reader.ReadToFollowing(PartitionSerializerTags.CrossingEdgesTag))
            {
                crossingEdges = ReadCrossingEdges(reader.ReadSubtree());
            }
            if (reader.ReadToFollowing(PartitionSerializerTags.ModelTag))
            {
                // Data fromat comes from is in DMT.Partition.Module.Remote.Service.PartitionSerializer
                model = modelSerializer.Deserialize(reader.ReadSubtree(), e =>
                {
                    IMatchEdge edge = (IMatchEdge)e;
                    // if crossing edge, mark it that way
                    if (crossingEdges.ContainsKey(edge.Id))
                    {
                        edge.RemotePartitionId = crossingEdges[edge.Id].PartitionId;
                    }
                });
            }
            logger.Info("Deserialization finished. Loaded {0} node(s).", model.Nodes.Count);
            return model;
        }

        private Dictionary<IId, CrossingEdge> ReadCrossingEdges(XmlReader subReader)
        {
            IId edgeId, partitionId;
            Dictionary<IId, CrossingEdge> crossingEdges = new Dictionary<IId, CrossingEdge>();

            while (subReader.ReadToFollowing(PartitionSerializerTags.CrossingEdgeTag))
            {
                subReader.ReadToFollowing(PartitionSerializerTags.IdTag);
                edgeId = ParseId(subReader);

                subReader.ReadToFollowing(PartitionSerializerTags.PartitionIdTag);
                partitionId = ParseId(subReader);

                crossingEdges.Add(edgeId, new CrossingEdge(edgeId, partitionId));
            }

            return crossingEdges;
        }

        private IId ParseId(XmlReader reader)
        {
            IId id = entityFactory.CreateId();
            id.Deserialize(reader, null);
            return id;
        }
    }
}
