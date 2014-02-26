using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionBrokerService : IPartitionBrokerService
    {
        public bool RegisterMatcher(MatcherInfo matcherInfo)
        {
            PartitionModule.Instance.MatcherRegistry.AddMatcher(matcherInfo);
            // at this point we accept every matcher
            return true;
        }

        public PartitionResponse GetPartition(Core.Interfaces.IId id)
        {
            throw new NotImplementedException();
        }
    }
}
