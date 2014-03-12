using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;
using DMT.Module.Common.Service;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionBrokerService : ServiceBase
    {
        private const int Port = 8080;

        public PartitionBrokerService()
            : base(PartitionBrokerService.Port)
        {
        }

        protected override void Initialize()
        {
            RegisterRoute(HttpMethod.Post, "/matchers", new RegisterMatcherHandler());
            RegisterRoute(HttpMethod.Put, "/matchers/{id}/ready", new MatcherReadyHandler());
            RegisterRoute(HttpMethod.Get, "/matchers/{id}/partition", new GetPartitionHandler());
            RegisterRoute(HttpMethod.Get, "/matchers/find/{partitionId}", new MatcherFinderHandler());
            RegisterRoute(HttpMethod.Get, "/matchers/job", new GetMatcherJobHandler());
        }
    }
}
