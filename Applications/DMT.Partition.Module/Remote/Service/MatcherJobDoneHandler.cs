using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Common.Rest;
using DMT.Common.Extensions;

namespace DMT.Partition.Module.Remote.Service
{
    class MatcherJobDoneHandler : IRouteHandler
    {
        public void Handle(Request request, Response response)
        {
            Guid id = request.Params.Get("id").ParseGuid();
            PartitionModule.Instance.MatcherRegistry.MarkDone(id);
        }
    }
}
