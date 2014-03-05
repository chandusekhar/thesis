using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Common.Rest;

namespace DMT.Partition.Module.Remote.Service
{
    class MatcherReadyHandler : IRouteHandler
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Handle(Request request, Response response)
        {
            Guid id = ParseId(request.Params.Get("id"));
            if (id == Guid.Empty) {
                logger.Warn("Empty id for matcher.");
                return;
            }

            logger.Info("Matcher (id: {0}) is ready.", id);
            PartitionModule.Instance.MatcherRegistry.MarkReady(id);
        }

        private Guid ParseId(string idString)
        {
            if (idString == null)
            {
                return Guid.Empty;
            }

            return Guid.Parse(idString);
        }
    }
}
