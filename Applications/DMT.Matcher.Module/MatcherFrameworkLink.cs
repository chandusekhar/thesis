using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<IId, MatcherInfo> partitionRouting;
        private ConcurrentDictionary<Guid, PartialMatchResult> results;

        public MatcherFrameworkLink()
        {
            this.partitionRouting = new ConcurrentDictionary<IId, MatcherInfo>();
            this.results = new ConcurrentDictionary<Guid, PartialMatchResult>();
        }

        public IPartialMatchResult BeginFindPartialMatch(IId partitionId, IPattern pattern)
        {
            var matcher = FindMatcher(partitionId);
            MatcherServiceClient client = new MatcherServiceClient(matcher.Url);

            var res = new PartialMatchResult();
            results.TryAdd(res.Id, res);

            client.FindPartialMatch(res.Id, pattern);

            return res;
        }

        public void EndFindPartialMatch(Guid sessionId, IPattern matchedPattern)
        {
            var session = SessionStore.Deafult[sessionId];
            MatcherServiceClient client = new MatcherServiceClient(session.Url);
            client.DonePartialMatch(sessionId, matchedPattern);

            SessionStore.Deafult.DeleteSession(sessionId);
        }

        public INode GetNode(IId partitionId, IId nodeId)
        {
            // try to lookup the node in this module (this should be O(1))
            var node = MatcherModule.Instance.GetNode(nodeId);
            if (node != null)
            {
                return node;
            }

            MatcherInfo matcher = FindMatcher(partitionId);
            MatcherServiceClient client = new MatcherServiceClient(matcher.Url);
            return client.GetNode(nodeId);
        }

        public void ReleasePartialMatchNode(Guid id, IPattern pattern)
        {
            PartialMatchResult res = null;
            results.TryRemove(id, out res);
            res.MatchedPattern = pattern;
            res.Release();
        }

        public void Reset()
        {
            this.results.Clear();
        }

        private MatcherInfo FindMatcher(IId partitionId)
        {
            if (!this.partitionRouting.ContainsKey(partitionId))
            {
                MatcherInfo mi = MatcherModule.Instance.CreatePartitionServiceClient().FindMatcher(partitionId);
                this.partitionRouting.TryAdd(partitionId, mi);
            }

            return this.partitionRouting[partitionId];
        }
    }
}
