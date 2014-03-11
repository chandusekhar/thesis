using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Common.Rest;

namespace DMT.Matcher.Module.Service
{
    class QuitHandler : IRouteHandler
    {
        public void Handle(Request request, Response response)
        {
            MatcherModule.Instance.Done();
        }
    }
}
