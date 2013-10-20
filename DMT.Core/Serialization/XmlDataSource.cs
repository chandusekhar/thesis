using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Exceptions;
using DMT.Core.Graph;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Serialization
{
    /// <summary>
    /// Basic xml implementation of the model loader/saver. 
    /// 
    /// It only loads only the a stub of nodes and edges.
    /// Loads the node's and edge's id-s, and also it builds the connection between
    /// </summary>
    public class XmlDataSource : IDataSource
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

        private Stream stream;

        public Task<IEnumerable<INode>> LoadModelAsync()
        {
            return Task.Run(new Func<IEnumerable<INode>>(this.LoadModel));
        }

        public Task SaveModelAsync()
        {
            throw new NotImplementedException();
        }

        public XmlDataSource UseStream(Stream stream)
        {
            this.stream = stream;
            return this;
        }

        private Stream GetStream()
        {
            if (this.stream != null)
            {
                return this.stream;
            }

            string modelFilePath = ConfigurationManager.AppSettings[CoreConstants.ModelFileNameSettingsKey];
            if (modelFilePath == null)
            {
                logger.Debug("No model file sepcified. Falling back to default model filename.");
                modelFilePath = XmlDataSource.BaseModelFileName;
            }
            return File.OpenRead(modelFilePath);
        }

        private IEnumerable<INode> LoadModel()
        {
            logger.Debug("Started loading model.");
            IContext context = new DeserializationContext(new CoreEntityFactory());
            List<INode> nodeList;

            using (var reader = XmlReader.Create(GetStream()))
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
            var t = Traverser.GetDefault();
            t.VisitedComponent += (_, e) => componentRoots.Add(e.RootNode);
            t.Traverse(nodeList);

            logger.Debug("Finished loading model.");
            return componentRoots;
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
    }
}
