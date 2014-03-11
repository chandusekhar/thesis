using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class DefaultRouter : IRouter
    {
        private PatternCollection routes;

        public DefaultRouter()
        {
            this.routes = new PatternCollection();
        }

        public IRouteMatchResult Get(HttpMethod method, string url)
        {
            NameValueCollection urlParams = new NameValueCollection();
            var leafSegment = this.routes.GetRoute(url, urlParams);
            var handler = leafSegment != null ? leafSegment.Handlers.Get(method) : null;

            return new RouteResult
            {
                RouteFound = leafSegment != null,
                HandlerFound = handler != null,
                Handler = handler,
                RouteParams = urlParams,
            };
        }

        public void Register(HttpMethod method, string urlPattern, IRouteHandler handler)
        {
            var leafSegment = this.routes.Add(urlPattern);
            leafSegment.Handlers.AddOrUpdate(method, handler);
        }
    }
}
