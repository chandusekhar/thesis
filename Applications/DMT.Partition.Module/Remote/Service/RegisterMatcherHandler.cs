using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;
using DMT.Module.Common.Service;

namespace DMT.Partition.Module.Remote.Service
{
    class RegisterMatcherHandler : XmlRouteHandlerBase<MatcherInfo, BoolResponse>
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override BoolResponse Handle(MatcherInfo request, NameValueCollection urlParams)
        {
            PartitionModule.Instance.MatcherRegistry.AddMatcher(request);
            logger.Info("Registered matcher with id {0}.", request.Id);
            return new BoolResponse(true);
        }
    }
}
