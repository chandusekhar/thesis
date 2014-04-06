using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.Matcher.Module.Service;
using DMT.Module.Common.Service;

namespace DMT.Matcher.Module
{
    /// <summary>
    /// MatcherFrameworkLink is the implementation of the link between the matcher job
    /// and the matcher framework. This is a one way interaction: matcher job -> framework
    /// </summary>
    class MatcherFrameworkLink : IMatcherFramework
    {
        private Dictionary<IId, MatcherInfo> partitionRouting;

        public MatcherFrameworkLink()
        {
            this.partitionRouting = new Dictionary<IId, MatcherInfo>();
        }

        public void BeginFindPartialMatch(IId partitionId, IPattern pattern)
        {
            throw new NotImplementedException();
        }

        public event FoundPartialMatchEventHandler FoundPartialMatch;


        public INode GetNode(IId partitionId, IId nodeId)
        {
            if (!this.partitionRouting.ContainsKey(partitionId))
            {
                MatcherInfo mi = MatcherModule.Instance.CreatePartitionServiceClient().FindMatcher(partitionId);
                this.partitionRouting.Add(partitionId, mi);
            }

            MatcherInfo matcher = this.partitionRouting[partitionId];

            MatcherServiceClient client = new MatcherServiceClient(matcher.Url);
            return client.GetNode(nodeId);
        }
    }
}
