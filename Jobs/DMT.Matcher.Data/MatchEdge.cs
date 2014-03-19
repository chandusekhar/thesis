using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Data
{
    public class MatchEdge : Edge, IMatchEdge
    {
        private IId remotePartitionId;

        public bool IsRemote
        {
            get { return this.remotePartitionId != null; }
        }

        public IId RemotePartitionId
        {
            get { return this.remotePartitionId; }
            set { this.remotePartitionId = value; }
        }

        public MatchEdge(INode nodeA, INode nodeB, EdgeDirection direction, IEntityFactory factory)
            : base(nodeA, nodeB, direction, factory)
        {

        }
    }
}
