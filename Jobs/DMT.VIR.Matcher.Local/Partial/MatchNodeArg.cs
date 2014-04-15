using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.VIR.Matcher.Local.Patterns;

namespace DMT.VIR.Matcher.Local.Partial
{
    class MatchNodeArg<T> where T : class, INode
    {
        public INode NodeToMatch { get; private set; }
        public Predicate<T> Predicate { get; private set; }
        public PatternNode PatternNode { get; private set; }
        public bool IsRemote { get; private set; }
        public IId RemotePartitionId { get; private set; }

        public MatchNodeArg(INode node, PatternNode patternNode, Predicate<T> predicate, bool isRemote, IId partitionId)
            : this(node, patternNode, predicate)
        {
            this.IsRemote = isRemote;
            this.RemotePartitionId = partitionId;
        }

        public MatchNodeArg(INode node, PatternNode patternNode, Predicate<T> predicate, IMatchEdge incomingEdge)
            : this(node, patternNode, predicate)
        {
            if (incomingEdge != null)
            {
                this.IsRemote = incomingEdge.IsRemote;
                this.RemotePartitionId = incomingEdge.RemotePartitionId;
            }
            else
            {
                this.IsRemote = false;
                this.RemotePartitionId = null;
            }
        }

        private MatchNodeArg(INode node, PatternNode patternNode, Predicate<T> predicate)
        {
            this.NodeToMatch = node;
            this.PatternNode = patternNode;
            this.Predicate = predicate;
        }


        public void MarkMatch()
        {
            this.PatternNode.MatchedNode = this.NodeToMatch;
        }
    }
}
