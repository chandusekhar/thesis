using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Serialization
{
    /// <summary>
    /// Basic xml implementation of the model loader/saver. 
    /// 
    /// It only loads only the a stub of nodes and edges.
    /// Loads the node's and edge's id-s, and also it builds the connection between
    /// 
    /// </summary>
    [Export(typeof(IDataSource))]
    public class XmlDataSource : IDataSource
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const string NodesTag = "Nodes";
        public const string NodeTag = "Node";
        public const string EdgesTag = "Edges";
        public const string EdgeTag = "Edge";
        public const string RootTag = "Model";

        private IEntityFactory entityFactory;

        [ImportingConstructor]
        public XmlDataSource(IEntityFactory entityFactory)
        {
            this.entityFactory = entityFactory;
        }


        public void SaveModel(Stream stream, IModel model)
        {
            logger.Debug("Model saving started");

            using (var writer = XmlWriter.Create(stream))
            {
                // opening tag
                writer.WriteStartDocument();
                writer.WriteStartElement(XmlDataSource.RootTag);

                IEnumerable<INode> nodes = model.Nodes;
                IEnumerable<IEdge> edges = CollectEdges(model);

                // nodes
                logger.Debug("Saving nodes.");
                WriteCollectionToXml(nodes, writer, XmlDataSource.NodesTag, XmlDataSource.NodeTag);
                logger.Debug("End of saving nodes.");

                // edges
                logger.Debug("Saving edges.");
                WriteCollectionToXml(edges, writer, XmlDataSource.EdgesTag, XmlDataSource.EdgeTag);
                logger.Debug("End of saving edges.");

                // closing root
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            logger.Debug("Model saving done.");
        }

        public IModel LoadModel(Stream stream)
        {
            logger.Debug("Started loading model.");
            IContext context = new DeserializationContext(entityFactory);
            List<INode> nodeList;

            using (var reader = XmlReader.Create(stream))
            {
                // read nodes
                logger.Trace("Starting to read nodes.");
                if (!reader.ReadToFollowing(XmlDataSource.NodesTag))
                {
                    logger.Debug("No 'Nodes' tag found in the xml.");
                    throw new ModelXmlFormatException();
                }
                nodeList = ReadNodes(reader.ReadSubtree(), context);
                logger.Trace("Reading nodes done.");

                // read edges, and connect nodes
                logger.Trace("Starting to read edges.");
                if (!reader.ReadToFollowing(XmlDataSource.EdgesTag))
                {
                    logger.Debug("No 'Edges' tag found in the xml.");
                    throw new ModelXmlFormatException();
                }
                // loading edge: it connects appropriate nodes on the way
                ReadEdges(reader.ReadSubtree(), context);
                logger.Trace("Reading edges done.");
            }

            logger.Debug("Finished loading model.");
            return new Model(nodeList);
        }

        private void ReadEdges(XmlReader reader, IContext context)
        {
            while (reader.ReadToFollowing(XmlDataSource.EdgeTag))
            {
                IEdge edge = context.EntityFactory.CreateEdge();
                edge.Deserialize(reader.ReadSubtree(), context);
            }
        }

        private List<INode> ReadNodes(XmlReader reader, IContext context)
        {
            List<INode> nodes = new List<INode>();
            while (reader.ReadToFollowing(XmlDataSource.NodeTag))
            {
                INode node = context.EntityFactory.CreateNode();
                node.Deserialize(reader.ReadSubtree(), context);
                nodes.Add(node);
                context.AddNode(node);
            }

            return nodes;
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

        #endregion
    }
}
