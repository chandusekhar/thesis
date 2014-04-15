using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Matcher.Local.Patterns;

namespace DMT.VIR.Matcher.Local.Partial
{
    class LocalOnlyPartialMatch : PartialMatchBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Guid sessionId;
        private INode node;
        private Dictionary<string, MatcherFunc> matcherFuncs;

        public LocalOnlyPartialMatch(Guid sessionId, INode node, Pattern pattern, IMatcherFramework framework)
            : base(framework)
        {
            this.sessionId = sessionId;
            this.node = node;
            this.pattern = pattern;

            if (string.IsNullOrEmpty(pattern.CurrentPatternNodeName))
            {
                throw new InvalidOperationException("pattern.CurrentPatternNodeName cannot be empty");
            }

            matcherFuncs = new Dictionary<string, MatcherFunc> {
                { PatternNodes.GroupLeader, TryMatchGroupLeader},
                { PatternNodes.CommunityScore, TryMatchCommunityScore},
                { PatternNodes.ActiveMembership1, TryMatchActiveMembershipForSemesterValuation},
                { PatternNodes.Group1, TryMatchGroupForSemesterValuation},
                { PatternNodes.SemesterValuation, TryMatchSemesterValuation},
                { PatternNodes.SemesterValuationNext, TryMatchNextSemesterValuation},
                { PatternNodes.ActiveMembership2, TryMatchActiveMemebership},
                { PatternNodes.Group2, TryMatchGroup},
            };
        }

        protected override void Start()
        {
            if (!matcherFuncs.ContainsKey(pattern.CurrentPatternNodeName))
            {
                logger.Error("Not supported patter node: " + pattern.CurrentPatternNodeName);
                Framework.EndFindPartialMatch(sessionId, null);
            }

            matcherFuncs[pattern.CurrentPatternNodeName](node, null);
            Framework.EndFindPartialMatch(sessionId, pattern);
        }

        protected override bool HandleRemoteNode<T>(MatchNodeArg<T> args)
        {
            // always return false for remote node partial match
            return false;
        }
    }
}
