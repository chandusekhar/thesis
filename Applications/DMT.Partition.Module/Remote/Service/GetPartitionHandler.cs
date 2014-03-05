using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using DMT.Common.Extensions;
using DMT.Common.Rest;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Module.Remote.Service
{
    class GetPartitionHandler : IRouteHandler
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IPartitionSerializer partitionSerializer;

        public void Handle(Request request, Response response)
        {
            var pm = PartitionModule.Instance;
            PartitionSelector selector = new PartitionSelector(pm.MatcherRegistry, pm.PartitionRegistry);
            var matcherId = request.Params.Get("id").ParseGuid();

            response.Chunked = true;

            IPartition partition = selector.Select(matcherId);
            logger.Info("Sending {0} partition to {1} matcher.", partition, matcherId);
            using (var input = new FileStream(pm.ModelFileName, FileMode.Open))
            {
                this.partitionSerializer.Serialize(partition, input, response.Body);
            }
        }
    }
}
