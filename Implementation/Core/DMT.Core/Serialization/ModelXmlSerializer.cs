using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Serialization
{
    [Export(typeof(IModelXmlSerializer))]
    public class ModelXmlSerializer : IModelXmlSerializer
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public const string NodesTag = "Nodes";
        public const string NodeTag = "Node";
        public const string EdgesTag = "Edges";
        public const string EdgeTag = "Edge";
        public const string RootTag = "Model";
        public const string TypeAttr = "type";

        private IEntityFactory entityFactory;

        [ImportingConstructor]
        public ModelXmlSerializer(IEntityFactory factory)
        {
            this.entityFactory = factory;
        }

        public void Serialize(XmlWriter writer, IModel model)
        {
            // opening tag
            writer.WriteStartDocument();
            writer.WriteStartElement(RootTag);

            IEnumerable<INode> nodes = model.Nodes;
            IEnumerable<IEdge> edges = CollectEdges(model);

            // nodes
            logger.Debug("Saving nodes.");
            WriteCollectionToXml(nodes, writer, NodesTag, NodeTag);
            logger.Debug("End of saving nodes.");

            // edges
            logger.Debug("Saving edges.");
            WriteCollectionToXml(edges, writer, EdgesTag, EdgeTag);
            logger.Debug("End of saving edges.");

            // closing root
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public IModel Deserialize(XmlReader reader)
        {
            return this.Deserialize(reader, null);
        }

        public IModel Deserialize(XmlReader reader, Action<IEdge> edgeDeserializedCallback)
        {
            IContext context = new DeserializationContext(this.entityFactory);
            List<INode> nodeList;

            // read nodes
            logger.Trace("Starting to read nodes.");
            if (!reader.ReadToFollowing(NodesTag))
            {
                logger.Debug("No 'Nodes' tag found in the xml.");
                throw new ModelXmlFormatException();
            }
            nodeList = ReadNodes(reader.ReadSubtree(), context);
            logger.Trace("Reading nodes done.");

            // read edges, and connect nodes
            logger.Trace("Starting to read edges.");
            if (!reader.ReadToFollowing(EdgesTag))
            {
                logger.Debug("No 'Edges' tag found in the xml.");
                throw new ModelXmlFormatException();
            }
            // loading edge: it connects appropriate nodes on the way
            ReadEdges(reader.ReadSubtree(), context, edgeDeserializedCallback);
            logger.Trace("Reading edges done.");

            return new Model(nodeList);
        }

        public void CopyNodes(XmlReader source, XmlWriter destination, HashSet<IId> nodeIdSet)
        {
            if (source.ReadToFollowing(NodesTag))
            {
                destination.WriteStartElement(NodesTag);
                SelectAndCopy(NodeTag, source.ReadSubtree(), destination, nodeIdSet);
                destination.WriteEndElement();
            }
        }

        public void CopyEdges(XmlReader source, XmlWriter destination, HashSet<IId> edgeIdSet)
        {
            if (source.ReadToFollowing(EdgesTag))
            {
                destination.WriteStartElement(EdgesTag);
                SelectAndCopy(EdgeTag, source.ReadSubtree(), destination, edgeIdSet);
                destination.WriteEndElement();
            }
        }

        #region private methods

        private IEnumerable<IEdge> CollectEdges(IModel model)
        {
            var edgeDict = new Dictionary<IId, IEdge>();

            foreach (var node in model.Nodes)
            {
                foreach (var edge in node.Edges)
                {
                    if (!edgeDict.ContainsKey(edge.Id))
                    {
                        edgeDict.Add(edge.Id, edge);
                    }
                }
            }

            return edgeDict.Values;
        }

        private void WriteCollectionToXml(IEnumerable<ISerializable> elements, XmlWriter writer, string collectionTag, string elementTag)
        {
            writer.WriteStartElement(collectionTag);
            foreach (var element in elements)
            {
                writer.WriteStartElement(elementTag);
                element.Serialize(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadEdges(XmlReader reader, IContext context, Action<IEdge> edgeDeserialized)
        {
            while (reader.ReadToFollowing(EdgeTag))
            {
                IEdge edge = context.EntityFactory.CreateEdge();
                edge.Deserialize(reader.ReadSubtree(), context);

                if (edgeDeserialized != null)
                {
                    edgeDeserialized(edge);
                }
            }
        }

        private List<INode> ReadNodes(XmlReader reader, IContext context)
        {
            List<INode> nodes = new List<INode>();
            while (reader.ReadToFollowing(NodeTag))
            {
                INode node ;
                if (reader.MoveToAttribute(TypeAttr))
                {
                    node = context.EntityFactory.CreateNode(reader.Value);
                }
                else
                {
                    node = context.EntityFactory.CreateNode();
                }
                node.Deserialize(reader.ReadSubtree(), context);
                nodes.Add(node);
                context.AddNode(node);
            }

            return nodes;
        }

        private void SelectAndCopy(string tag, XmlReader reader, XmlWriter writer, HashSet<IId> ids)
        {
            while (reader.ReadToFollowing(tag))
            {
                var subreader = reader.ReadSubtree();
                subreader.Read();
                var node = (XElement)XElement.ReadFrom(subreader);
                IId id = ParseId(node);
                if (ids.Contains(id))
                {
                    writer.WriteNode(node.CreateReader(), false);
                }
            }
        }

        private IId ParseId(XElement element)
        {
            var idElement = element.Element(DMTId.IdTagName);
            if (idElement == null)
            {
                throw new ModelXmlFormatException("No id found for node.");
            }

            IId id = entityFactory.CreateId();
            var idReader = idElement.CreateReader();
            idReader.Read();
            id.Deserialize(idReader, null);

            return id;
        }

        #endregion
    }
}
