using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using DMT.Common.Rest;
using DMT.Module.Common.Service;

namespace DMT.Partition.Module.Remote.Service
{
    class DeleteMatcherHandler : XmlResponseOnlyRouteHandlerBase<BoolResponse>
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override BoolResponse Handle(NameValueCollection urlParams)
        {
            string idString = urlParams.Get("id");
            Guid id = Guid.Parse(idString);

            var result = PartitionModule.Instance.MatcherRegistry.RemoveMatcher(id);
            logger.Info("Deleted matcher (id: {0}) with {1} result.", idString, result);
            return new BoolResponse(result);
        }
    }
}
