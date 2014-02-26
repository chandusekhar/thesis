using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionBrokerService : IPartitionBrokerService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool RegisterMatcher(MatcherInfo matcherInfo)
        {
            logger.Info("Registering matcher with id {0}", matcherInfo.Id);
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
