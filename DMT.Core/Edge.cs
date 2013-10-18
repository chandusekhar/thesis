using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;

namespace DMT.Core
{
    public class Edge : Entity, IEdge
    {
        public const string StartNodeTag = "StartNode";
        public const string EndNodeTag = "EndNode";

        private INode _start;
        private INode _end;

        public Edge()
            : this(null, null)
        { }

        public Edge(INode start, INode end)
        {
            this._start = start;
            this._end = end;
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
            // id
            base.Deserialize(reader, context);

            // start node
            IId startNodeId = context.EntityFactory.CreateId();
            startNodeId.Deserialize(reader, context);

            // end node
            IId endNodeId = context.EntityFactory.CreateId();
            endNodeId.Deserialize(reader, context);

            INode start = context.GetNode(startNodeId);
            INode end = context.GetNode(endNodeId);

            this.ConnectNodes(start, end);

        }

        public void ConnectNodes(INode start, INode end)
        {
            _start = start;
            _start.OutboundEdges.Add(this);

            _end = end;
            _end.InboundEdges.Add(this);
        }
    }
}
