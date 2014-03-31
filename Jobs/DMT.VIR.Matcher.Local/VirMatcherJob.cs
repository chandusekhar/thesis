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

        // pattern node names
        private const string PersonPatternNode = "person";
        private const string GroupLeaderPatternNode = "group leader";
        private const string CommunityScorePatternNode = "community point";

        private IPattern pattern;

        [Import]
        private IMatcherEntityFactory EntityFactory { get; set; }

        public string Name
        {
            get { return "VIR case study matcher - local only implementation"; }
        }

        public string[] Dependencies
        {
            get { return new[] { "DMT.VIR.Data", "DMT.Core" }; }
        }

        public event MatcherJobDoneEventHandler Done;

        public VirMatcherJob()
        {
            CompositionService.Default.InjectOnce(this);
        }

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
            var person = this.EntityFactory.CreatePatternNode(VirMatcherJob.PersonPatternNode);
            var groupLeader = this.EntityFactory.CreatePatternNode(VirMatcherJob.GroupLeaderPatternNode);
            var communityPoint = this.EntityFactory.CreatePatternNode(VirMatcherJob.CommunityScorePatternNode);

            person.ConnectTo(groupLeader, EdgeDirection.Both);
            person.ConnectTo(communityPoint, EdgeDirection.Both);

            IPattern pattern = this.EntityFactory.CreatePattern();
            pattern.AddNodes(person, groupLeader, communityPoint);

            return pattern;
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
            this.pattern.GetNodeByName(VirMatcherJob.PersonPatternNode).MatchedNode = person;

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

                TryMatchMembership(neighbour as Membership);
                TryMatchComminityScore(neighbour as CommunityScore);

                if (this.pattern.IsFullyMatched)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private bool TryMatchMembership(Membership ms)
        {
            // trying to match membership:
            // if not null, has not been matched before and has the correct post
            if (ms != null
                && !this.pattern.HasMatchedNodeFor(VirMatcherJob.GroupLeaderPatternNode)
                && Array.Exists(ms.Posts, p => p == VirMatcherJob.GroupLeaderPost || p == VirMatcherJob.ExGroupLeaderPost))
            {
                this.pattern.GetNodeByName(VirMatcherJob.GroupLeaderPatternNode).MatchedNode = ms;
                return true;
            }

            return false;
        }

        private bool TryMatchComminityScore(CommunityScore cs)
        {
            if (cs != null
                && !this.pattern.HasMatchedNodeFor(VirMatcherJob.CommunityScorePatternNode)
                && cs.Score > VirMatcherJob.CommunityScoreThreshold)
            {
                this.pattern.GetNodeByName(VirMatcherJob.CommunityScorePatternNode).MatchedNode = cs;
                return true;
            }

            return false;
        }
    }
}
