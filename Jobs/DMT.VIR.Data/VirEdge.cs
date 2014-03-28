using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Data
{
    public class VirEdge : Edge, IMatchEdge
    {
        private IId partitionId;

        public bool IsRemote
        {
            get { return this.partitionId != null; }
        }

        public IId RemotePartitionId
        {
            get { return this.partitionId; }
            set { this.partitionId = value; }
        }

        public VirEdge(INode nodeA, INode nodeB, EdgeDirection direction, IEntityFactory factory)
            : base(nodeA, nodeB, direction, factory)
        {

        }
    }
}
