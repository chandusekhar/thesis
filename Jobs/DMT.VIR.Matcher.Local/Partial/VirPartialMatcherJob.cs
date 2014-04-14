using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Data;
using DMT.VIR.Matcher.Local.Patterns;

namespace DMT.VIR.Matcher.Local.Partial
{
    public class VirPartialMatcherJob : IMatcherJob
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private delegate bool MatcherFunc(INode node, Pattern pattern, IMatchEdge incomingEdge);

        private IModel model;
        private IMatcherFramework framework;
        private List<MatcherFunc> matcherFuncs;
        private CancellationTokenSource cts;

        public string Name
        {
            get { return "VIR Partial matcher"; }
        }

        public bool IsRunning { get; private set; }

        public event MatcherJobDoneEventHandler Done;

        public VirPartialMatcherJob()
        {
            this.matcherFuncs = new List<MatcherFunc>
            {
                TryMatchGroupLeader,
                TryMatchCommunityScore,
                TryMatchActiveMemebership,
                TryMatchActiveMembershipForSemesterValuation,
            };
        }

        public void Initialize(IMatcherFramework framework)
        {
            this.framework = framework;
        }

        public void StartAsync(IModel matcherModel, MatchMode mode)
        {
            this.model = matcherModel;
            this.IsRunning = true;
            this.cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    Start(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    logger.Info("{0} was cancelled.", this.Name);
                }
            }, cts.Token);
        }

        public void Cancel()
        {
            this.IsRunning = false;
            this.cts.Cancel();
        }

        public IEnumerable<object> FindPartialMatch(IPattern pattern)
        {
            throw new NotImplementedException();
        }

        private void Start(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                logger.Warn("Cancelled before start.");
                return;
            }

            Pattern pattern = PatternFactory.CreateUnmatched();
            foreach (var person in this.model.Nodes.OfType<Person>())
            {
                pattern.Reset();
                if (TryMatchPerson(person, pattern, ct))
                {
                    logger.Info("Match found: {0}", person.FullName);
                    OnDone(new IPattern[] { pattern });
                    return;
                }

                ct.ThrowIfCancellationRequested();
            }
            OnDone(new IPattern[0]);
        }

        private void OnDone(IEnumerable<IPattern> patterns)
        {
            var handler = this.Done;
            if (handler != null)
            {
                handler(this, new MatcherJobDoneEventArgs(patterns));
            }
        }

        /// <summary>
        /// Handles remote node if necessary. Blocks the thread until the
        /// </summary>
        /// <param name="remoteNode"></param>
        /// <param name="pattern"></param>
        /// <returns>true if node is handle remotely, false otherwise</returns>
        private bool HandleRemoteNode(INode remoteNode, Pattern pattern)
        {
            throw new NotImplementedException();
        }

        private bool TryMatchPerson(Person person, Pattern pattern, CancellationToken ct)
        {
            // always match a person
            pattern.SetMatchedNodeForPatternNode(PatternNodes.Person, person);

            // let's walk the person's edges
            foreach (var edge in person.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(person);

                foreach (var match in this.matcherFuncs)
                {
                    if (match(neighbour, pattern, edge))
                    {
                        break;
                    }
                }

                if (pattern.IsMatched)
                {
                    return true;
                }

                ct.ThrowIfCancellationRequested();
            }

            return false;
        }

        private bool TryMatchGroupLeader(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNode<Membership>(node, pattern.GetNodeByName(PatternNodes.GroupLeader), n => PatternCriteria.HasGroupLeader(n.Posts));
        }

        private bool TryMatchCommunityScore(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNode<CommunityScore>(node, pattern.GetNodeByName(PatternNodes.CommunityScore), PatternCriteria.CheckCommunityScore);
        }

        private bool TryMatchActiveMemebership(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.ActiveMembership2),
                n => n.IsActive,
                TryMatchGroup
            );
        }

        private bool TryMatchGroup(INode neighbour, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNode<Group>(neighbour, pattern.GetNodeByName(PatternNodes.Group2), n => true);
        }

        private bool TryMatchActiveMembershipForSemesterValuation(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.ActiveMembership1),
                n => n.IsActive,
                TryMatchGroupForSemesterValuation
            );
        }

        private bool TryMatchGroupForSemesterValuation(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Group>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.Group1),
                n => true,
                TryMatchSemesterValuation
            );
        }

        private bool TryMatchSemesterValuation(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNodeWithNext<SemesterValuation>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.SemesterValuation),
                n => n.Semester.Equals(PatternCriteria.Semester),
                TryMatchNexSemesterValuation
            );
        }

        private bool TryMatchNexSemesterValuation(INode node, Pattern pattern, IEdge incomingEdge)
        {
            return TryMatchNode<SemesterValuation>(node, pattern.GetNodeByName(PatternNodes.SemesterValuationNext), n => n.Semester.Equals(PatternCriteria.Semester));
        }

        private bool TryMatchNodeWithNext<T>(INode node, Pattern pattern, PatternNode patternNode, Predicate<T> predicate, MatcherFunc next) where T: class, INode
        {
            if (!TryMatchNode<T>(node, patternNode, predicate))
            {
                return false;
            }

            bool isSubpatternMatch = false;
            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(node);
                if (next(neighbour, pattern, edge))
                {
                    isSubpatternMatch = true;
                    break;
                }
            }

            if (!isSubpatternMatch)
            {
                // clear
                patternNode.MatchedNode = null;
            }

            return isSubpatternMatch;
        }

        private bool TryMatchNode<T>(INode node, PatternNode patternNode, Predicate<T> predicate) where T : class, INode
        {
            T typedNode = node as T;
            if (typedNode != null && !patternNode.IsMatched && predicate(typedNode) && CheckNeighbours(node, patternNode))
            {
                patternNode.MatchedNode = node;
                return true;
            }

            return false;
        }

        private bool CheckNeighbours(INode node, PatternNode patternNode)
        {
            foreach (var edge in patternNode.Edges)
            {
                PatternNode n = (PatternNode)edge.GetOtherNode(patternNode);
                if (n.IsMatched && !n.MatchedNode.IsNeighbour(node).Success)
                {
                    return false;
                }
            }

            return true;
        }
    }

}
