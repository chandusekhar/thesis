using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;

namespace DMT.Module.Common.Service
{
    public abstract class ServiceBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public const string Localhost = "localhost";

        private string host;
        private int port;
        private RestServiceHost service;

        protected RestServiceHost Service
        {
            get { return this.service; }
        }

        public Uri BaseAddress
        {
            get
            {
                return new UriBuilder("http", this.host, this.port).Uri;
            }
        }

        public ServiceBase(int port) : this(ServiceBase.Localhost, port) { }

        public ServiceBase(string host, int port)
        {
            this.host = host;
            this.port = port;

            this.service = new RestServiceHost(port);

            Initialize();
        }

        public void Start()
        {
            this.service.StartAsync();
            logger.Info("Started REST service. Listening on {0}:{1}", this.host, this.port);
        }

        public void Close()
        {
            this.service.Close();
        }

        protected void RegisterRoute(HttpMethod method, string urlPatter, IRouteHandler handler)
        {
            this.service.Router.Register(method, urlPatter, handler);
        }

        protected abstract void Initialize();
    }
}
