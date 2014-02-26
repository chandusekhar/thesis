using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Module.Remote.Service;

namespace DMT.Partition.Module.Remote
{
    class MatcherRegistry
    {
        private List<MatcherInfo> matchers;

        public MatcherRegistry()
        {
            this.matchers = new List<MatcherInfo>();
        }

        public void AddMatcher(MatcherInfo matcherinfo)
        {
            this.matchers.Add(matcherinfo);
        }
    }
}
