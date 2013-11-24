using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common;
using DMT.Core.Exceptions;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Entities
{
    public class Edge : Entity, IEdge
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const string StartNodeTag = "StartNode";
        public const string EndNodeTag = "EndNode";

        private INode source = null;
        private INode target = null;

        /// <summary>
        /// FOR TESTS ONLY!
        /// </summary>
        public Edge()
            : this(new CoreEntityFactory())
        { }

        public Edge(IEntityFactory factory)
            : base(factory)
        { }

        /// <summary>
        /// FOR TESTS ONLY!
        /// </summary>
        public Edge(INode start, INode end)
            : this(start, end, new CoreEntityFactory())
        { }

        /// <summary>
        /// Instantiates a new <c>Edge</c> object and connects its endpoints
        /// which means that source and target will be set, also <c>this</c> edge will be
        /// added to the <c>source</c> node's outbound edges collection and the
        /// the <c>target</c> node's inbound edges collections.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public Edge(INode start, INode end, IEntityFactory factory)
            : base(factory)
        {
            this.ConnectNodes(start, end);
        }

        public INode Source
        {
            get { return source; }
        }

        public INode Target
        {
            get { return target; }
        }

        #region ISerializable

        public override void Serialize(XmlWriter writer)
        {
            // id
            base.Serialize(writer);

            // startnode
            writer.WriteStartElement(Edge.StartNodeTag);
            source.Id.Serialize(writer);
            writer.WriteEndElement();

            // endnode
            writer.WriteStartElement(Edge.EndNodeTag);
            target.Id.Serialize(writer);
            writer.WriteEndElement();
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            logger.Trace("Starting Core.Edge deserialization.");

            // id
            base.Deserialize(reader, context);

            // source node
            IId startNodeId = context.EntityFactory.CreateId();
            if (reader.Name != Edge.StartNodeTag)
            {
                reader.ReadToFollowing(Edge.StartNodeTag);
            }
            startNodeId.Deserialize(reader, context);
            logger.Trace("Deserialized edge start node: {0}", startNodeId);

            // target node
            IId endNodeId = context.EntityFactory.CreateId();
            if (reader.Name != Edge.EndNodeTag)
            {
                reader.ReadToFollowing(Edge.EndNodeTag);
            }
            endNodeId.Deserialize(reader, context);
            logger.Trace("Deserialized edge end node: {0}", endNodeId);

            INode start = context.GetNode(startNodeId);
            INode end = context.GetNode(endNodeId);

            this.ConnectNodes(start, end);
            logger.Trace("Finishing Core.Edge deserialization.");
        }

        #endregion

        /// <summary>
        /// Sets the <c>source</c> and <c>target</c> nodes of this edge and sets up connections
        /// which means that <c>this</c> edge will be added to the <c>source</c> node's
        /// outbound edges collection and the the <c>target</c> node's inbound edges collections.
        /// </summary>
        /// <param name="source">source node of the relationship</param>
        /// <param name="target">target node of the relationship</param>
        /// <exception cref="EdgeAlreadyConnectedException">When an edge has been already added to a graph.</exception>
        public void ConnectNodes(INode start, INode end)
        {
            Objects.RequireNonNull(start);
            Objects.RequireNonNull(end);

            if (this.source != null || this.target != null)
            {
                logger.Error("Connecting already connected edge to node (start: {0}, end: {1})", start, end);
                throw new EdgeAlreadyConnectedException("Remove edge from graph before re-adding.");
            }

            this.source = start;
            this.source.OutboundEdges.Add(this);

            this.target = end;
            this.target.InboundEdges.Add(this);
            logger.Debug("Connected nodes (start: {0}, end: {1}) with edge {2}", start, end, this);
        }

        /// <summary>
        /// Cuts the connection between the <c>source</c> and <c>target</c> nodes.
        ///
        /// Removes the <c>this</c> edge from the endpoint nodes, and sets
        /// the appropriate variables <c>null</c>.
        /// </summary>
        public override bool Remove()
        {
            if (this.source == null || this.target == null)
            {
                logger.Warn("Edge has not been added to the graph before removing.");
                return false;
            }

            this.source.OutboundEdges.Remove(this);
            this.target.InboundEdges.Remove(this);

            logger.Debug("Removed edge ({0}) between start: {1}, end: {2} nodes.", this, this.source, this.target);

            this.source = null;
            this.target = null;

            return true;
        }

        public double GetWeight()
        {
            // basic implementation for the moment
            return 1.0;
        }
    }
}
