using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using DMT.Common.Rest;
using DMT.Module.Common.Service;
using DMT.Common.Extensions;
using System.ComponentModel.Composition;
using DMT.Core.Interfaces;
using DMT.Common.Composition;

namespace DMT.Partition.Module.Remote.Service
{
    /// <summary>
    /// Find a matcher by the id of the partition that it holds.
    /// </summary>
    class MatcherFinderHandler : XmlResponseOnlyRouteHandlerBase<MatcherInfoResponse>
    {
        [Import]
        private IEntityFactory factory;

        public MatcherFinderHandler()
        {
            CompositionService.Default.InjectOnce(this);
        }


        protected override MatcherInfoResponse Handle(NameValueCollection urlParams)
        {
            string idStr = urlParams.Get("partitionId");

            MatcherInfoResponse response = new MatcherInfoResponse();

            if (idStr != null)
            {
                IId id = factory.ParseId(idStr);
                response.Result = PartitionModule.Instance.MatcherRegistry.GetByPartitionId(id);
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "Partition id is not present.";
            }

            return response;
        }
    }
}
