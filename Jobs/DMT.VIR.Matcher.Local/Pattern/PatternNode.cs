using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Matcher.Local.Pattern
{
    public class PatternNode : Node, IPatternNode
    {
        private INode matchedNode;
        private string name;

        public INode MatchedNode
        {
            get { return this.matchedNode; }
            set { this.matchedNode = value; }
        }

        public bool IsMatched
        {
            get { return this.matchedNode != null; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public PatternNode(string name, IEntityFactory factory)
            : base(factory)
        {
            this.name = name;
        }

        // TODO: serialization
    }
}
