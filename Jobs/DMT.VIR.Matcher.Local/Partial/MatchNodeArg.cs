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
        public INode NodeToMatch { get; set; }
        public IMatchEdge IncomingEdge { get; set; }
        public Predicate<T> Predicate { get; set; }
        public Pattern Pattern { get; set; }
        public PatternNode PatternNode { get; set; }

        public bool IsRemote
        {
            get { return this.IncomingEdge.IsRemote; }
        }

        public MatchNodeArg()
        {
        }

        public MatchNodeArg(INode node, PatternNode patternNode, Predicate<T> predicate, IMatchEdge incomingEdge, Pattern pattern)
        {
            this.NodeToMatch = node;
            this.Pattern = pattern;
            this.IncomingEdge = incomingEdge;
            this.PatternNode = patternNode;
            this.Predicate = predicate;
        }

        public void MarkMatch()
        {
            this.PatternNode.MatchedNode = this.NodeToMatch;
        }
    }
}
