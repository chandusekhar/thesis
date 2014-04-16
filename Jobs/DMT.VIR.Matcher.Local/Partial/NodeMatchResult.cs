using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.VIR.Matcher.Local.Partial
{
    class NodeMatchResult
    {
        public bool IsMatched { get; private set; }
        public bool IsFullSubpatternMatched { get; private set; }

        public NodeMatchResult(bool matched)
            : this(matched, false)
        {

        }

        public NodeMatchResult(bool matched, bool subpatternfullymatched)
        {
            this.IsMatched = matched;
            this.IsFullSubpatternMatched = subpatternfullymatched;
        }
    }
}
