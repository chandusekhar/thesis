using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest.Router;

namespace DMT.Common.Rest
{
    /// <summary>
    /// Super simple RESTful service host. Delegates HTTP requests to registered routes.
    /// The default router uses static path matching
    /// </summary>
    public class RestServiceHost : IDisposable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const string NotFoundBody = "Requested resource was not found.";

        private readonly int port;
        private HttpListener listener;
        private IRouter router;
        private bool started;

        /// <summary>
        /// Gets or sets the url router.
        /// </summary>
        public IRouter Router
        {
            get { return this.router; }
            set { this.router = value; }
        }

        public RestServiceHost(int port)
        {
            this.port = port;
            this.router = new DefaultRouter();
        }

        /// <summary>
        /// Start the REST service in a blocking way.
        /// </summary>
        public void Start()
        {
            if (this.started)
                return;

            this.started = true;
            this.listener = new HttpListener();
            this.listener.Prefixes.Add(GetPrefix());
            this.listener.Start();

            while (this.listener.IsListening)
            {
                try
                {
                    var context = this.listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException ex) {
                    logger.WarnException("http service ended", ex);
                }
            }
        }

        /// <summary>
        /// Start the REST service in a non-blocking way.
        /// </summary>
        public void StartAsync()
        {
            Task.Run(new Action(Start));
        }

        private void HandleRequest(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            HttpMethod method = req.HttpMethod;
            var result = this.router.Get(method, req.RawUrl);

            if (result.Success)
            {
                try
                {
                    Request ireq = new Request(req, result.RouteParams);
                    Response ires = new Response(res);
                    result.Handler.Handle(ireq, ires);
                    ires.EnsureClosed();
                }
                catch (Exception ex)
                {
                    logger.ErrorException(string.Format("Unhandled exception on [{1}] {0} route", req.RawUrl, method), ex);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Exception occured!");
                    sb.AppendFormat("Message: {0}\n", ex.Message);
                    sb.Append(ex.StackTrace);
                    Send(res, HttpStatusCode.InternalServerError, sb.ToString());
                }
            }
            else
            {
                logger.Warn("Route handler not found for {0} route.", req.RawUrl);
                Send(res, HttpStatusCode.NotFound, NotFoundBody);
            }
        }

        private void Send(HttpListenerResponse res, HttpStatusCode status, string body)
        {
            res.StatusCode = (int)status;
            using (var writer = new StreamWriter(res.OutputStream))
            {
                if (body != null)
                {
                    writer.Write(body);
                }
            }
        }

        private string GetPrefix()
        {
            return string.Format("http://+:{0}/", this.port);
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            if (listener != null)
            {
                ((IDisposable)listener).Dispose();
            }
        }

        #endregion
    }
}
