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

        public string BaseAddress
        {
            get
            {
                return string.Format("http://localhost:8080/");
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
        }
    }
}
