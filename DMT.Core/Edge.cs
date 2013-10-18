using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common;
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

        private INode _start = null;
        private INode _end = null;

        public Edge()
        { }

        /// <summary>
        /// Instantiates a new <c>Edge</c> object and sets its endpoints.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Edge(INode start, INode end)
        {
            this.ConnectNodes(start, end);
        }

        public INode Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public INode End
        {
            get { return _end; }
            set { _end = value; }
        }

        public override void Serialize(XmlWriter writer)
        {
            // id
            base.Serialize(writer);

            // startnode
            writer.WriteStartElement(Edge.StartNodeTag);
            _start.Id.Serialize(writer);
            writer.WriteEndElement();

            // endnode
            writer.WriteStartElement(Edge.EndNodeTag);
            _end.Id.Serialize(writer);
            writer.WriteEndElement();
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            logger.Trace("Starting Core.Edge deserialization.");

            // id
            base.Deserialize(reader, context);

            // start node
            IId startNodeId = context.EntityFactory.CreateId();
            startNodeId.Deserialize(reader, context);
            logger.Trace("Deserialized edge start node: {0}", startNodeId);

            // end node
            IId endNodeId = context.EntityFactory.CreateId();
            endNodeId.Deserialize(reader, context);
            logger.Trace("Deserialized edge end node: {0}", endNodeId);

            INode start = context.GetNode(startNodeId);
            INode end = context.GetNode(endNodeId);

            this.ConnectNodes(start, end);
            logger.Trace("Finishing Core.Edge deserialization.");
        }

        public void ConnectNodes(INode start, INode end)
        {
            Objects.RequireNonNull(start);
            Objects.RequireNonNull(end);

            _start = start;
            _start.OutboundEdges.Add(this);

            _end = end;
            _end.InboundEdges.Add(this);
            logger.Debug("Connected nodes ({0}, {1}) with edge {2}", start, end, this);
        }
    }
}
