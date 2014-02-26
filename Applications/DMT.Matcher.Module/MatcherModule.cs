using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Module
{
    public class MatcherModule
    {
        public void Start(string[] argv)
        {
            new Partition.PartitionBrokerServiceClient().RegisterMatcher(new Partition.MatcherInfo() { Id = Guid.NewGuid() });
        }
    }
}
