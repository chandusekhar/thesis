using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Data;

namespace DMT.VIR.Matcher.Local
{
    public class VirMatcherJob : IMatcherJob
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string GroupLeaderPost = "körvezető";
        private const string ExGroupLeaderPost = "volt körvezető";
        private const int CommunityScoreThreshold = 60;

        private IPattern pattern;

        public string Name
        {
            get { return "VIR case study matcher - local only implementation"; }
        }

        public event MatcherJobDoneEventHandler Done;

        public void Initialize(IMatcherFramework framework)
        {
            pattern = CreateUnmatchedPattern();
        }

        public void Start(IModel matcherModel, MatchMode mode)
        {
            logger.Info("Starting VIR local matcher.");
            foreach (var node in matcherModel.Nodes)
            {
                this.pattern.Reset();

                var person = node as Person;
                if (person != null)
                {
                    if (TryMatchPerson(person))
                    {
                        logger.Info("Match found: {0}", person.FullName);
                        OnDone(new[] { this.pattern });
                        return;
                    }
                }
            }

            logger.Warn("No match found.");
            OnDone(new IPattern[0]);
        }

        public IEnumerable<object> FindPartialMatch(IId paritionId, IPattern pattern)
        {
            throw new NotSupportedException();
        }

        private IPattern CreateUnmatchedPattern()
        {
            return PatternFactory.CreateUnmatched();
        }

        private void OnDone(IEnumerable<IPattern> matchedPatterns)
        {
            var handler = this.Done;
            if (handler != null)
            {
                handler(this, new MatcherJobDoneEventArgs(matchedPatterns));
            }
        }

        private bool TryMatchPerson(Person person)
        {
            // match person
            this.pattern.GetNodeByName(PatternNodes.Person).MatchedNode = person;

            bool found = false;
            IMatchEdge e;
            INode neighbour;
            foreach (var edge in person.Edges)
            {
                e = (IMatchEdge)edge;
                if (e.IsRemote)
                {
                    continue;
                }

                neighbour = edge.GetOtherNode(person);

                // could not match group leader (maybe because it has already been matched)
                // then try to match an active membership
                if (!TryMatchGroupLeader(neighbour as Membership))
                {
                    // could not match an active membership ((maybe because it has already been matched)
                    // then try to match semester valuations with active
                    if (!TryMatchActiveMembership2(neighbour as Membership))
                    {

                    }
                }

                TryMatchComminityScore(neighbour as CommunityScore);

                if (this.pattern.IsFullyMatched)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private bool TryMatchGroupLeader(Membership ms)
        {
            // trying to match membership:
            // if not null, has not been matched before and has the correct post
            if (ms != null
                && !this.pattern.HasMatchedNodeFor(PatternNodes.GroupLeader)
                && Array.Exists(ms.Posts, p => p == VirMatcherJob.GroupLeaderPost || p == VirMatcherJob.ExGroupLeaderPost))
            {
                this.pattern.GetNodeByName(PatternNodes.GroupLeader).MatchedNode = ms;
                return true;
            }

            return false;
        }

        private bool TryMatchComminityScore(CommunityScore cs)
        {
            if (cs != null
                && !this.pattern.HasMatchedNodeFor(PatternNodes.CommunityScore)
                && cs.Score > VirMatcherJob.CommunityScoreThreshold)
            {
                this.pattern.GetNodeByName(PatternNodes.CommunityScore).MatchedNode = cs;
                return true;
            }

            return false;
        }

        private bool TryMatchActiveMembership2(Membership ms)
        {
            // has a suitable membership.
            if (ms != null
                && !this.pattern.HasMatchedNodeFor(PatternNodes.ActiveMembership2)
                && ms.IsActive)
            {
                IMatchEdge e;
                foreach (var edge in ms.Edges)
                {
                    e = (IMatchEdge)edge;
                    if (!e.IsRemote && e.GetOtherNode(ms) is Group)
                    {
                        // match nodes when there is an available group in the local partition
                        this.pattern.GetNodeByName(PatternNodes.Group2).MatchedNode = e.GetOtherNode(ms);
                        this.pattern.GetNodeByName(PatternNodes.ActiveMembership2).MatchedNode = ms;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
