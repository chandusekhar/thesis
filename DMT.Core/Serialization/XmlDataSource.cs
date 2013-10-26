using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Exceptions;
using DMT.Core.Extensions;
using DMT.Core.Graph;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Graph;
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
    internal class XmlDataSource : IDataSource
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Fallback name for the model file
        /// </summary>
        public const string BaseModelFileName = "model.xml";
        public const string NodesTag = "Nodes";
        public const string NodeTag = "Node";
        public const string EdgesTag = "Edges";
        public const string EdgeTag = "Edge";
        public const string RootTag = "Model";

        private Stream stream;

        #region IDataSource

        public Task<IModel> LoadModelAsync()
        {
            return Task.Run(new Func<IModel>(LoadModel));
        }

        public Task SaveModelAsync(IModel model)
        {
            return Task.Run(() => SaveModel(model));
        } 

        #endregion

        /// <summary>
        /// For testing purposes!
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal XmlDataSource UseStream(Stream stream)
        {
            this.stream = stream;
            return this;
        }

        #region private methods

        private void SaveModel(IModel model)
        {
            logger.Debug("Model saving started");

            using (var writer = XmlWriter.Create(GetOutputStream()))
            {
                // opening tag
                writer.WriteStartDocument();
                writer.WriteStartElement(XmlDataSource.RootTag);

                IEnumerable<INode> nodes;
                IEnumerable<IEdge> edges;

                CollectNodesAndEdges(model, out nodes, out edges);

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

        private Stream GetOutputStream()
        {
            if (this.stream != null)
            {
                return this.stream;
            }

            string filepath = GetModelFilePath();
            return File.Create(filepath);
        }

        private Stream GetInputStream()
        {
            if (this.stream != null)
            {
                return this.stream;
            }

            string modelFilePath = GetModelFilePath();
            return File.OpenRead(modelFilePath);
        }

        private string GetModelFilePath()
        {
            string modelFilePath = ConfigurationManager.AppSettings[CoreConstants.ModelFileNameSettingsKey];
            if (modelFilePath == null)
            {
                logger.Debug("No model file sepcified. Falling back to default model filename.");
                modelFilePath = XmlDataSource.BaseModelFileName;
            }
            return modelFilePath;
        }

        private IModel LoadModel()
        {
            logger.Debug("Started loading model.");
            IContext context = new DeserializationContext(new CoreEntityFactory());
            List<INode> nodeList;

            using (var reader = XmlReader.Create(GetInputStream()))
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

            // at this point the whole graph is built
            // we only have to select one (root) node from each connected component.
            List<INode> componentRoots = new List<INode>();
            var t = new Traverser();
            t.VisitedComponent += (_, e) => componentRoots.Add(e.RootNode);
            t.Traverse(nodeList, ComponentTraversalStrategy.BFS);

            logger.Debug("Finished loading model.");
            return new Model(componentRoots);
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

        private void CollectNodesAndEdges(IModel model, out IEnumerable<INode> nodes, out IEnumerable<IEdge> edges)
        {
            var nodeList = new List<INode>();
            var edgeDict = new Dictionary<IId, IEdge>();

            var t = new Traverser();
            t.VisitingNode += (s, e) =>
            {
                nodeList.Add(e.Node);
                foreach (var edge in e.Node.AllEdges())
                {
                    if (!edgeDict.ContainsKey(edge.Id))
                    {
                        edgeDict.Add(edge.Id, edge);
                    }
                }
            };

            t.Traverse(model.ComponentRoots, ComponentTraversalStrategy.BFS);

            nodes = nodeList;
            edges = edgeDict.Values;
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
