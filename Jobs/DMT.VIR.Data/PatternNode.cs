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
    public class PatternNode : Node, IPatternNode
    {
        private INode matchedNode;

        public INode MatchedNode
        {
            get { return this.matchedNode; }
            set { this.matchedNode = value; }
        }

        public bool IsMatched
        {
            get { return this.matchedNode != null; }
        }

        public PatternNode(IEntityFactory factory)
            : base(factory)
        {

        }

        // TODO: serialization
    }
}
