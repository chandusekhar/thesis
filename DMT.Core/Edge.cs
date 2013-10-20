using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common;
using DMT.Core.Exceptions;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core
{
    public class Edge : Entity, IEdge
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const string StartNodeTag = "StartNode";
        public const string EndNodeTag = "EndNode";

        private INode start = null;
        private INode end = null;

        public Edge()
        { }

        /// <summary>
        /// Instantiates a new <c>Edge</c> object and connects its endpoints
        /// which means that start and end will be set, also <c>this</c> edge will be
        /// added to the <c>start</c> node's outbound edges collection and the
        /// the <c>end</c> node's inbound edges collections.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Edge(INode start, INode end)
        {
            this.ConnectNodes(start, end);
        }

        public INode Start
        {
            get { return start; }
        }

        public INode End
        {
            get { return end; }
        }

        public override void Serialize(XmlWriter writer)
        {
            // id
            base.Serialize(writer);

            // startnode
            writer.WriteStartElement(Edge.StartNodeTag);
            start.Id.Serialize(writer);
            writer.WriteEndElement();

            // endnode
            writer.WriteStartElement(Edge.EndNodeTag);
            end.Id.Serialize(writer);
            writer.WriteEndElement();
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            logger.Trace("Starting Core.Edge deserialization.");

            // id
            base.Deserialize(reader, context);

            // start node
            IId startNodeId = context.EntityFactory.CreateId();
            if (reader.Name != Edge.StartNodeTag)
            {
                reader.ReadToFollowing(Edge.StartNodeTag);
            }
            startNodeId.Deserialize(reader, context);
            logger.Trace("Deserialized edge start node: {0}", startNodeId);

            // end node
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

        /// <summary>
        /// Sets the <c>start</c> and <c>end</c> nodes of this edge and sets up connections
        /// which means that <c>this</c> edge will be added to the <c>start</c> node's
        /// outbound edges collection and the the <c>end</c> node's inbound edges collections.
        /// </summary>
        /// <param name="start">start node of the relationship</param>
        /// <param name="end">end node of the relationship</param>
        public void ConnectNodes(INode start, INode end)
        {
            Objects.RequireNonNull(start);
            Objects.RequireNonNull(end);

            if (this.start != null || this.end != null)
            {
                logger.Error("Connecting already connected edge to node (start: {0}, end: {1})", start, end);
                throw new EdgeAlreadyConnectedException("Remove edge from graph before re-adding.");
            }

            this.start = start;
            this.start.OutboundEdges.Add(this);

            this.end = end;
            this.end.InboundEdges.Add(this);
            logger.Debug("Connected nodes (start: {0}, end: {1}) with edge {2}", start, end, this);
        }

        public void Remove()
        {
            if (this.start == null || this.end == null)
            {
                logger.Error("Edge has not been added to the graph before removing.");
                throw new EdgeNotYetConnectedException("Add the edge to the graph first.");
            }

            this.start.OutboundEdges.Remove(this);
            this.end.InboundEdges.Remove(this);

            logger.Debug("Removed edge ({0}) between start: {1}, end: {2} nodes.", this, this.start, this.end);

            this.start = null;
            this.end = null;
        }
    }
}
