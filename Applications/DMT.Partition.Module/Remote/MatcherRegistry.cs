using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Module.Exceptions;
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

        public MatcherInfo GetById(Guid id)
        {
            var matcher = this.matchers.FirstOrDefault(m => m.Id == id);
            if (matcher == null)
            {
                throw new MatcherNotFoundException(string.Format("Matcher with {0} id was not found.", id));
            }

            return matcher;
        }
    }
}
