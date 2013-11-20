using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Core.Interfaces;

namespace DMT.Core.Entities
{
    public class Node : Entity, INode
    {
        protected List<IEdge> _outboundEdges;
        protected List<IEdge> _inboundEdges;

        protected IEntityFactory factory;

        public ICollection<IEdge> OutboundEdges
        {
            get { return _outboundEdges; }
        }

        public ICollection<IEdge> InboundEdges
        {
            get { return _inboundEdges; }
        }

        public int Degree
        {
            get { return this._outboundEdges.Count + this._inboundEdges.Count; }
        }

        public Node()
            : base()
        {
            _outboundEdges = new List<IEdge>();
            _inboundEdges = new List<IEdge>();
        }

        public Node(IEntityFactory factory)
            : this()
        {
            this.factory = factory;
        }

        public override bool Remove()
        {
            throw new NotImplementedException();
        }

        public IEdge ConnectTo(INode node, EdgeDirection direction)
        {
            Objects.RequireNonNull(node);

            var edge = factory.CreateEdge();

            switch (direction)
            {
                case EdgeDirection.Inbound:
                    edge.ConnectNodes(node, this);
                    break;
                case EdgeDirection.Outbound:
                    edge.ConnectNodes(this, node);
                    break;
                default:
                    throw new ArgumentException("direction", "Not recognized direction.");
            }

            return edge;
        }


        public IEnumerable<IEdge> GetAllEdges()
        {
            return this._inboundEdges.Union(this._outboundEdges);
        }

        public IEnumerable<INode> GetAdjacentNodes()
        {
            List<INode> neighbours = new List<INode>(this._outboundEdges.Count + this._inboundEdges.Count);

            foreach (var edge in this._inboundEdges)
            {
                neighbours.Add(edge.Source);
            }

            foreach (var edge in this._outboundEdges)
            {
                neighbours.Add(edge.Target);
            }

            return neighbours;
        }
    }
}
