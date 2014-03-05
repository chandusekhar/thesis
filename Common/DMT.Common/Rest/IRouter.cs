using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMT.Common.Rest
{
    public interface IRouter
    {
        /// <summary>
        /// Gets the route handler registered for the specified route
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="url">the url</param>
        /// <returns>the route handler if found, null otherwise</returns>
        IRouteMatchResult Get(HttpMethod method, string url);

        /// <summary>
        /// Register a handler for a method-route pair.
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="urlPattern">the route. it must be an absolute path and can contain parameters</param>
        /// <param name="handler">handler object</param>
        void Register(HttpMethod method, string urlPattern, IRouteHandler handler);
    }
}
