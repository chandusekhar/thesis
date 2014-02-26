using System;
using System.Collections.Generic;
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
        /// Gets the partition with the given id.
        /// </summary>
        /// <param name="id">the id of the partition</param>
        /// <returns>the partition response containing all the information that the matcher needs</returns>
        [OperationContract]
        PartitionResponse GetPartition();
    }
}
