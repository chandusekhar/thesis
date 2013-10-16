using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    public class Edge : Entity, IEdge
    {
        private INode _start, _end;

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
    }
}
