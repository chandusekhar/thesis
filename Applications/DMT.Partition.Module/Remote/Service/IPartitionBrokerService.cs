using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.Module.Remote.Service
{
    [ServiceContract]
    public interface IPartitionBrokerService
    {
        /// <summary>
        /// Registers a matcher.
        /// </summary>
        /// <param name="matcherInfo">Information about the matcher instance.</param>
        /// <returns>true if t</returns>
        [OperationContract]
        bool RegisterMatcher(MatcherInfo matcherInfo);

        /// <summary>
        /// Gets a partition.
        /// </summary>
        /// <param name="matcherId">The id of the matcher instance that asks for a partition</param>
        /// <returns>the partition in xml which has to be processed in the matcher</returns>
        [OperationContract]
        Stream GetPartition(Guid matcherId);
    }
}
