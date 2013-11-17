using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        public Node()
            : base()
        {
            _outboundEdges = new List<IEdge>();
            _inboundEdges = new List<IEdge>();
        }

        [ImportingConstructor]
        public Node(IEntityFactory factory)
            : this()
        {
            this.factory = factory;
        }

        public override bool Remove()
        {
            throw new NotImplementedException();
        }

        public void ConnectTo(INode node)
        {
            Objects.RequireNonNull(node);

            var edge = factory.CreateEdge();
            edge.ConnectNodes(this, node);
        }
    }
}
