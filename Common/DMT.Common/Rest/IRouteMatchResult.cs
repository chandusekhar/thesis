using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Common.Rest
{
    public interface IRouteMatchResult
    {
        bool RouteFound { get; }
        bool HandlerFound { get; }
        bool Success { get; }
        IRouteHandler Handler { get; }
        NameValueCollection RouteParams { get; }
    }
}
