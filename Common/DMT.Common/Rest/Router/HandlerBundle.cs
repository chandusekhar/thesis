using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class HandlerBundle
    {
        private Dictionary<HttpMethod, IRouteHandler> handlers;

        public HandlerBundle()
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
