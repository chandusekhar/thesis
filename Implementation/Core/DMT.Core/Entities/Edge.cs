using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Exceptions;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Entities
{
    public class Edge : Entity, IEdge
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public const string EndATag = "EndA";
        public const string EndBTag = "EndB";
        public const string DirectionTag = "Direction";

        private INode endA = null;
        private INode endB = null;
        private EdgeDirection direction = EdgeDirection.Both;

        public double Weight { get; set; }

        public INode EndA
        {
            get { return endA; }
        }

        public INode EndB
        {
            get { return endB; }
        }

        public EdgeDirection Direction
        {
            get { return direction; }
        }

        public Edge(IEntityFactory factory)
            : base(factory)
        {

        }

        public Edge(INode nodeA, INode nodeB, EdgeDirection direction, IEntityFactory factory)
            : base(factory)
        {
            this.endA = nodeA;
            this.endB = nodeB;
            this.direction = direction;
        }

        #region ISerializable

        public override void Serialize(XmlWriter writer)
        {
            logger.Trace("Started serialization of edge {0}", Id);

            // id
            base.Serialize(writer);

            // endpoint a
            writer.WriteStartElement(Edge.EndATag);
            endA.Id.Serialize(writer);
            writer.WriteEndElement();

            // endpoint b
            writer.WriteStartElement(Edge.EndBTag);
            endB.Id.Serialize(writer);
            writer.WriteEndElement();

            // direction
            writer.WriteElementString(Edge.DirectionTag, this.direction.ToString());

            logger.Trace("Finished serialization of edge {0}", Id);
        }

        public override void Deserialize(XmlReader reader, IContext context)
        {
            logger.Trace("Starting edge deserialization.");

            // id
            base.Deserialize(reader, context);

            // the order of these is important, DO NOT change them
            IId endAId = DeserializeEndPoint(Edge.EndATag, reader, context);
            IId endBId = DeserializeEndPoint(Edge.EndBTag, reader, context);
            this.direction = DeserializeDirection(reader, context);

            this.endA = context.GetNode(endAId);
            this.endB = context.GetNode(endBId);

            // register edge at the node
            endA.ConnectTo(endB, this);

            logger.Trace("Finishing edge deserialization.");
        }

        private IId DeserializeEndPoint(string tag, XmlReader reader, IContext context)
        {
            // endpoint B node
            IId nodeId = context.EntityFactory.CreateId();
            if (reader.Name != tag)
            {
                reader.ReadToFollowing(tag);
            }
            nodeId.Deserialize(reader, context);
            logger.Trace("Deserialized edge end node: {0}", nodeId);
            return nodeId;
        }

        private EdgeDirection DeserializeDirection(XmlReader reader, IContext context)
        {
            EdgeDirection direction;
            // direction
            if (reader.Name != Edge.DirectionTag)
            {
                reader.ReadToFollowing(Edge.DirectionTag);
            }
            if (!Enum.TryParse<EdgeDirection>(reader.ReadElementContentAsString(), out direction))
            {
                logger.Warn("Could not parse direction of edge (id: {0}), falling back to bidirectional.", this.Id);
                direction = EdgeDirection.Both;
            }

            return direction;
        }

        #endregion

        /// <summary>
        /// Cuts the connection between the <c>source</c> and <c>target</c> nodes.
        ///
        /// Removes the <c>this</c> edge from the endpoint nodes, and sets
        /// the appropriate variables <c>null</c>.
        /// </summary>
        public override bool Remove()
        {
            if (this.endA == null || this.endB == null)
            {
                logger.Warn("Edge has not been added to the graph before removing.");
                return false;
            }

            this.endA.Disconnect(this);

            this.endA = null;
            this.endB = null;

            logger.Trace("Removed edge ({0}) between start: {1}, end: {2} nodes.", this, this.endA, this.endB);
            return true;
        }

        public INode GetOtherNode(INode node)
        {
            // NOTE: a reference check might suffice, but using proper Equals here for the moment
            if (this.endA.Equals(node))
            {
                return this.endB;
            }

            if (this.endB.Equals(node))
            {
                return this.endA;
            }

            throw new InvalidNodeException("{0} node is not connected by edge {1}", node, this);
        }
    }
}
