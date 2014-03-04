using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    internal class DefaultRouter : IRouter
    {
        private Dictionary<string, RouteBundle> routes;

        public DefaultRouter()
        {
            this.routes = new Dictionary<string, RouteBundle>();
        }

        public IRouteHandler Get(HttpMethod method, string url)
        {
            RouteBundle bundle;
            if (!this.routes.TryGetValue(new UrlSanitizer().Sanitize(url), out bundle))
            {
                return null;
            }

            return bundle.Get(method);
        }

        public void Register(HttpMethod method, string urlPattern, IRouteHandler handler)
        {
            var url = new UrlSanitizer().Sanitize(urlPattern);
            RouteBundle bundle;
            if (this.routes.TryGetValue(url, out bundle))
            {
                bundle.AddOrUpdate(method, handler);
            }
            else
            {
                bundle = new RouteBundle();
                bundle.AddOrUpdate(method, handler);
                this.routes.Add(url, bundle);
            }
        }

        private class RouteBundle
        {
            private Dictionary<HttpMethod, IRouteHandler> handlers;

            public RouteBundle()
            {
                handlers = new Dictionary<HttpMethod, IRouteHandler>();
            }

            public void AddOrUpdate(HttpMethod method, IRouteHandler handler)
            {
                this.handlers[method] = handler;
            }

            public IRouteHandler Get(HttpMethod method)
            {
                if (!this.handlers.ContainsKey(method))
                {
                    return null;
                }

                return this.handlers[method];
            }
        }
    }
}
