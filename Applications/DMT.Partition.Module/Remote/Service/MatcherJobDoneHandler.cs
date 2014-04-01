using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Common.Rest;
using DMT.Common.Extensions;
using DMT.Module.Common.Service;
using System.Collections.Specialized;

namespace DMT.Partition.Module.Remote.Service
{
    class MatcherJobDoneHandler : XmlRouteHandlerBase<MatchFoundRequest, IXmlRouteResponse>
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override bool HasResponseBody
        {
            get
            {
                return false;
            }
        }

        protected override IXmlRouteResponse Handle(MatchFoundRequest request, NameValueCollection urlParams)
        {
            Guid id = urlParams.Get("id").ParseGuid();

            if (request.MatchFound)
            {
                logger.Info("Match found in {0} matcher module", id);
                Console.WriteLine("Match found in {0} module", id);
            }

            logger.Info("Matcher ({0}) is done.", id);
            PartitionModule.Instance.MatcherRegistry.MarkDone(id);
            return null;
        }
    }
}
