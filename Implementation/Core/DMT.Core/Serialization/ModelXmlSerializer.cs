﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core.Serialization
{
    [Export(typeof(IModelXmlSerializer))]
    class ModelXmlSerializer : IModelXmlSerializer
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public const string NodesTag = "Nodes";
        public const string NodeTag = "Node";
        public const string EdgesTag = "Edges";
        public const string EdgeTag = "Edge";
        public const string RootTag = "Model";

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
            ReadEdges(reader.ReadSubtree(), context);
            logger.Trace("Reading edges done.");

            return new Model(nodeList);
        }

        public void CopyNodes(XmlReader source, XmlWriter destination, HashSet<IId> nodeIdSet)
        {
            throw new NotImplementedException();
        }

        public void CopyEdges(XmlReader source, XmlWriter destination, HashSet<IId> edgeIdSet)
        {
            throw new NotImplementedException();
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

        private void ReadEdges(XmlReader reader, IContext context)
        {
            while (reader.ReadToFollowing(EdgeTag))
            {
                IEdge edge = context.EntityFactory.CreateEdge();
                edge.Deserialize(reader.ReadSubtree(), context);
            }
        }

        private List<INode> ReadNodes(XmlReader reader, IContext context)
        {
            List<INode> nodes = new List<INode>();
            while (reader.ReadToFollowing(NodeTag))
            {
                INode node = context.EntityFactory.CreateNode();
                node.Deserialize(reader.ReadSubtree(), context);
                nodes.Add(node);
                context.AddNode(node);
            }

            return nodes;
        }

        #endregion
    }
}
