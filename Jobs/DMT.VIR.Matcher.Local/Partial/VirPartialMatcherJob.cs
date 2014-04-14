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
        private void HandleRemoteNode<T>(MatchNodeArg<T> args) where T : class, INode
        {
            var pattern = args.Pattern.Copy();
            pattern.CurrentNode = args.NodeToMatch.Id;
            pattern.CurrentPatternNodeName = args.PatternNode.Name;

            framework.BeginFindPartialMatch(args.IncomingEdge.RemotePartitionId, pattern);
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

        private bool TryMatchGroupLeader(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<Membership>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.GroupLeader),
                Predicate = n => PatternCriteria.HasGroupLeader(n.Posts),
                IncomingEdge = incomingEdge,
                Pattern = pattern,
            });
        }

        private bool TryMatchCommunityScore(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<CommunityScore>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.CommunityScore),
                Predicate = new Predicate<CommunityScore>(PatternCriteria.CheckCommunityScore),
                IncomingEdge = incomingEdge,
                Pattern = pattern,
            });
        }

        private bool TryMatchActiveMemebership(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.ActiveMembership2),
                n => n.IsActive,
                TryMatchGroup,
                incomingEdge
            );
        }

        private bool TryMatchGroup(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<Group>
            {
                NodeToMatch = node,
                Pattern = pattern,
                PatternNode = pattern.GetNodeByName(PatternNodes.Group2),
                IncomingEdge = incomingEdge,
                Predicate = n => true,
            });
        }

        private bool TryMatchActiveMembershipForSemesterValuation(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.ActiveMembership1),
                n => n.IsActive,
                TryMatchGroupForSemesterValuation,
                incomingEdge
            );
        }

        private bool TryMatchGroupForSemesterValuation(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Group>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.Group1),
                n => true,
                TryMatchSemesterValuation,
                incomingEdge
            );
        }

        private bool TryMatchSemesterValuation(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<SemesterValuation>(
                node,
                pattern,
                pattern.GetNodeByName(PatternNodes.SemesterValuation),
                n => n.Semester.Equals(PatternCriteria.Semester),
                TryMatchNexSemesterValuation,
                incomingEdge
            );
        }

        private bool TryMatchNexSemesterValuation(INode node, Pattern pattern, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<SemesterValuation>
            {
                NodeToMatch = node,
                Pattern = pattern,
                PatternNode = pattern.GetNodeByName(PatternNodes.SemesterValuationNext),
                IncomingEdge = incomingEdge,
                Predicate = n => n.Semester.Equals(PatternCriteria.Semester),
            });
        }

        private bool TryMatchNodeWithNext<T>(INode node, Pattern pattern, PatternNode patternNode, Predicate<T> predicate, MatcherFunc next, IMatchEdge incomingEdge) where T : class, INode
        {
            if (!TryMatchNode(new MatchNodeArg<T>(node, patternNode, predicate, incomingEdge, pattern)))
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

        private bool TryMatchNode<T>(MatchNodeArg<T> args) where T : class, INode
        {
            // handling remote edge
            if (args.IsRemote) {
                HandleRemoteNode(args);
                // returning false, so the local search can be continued undisturbed
                return false;
            }

            T typedNode = args.NodeToMatch as T;
            if (typedNode != null && !args.PatternNode.IsMatched && args.Predicate(typedNode) && CheckNeighbours(args.NodeToMatch, args.PatternNode))
            {
                args.MarkMatch();
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
