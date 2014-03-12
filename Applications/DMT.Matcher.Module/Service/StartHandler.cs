using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using DMT.Common.Rest;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module.Service
{
    class StartHandler : XmlRouteHandlerBase<StartMatcherJobRequest, IXmlRouteResponse>
    {
        protected override bool HasResponseBody
        {
            get { return false; }
        }

        protected override IXmlRouteResponse Handle(StartMatcherJobRequest request, NameValueCollection urlParams)
        {
            MatcherModule.Instance.StartJob(request.Mode);
            
            // no return value need
            return null;
        }
    }
}
