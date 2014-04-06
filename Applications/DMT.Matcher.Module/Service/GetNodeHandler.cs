using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using DMT.Common.Composition;
using DMT.Common.Rest;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Module.Service
{
    class GetNodeHandler : IRouteHandler
    {
        [Import]
        private IEntityFactory factory;

        public GetNodeHandler()
        {
            CompositionService.Default.InjectOnce(this);
        }

        public void Handle(Request request, Response response)
        {
            IId nodeid = factory.ParseId(request.Params["id"]);
            INode node = MatcherModule.Instance.GetNode(nodeid);

            if (node != null)
            {
                new NodeSerializer(response.Body).Serialize(node);
            }
            else
            {
                response.Status = System.Net.HttpStatusCode.NotFound;
            }
        }
    }
}
