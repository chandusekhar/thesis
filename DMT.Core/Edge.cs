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
    }
}
