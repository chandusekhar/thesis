using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Data;
using DMT.VIR.Matcher.Local.Patterns;

namespace DMT.VIR.Matcher.Local
{
    public abstract class VirMatcherJobBase : IMatcherJob
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Pattern pattern;

        private bool isRunning = false;
        private CancellationTokenSource cancellationTokenSource;

        public abstract string Name { get; }

        public bool IsRunning
        {
            get { return this.isRunning; }
        }

        public event MatcherJobDoneEventHandler Done;

        public virtual void Initialize(IMatcherFramework framework)
        {
            this.pattern = CreateUnmatchedPattern();
        }

        public void StartAsync(IModel matcherModel, MatchMode mode)
        {
            this.isRunning = true;
            this.cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = this.cancellationTokenSource.Token;

            Task.Run(() =>
            {
                try
                {
                    Start(matcherModel, mode, ct);
                }
                catch (OperationCanceledException)
                {
                    logger.Info("{0} was cancelled", this.Name);
                }
            }, ct);
        }

        public virtual void Cancel()
        {
            this.isRunning = false;
            this.cancellationTokenSource.Cancel();
        }

        public virtual void FindPartialMatch(Guid sessionId, IPattern pattern)
        {
            throw new NotSupportedException();
        }

        protected virtual void Start(IModel matcherModel, MatchMode mode, CancellationToken ct)
        {
            // cancelled before start?
            if (ct.IsCancellationRequested)
            {
                logger.Warn("Cancelled before start");
                ct.ThrowIfCancellationRequested();
            }

            foreach (var person in matcherModel.Nodes.OfType<Person>())
            {
                this.pattern.Reset();
                if (TryMatchPerson(person, ct))
                {
                    logger.Info("Match found: {0}", person.FullName);
                    OnDone(new[] { this.pattern });
                    return;
                }

                ct.ThrowIfCancellationRequested();
            }

            logger.Warn("No match found.");
            OnDone(new IPattern[0]);
        }

        protected void OnDone(IEnumerable<IPattern> matchedPatterns)
        {
            this.isRunning = false;
            var handler = this.Done;
            if (handler != null)
            {
                handler(this, new MatcherJobDoneEventArgs(matchedPatterns));
            }
        }

        protected virtual bool SkipEdge(IMatchEdge edge)
        {
            return false;
        }

        protected virtual T ConvertNode<T>(IMatchEdge incomingEdge, INode node) where T : class, INode
        {
            return node as T;
        }

        private bool TryMatchPerson(Person person, CancellationToken ct)
        {
            // match person
            this.pattern.SetMatchedNodeForPatternNode(PatternNodes.Person, person);

            bool found = false;
            IMatchEdge e;
            INode neighbour;
            foreach (var edge in person.Edges.Cast<IMatchEdge>())
            {
                if (SkipEdge(edge))
                {
                    continue;
                }

                neighbour = edge.GetOtherNode(person);
                var membership = ConvertNode<Membership>(edge, neighbour);

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

                TryMatchComminityScore(ConvertNode<CommunityScore>(edge, neighbour));

                if (this.pattern.IsMatched)
                {
                    found = true;
                    break;
                }

                // check for cancellation after every edge
                ct.ThrowIfCancellationRequested();
            }

            return found;
        }

        private bool TryMatchGroupLeader(Membership ms)
        {
            // trying to match membership:
            // if not null, has not been matched before and has the correct post
            if (CheckNode(ms, PatternNodes.GroupLeader,
                n => Array.Exists(n.Posts, p => p == PatternCriteria.GroupLeaderPost || p == PatternCriteria.ExGroupLeaderPost)))
            {
                this.pattern.SetMatchedNodeForPatternNode(PatternNodes.GroupLeader, ms);
                return true;
            }

            return false;
        }

        private bool TryMatchComminityScore(CommunityScore cs)
        {
            var match = new Predicate<CommunityScore>(n =>
            {
                return n.Score > PatternCriteria.CommunityScoreThreshold
                    && PatternCriteria.Semester.Equals(n.Semester);
            });

            if (CheckNode(cs, PatternNodes.CommunityScore, match))
            {
                this.pattern.SetMatchedNodeForPatternNode(PatternNodes.CommunityScore, cs);
                return true;
            }

            return false;
        }

        private bool TryMatchActiveMembership2(Membership ms)
        {
            // has a suitable membership.
            if (CheckNode(ms, PatternNodes.ActiveMembership2, n => n.IsActive))
            {
                foreach (var edge in ms.Edges.Cast<IMatchEdge>())
                {
                    if (SkipEdge(edge))
                    {
                        continue;
                    }
                    Group g = ConvertNode<Group>(edge, edge.GetOtherNode(ms));
                    if (g != null)
                    {
                        // match nodes when there is an available group in the local partition
                        this.pattern.SetMatchedNodeForPatternNode(PatternNodes.Group2, g);
                        this.pattern.SetMatchedNodeForPatternNode(PatternNodes.ActiveMembership2, ms);
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
                foreach (var edge in ms.Edges.Cast<IMatchEdge>())
                {
                    if (SkipEdge(edge))
                    {
                        // skip remote, looking for local only
                        continue;
                    }

                    // has group
                    Group g = ConvertNode<Group>(edge, edge.GetOtherNode(ms));
                    if (g != null)
                    {
                        foreach (var groupEdge in g.Edges.Cast<IMatchEdge>())
                        {
                            if (SkipEdge(edge))
                            {
                                continue;
                            }

                            // has semester valuation for the specfied semester
                            SemesterValuation sv = ConvertNode<SemesterValuation>(groupEdge, groupEdge.GetOtherNode(g));
                            if (CheckSemesterValuation(sv, PatternNodes.SemesterValuation))
                            {
                                // semester valuation is ok, check for next (or prev) version
                                // versions are symmetric, no need to differentiate between 
                                // how we find these: orig -> next or next <- orig
                                foreach (var svEdge in sv.Edges.Cast<IMatchEdge>())
                                {
                                    if (SkipEdge(svEdge))
                                    {
                                        continue;
                                    }

                                    SemesterValuation svNext = ConvertNode<SemesterValuation>(svEdge, svEdge.GetOtherNode(sv));
                                    if (CheckSemesterValuation(svNext, PatternNodes.SemesterValuationNext)
                                        && svNext.Edges.Any(svNextEdge => !((IMatchEdge)svNextEdge).IsRemote && svNextEdge.GetOtherNode(svNext) == sv))
                                    {
                                        // everything lines up, set matches for pattern and return
                                        this.pattern.SetMatchedNodeForPatternNode(PatternNodes.ActiveMembership1, ms);
                                        this.pattern.SetMatchedNodeForPatternNode(PatternNodes.Group1, g);
                                        this.pattern.SetMatchedNodeForPatternNode(PatternNodes.SemesterValuation, sv);
                                        this.pattern.SetMatchedNodeForPatternNode(PatternNodes.SemesterValuationNext, svNext);
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
                bool semOk = n.Semester.Equals(PatternCriteria.Semester);
                bool hasPerson = false;
                var person = this.pattern.GetNodeByName(PatternNodes.Person).MatchedNode;

                foreach (var edge in n.Edges.Cast<IMatchEdge>())
                {
                    if (SkipEdge(edge))
                    {
                        continue;
                    }

                    if (edge.GetOtherNode(n).Id.Equals(person.Id))
                    {
                        hasPerson = true;
                        break;
                    }
                }

                return semOk && hasPerson;
            });
        }

        private Pattern CreateUnmatchedPattern()
        {
            return PatternFactory.CreateUnmatched();
        }
    }
}
