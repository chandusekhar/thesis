using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Matcher.Local.Patterns
{
    public class PatternNode : Node
    {
        private string name;

        public INode MatchedNode { get; set; }

        public bool IsMatched
        {
            get { return this.MatchedNode != null; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public PatternNode(IEntityFactory factory)
            : base(factory)
        {

        }

        public PatternNode(string name, IEntityFactory factory)
            : base(factory)
        {
            this.name = name;
        }

        // TODO: serialization
    }
}
