using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    public class Node : Entity, INode
    {
        protected List<IEdge> _outboundEdges;
        protected List<IEdge> _inboundEdges;

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

        public override bool Remove()
        {
            throw new NotImplementedException();
        }
    }
}
