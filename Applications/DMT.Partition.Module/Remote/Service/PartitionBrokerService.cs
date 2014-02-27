using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionBrokerService : IPartitionBrokerService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IPartitionSerializer partitionSerializer;

        public PartitionBrokerService()
        {
            CompositionService.Instance.InjectOnce(this);
        }

        public bool RegisterMatcher(MatcherInfo matcherInfo)
        {
            logger.Info("Registering matcher with id {0}", matcherInfo.Id);
            PartitionModule.Instance.MatcherRegistry.AddMatcher(matcherInfo);
            // at this point we accept every matcher
            return true;
        }

        public Stream GetPartition(Guid matcherId)
        {
            var pm = PartitionModule.Instance;
            PartitionSelector selector = new PartitionSelector(pm.MatcherRegistry, pm.PartitionRegistry);

            try
            {
                IPartition partition = selector.Select(matcherId);
                logger.Info("Sending {0} partition to {1} matcher.", partition, matcherId);
                using (var input = new FileStream(pm.ModelFileName, FileMode.Open))
                {
                    return this.partitionSerializer.Serialize(partition, input);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error during partition selection/serialization.", ex);
                // rethrow exception
                throw new FaultException<Exception>(ex);
            }
        }


        public void MatcherReady(Guid matcherId)
        {
            logger.Info("Matcher ready. Id: {0}", matcherId);
            PartitionModule.Instance.MatcherRegistry.MarkReady(matcherId);
        }
    }
}
