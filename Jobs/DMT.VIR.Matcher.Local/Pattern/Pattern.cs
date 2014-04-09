using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Matcher.Local.Pattern
{
    public class Pattern : IPattern
    {
        private List<IPatternNode> patternNodes;

        public IEnumerable<IPatternNode> PatternNodes
        {
            get { return this.patternNodes; }
            set { this.patternNodes = new List<IPatternNode>(value); }
        }

        public bool IsFullyMatched
        {
            get { return this.patternNodes.All(pn => pn.IsMatched); }
        }

        public Pattern()
        {
            this.patternNodes = new List<IPatternNode>();
        }

        public IEnumerable<INode> GetMatchedNodes()
        {
            return this.patternNodes.Where(pn => pn.IsMatched);
        }

        public void AddNodes(params IPatternNode[] nodes)
        {
            this.patternNodes.AddRange(nodes);
        }

        public void Reset()
        {
            patternNodes.ForEach(pn => pn.MatchedNode = null);
        }

        public IPatternNode GetNodeByName(string name)
        {
            return this.patternNodes.Single(pn => pn.Name == name);
        }

        public bool HasMatchedNodeFor(string name)
        {
            var node = GetNodeByName(name);
            return node.IsMatched;
        }
    }
}
