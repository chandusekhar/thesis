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
        private static readonly Semester semester = new Semester(2013, 2014, Semester.SemesterPeriod.Autumn); 

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
                var membership = neighbour as Membership;

                // could not match group leader (maybe because it has already been matched)
                // then try to match an active membership
                if (!TryMatchGroupLeader(membership))
                {
                    // could not match an active membership ((maybe because it has already been matched)
                    // then try to match semester valuations with active
                    if (!TryMatchActiveMembership2(membership))
                    {
                        TryMatchSemesterValuation(membership);
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
            if (CheckNode(ms, PatternNodes.GroupLeader,
                n => Array.Exists(n.Posts, p => p == VirMatcherJob.GroupLeaderPost || p == VirMatcherJob.ExGroupLeaderPost)))
            {
                this.pattern.GetNodeByName(PatternNodes.GroupLeader).MatchedNode = ms;
                return true;
            }

            return false;
        }

        private bool TryMatchComminityScore(CommunityScore cs)
        {
            if (CheckNode(cs, PatternNodes.CommunityScore, n => n.Score > VirMatcherJob.CommunityScoreThreshold))
            {
                this.pattern.GetNodeByName(PatternNodes.CommunityScore).MatchedNode = cs;
                return true;
            }

            return false;
        }

        private bool TryMatchActiveMembership2(Membership ms)
        {
            // has a suitable membership.
            if (CheckNode(ms, PatternNodes.ActiveMembership2, n => n.IsActive))
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

        private bool TryMatchSemesterValuation(Membership ms)
        {
            // needs an active membership
            if (CheckNode(ms, PatternNodes.ActiveMembership1, n => n.IsActive))
            {
                foreach (var edge in ms.Edges)
                {
                    var e = (IMatchEdge)edge;
                    if (e.IsRemote)
                    {
                        // skip remote, looking for local only
                        continue;
                    }

                    // has group
                    Group g = e.GetOtherNode(ms) as Group;
                    if (g != null)
                    {
                        foreach (var groupEdge in g.Edges)
                        {
                            if (((IMatchEdge)groupEdge).IsRemote)
                            {
                                continue;
                            }

                            // has semester valuation for the specfied semester
                            SemesterValuation sv = groupEdge.GetOtherNode(g) as SemesterValuation;
                            if (CheckSemesterValuation(sv, PatternNodes.SemesterValuation))
                            {
                                // semester valuation is ok, check for next (or prev) version
                                // versions are symmetric, no need to differentiate between 
                                // how we find these: orig -> next or next <- orig
                                foreach (var svEdge in sv.Edges)
                                {
                                    if (((IMatchEdge)svEdge).IsRemote)
                                    {
                                        continue;
                                    }

                                    SemesterValuation svNext = svEdge.GetOtherNode(sv) as SemesterValuation;
                                    if (CheckSemesterValuation(svNext, PatternNodes.SemesterValuationNext)
                                        && svNext.Edges.Any(svNextEdge => !((IMatchEdge)svNextEdge).IsRemote && svNextEdge.GetOtherNode(svNext) == sv))
                                    {
                                        // everything lines up, set matches for pattern and return
                                        this.pattern.GetNodeByName(PatternNodes.ActiveMembership1).MatchedNode = ms;
                                        this.pattern.GetNodeByName(PatternNodes.Group1).MatchedNode = g;
                                        this.pattern.GetNodeByName(PatternNodes.SemesterValuation).MatchedNode = sv;
                                        this.pattern.GetNodeByName(PatternNodes.SemesterValuationNext).MatchedNode = svNext;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckNode<T>(T node, string name, Predicate<T> predicate) where T : class
        {
            return node != null
                && !this.pattern.HasMatchedNodeFor(name)
                && predicate(node);
        }

        private bool CheckSemesterValuation(SemesterValuation sv, string name)
        {
            return CheckNode(sv, name, n =>
            {
                bool semOk = n.Semester.Equals(VirMatcherJob.semester);
                bool hasPerson = false;

                foreach (var edge in n.Edges)
                {
                    if (((IMatchEdge)edge).IsRemote)
                    {
                        continue;
                    }

                    if (edge.GetOtherNode(n).Equals(this.pattern.GetNodeByName(PatternNodes.Person).Id))
                    {
                        hasPerson = true;
                        break;
                    }
                }

                return semOk && hasPerson;
            });
        }
    }
}
