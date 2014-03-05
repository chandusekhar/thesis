using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;

namespace DMT.Partition.Module.Remote.Service
{
    class PartitionBrokerService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const int Port = 8080;

        private RestServiceHost service;

        public Uri BaseAddress
        {
            get
            {
                UriBuilder ub = new UriBuilder();
                ub.Host = "localhost";
                ub.Scheme = "http";
                ub.Port = PartitionBrokerService.Port;

                return ub.Uri;
            }
        }

        public PartitionBrokerService()
        {
            this.service = new RestServiceHost(8080);

            // register routes and stuff
            Initialize();
        }

        public void Start()
        {
            logger.Info("Starting REST service");
            this.service.StartAsync();
        }

        private void Initialize()
        {
            this.service.Router.Register(HttpMethod.Post, "/register", new RegisterMatcherHandler());
            this.service.Router.Register(HttpMethod.Delete, "/matchers/{id}", new DeleteMatcherHandler());
            this.service.Router.Register(HttpMethod.Put, "/matchers/{id}/ready", new MatcherReadyHandler());
        }
    }
}
