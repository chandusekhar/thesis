using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Data
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

        public IEnumerable<Core.Interfaces.INode> GetMatchedNodes()
        {
            throw new NotImplementedException();
        }
    }
}
