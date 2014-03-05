using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest.Router
{
    class RouteResult : IRouteMatchResult
    {
        public bool Success { get; set; }
        public IRouteHandler Handler { get; set; }
        public NameValueCollection RouteParams { get; set; }
    }
}
