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
    abstract class PartialMatchBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected delegate bool MatcherFunc(INode node, IMatchEdge incomingEdge);

        protected Pattern pattern;
        private IMatcherFramework framework;
        private List<MatcherFunc> matcherFuncs;

        protected CancellationTokenSource cts;

        protected IMatcherFramework Framework
        {
            get { return this.framework; }
        }

        public PartialMatchBase(IMatcherFramework framework)
        {
            this.framework = framework;
            this.cts = new CancellationTokenSource();
            this.pattern = PatternFactory.CreateUnmatched();
            this.matcherFuncs = new List<MatcherFunc>
            {
                TryMatchGroupLeader,
                TryMatchCommunityScore,
                TryMatchActiveMemebership,
                TryMatchActiveMembershipForSemesterValuation,
            };
        }

        public void StartAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    Start();
                }
                catch (OperationCanceledException)
                {
                    logger.Warn("Matcher was cancelled.");
                }
            }, cts.Token);
        }

        protected abstract void Start();

        protected abstract bool HandleRemoteNode<T>(MatchNodeArg<T> args) where T : class, INode;

        #region matcher functions

        protected bool TryMatchPerson(Person person)
        {
            // always match a person
            pattern.SetMatchedNodeForPatternNode(PatternNodes.Person, person);

            // let's walk the person's edges
            foreach (var edge in person.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(person);

                foreach (var match in this.matcherFuncs)
                {
                    if (match(neighbour, edge))
                    {
                        break;
                    }
                }

                if (pattern.IsMatched)
                {
                    return true;
                }

                cts.Token.ThrowIfCancellationRequested();
            }

            return false;
        }

        protected bool TryMatchGroupLeader(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<Membership>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.GroupLeader),
                Predicate = n => PatternCriteria.HasGroupLeader(n.Posts),
                IncomingEdge = incomingEdge,
            });
        }

        protected bool TryMatchCommunityScore(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<CommunityScore>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.CommunityScore),
                Predicate = new Predicate<CommunityScore>(PatternCriteria.CheckCommunityScore),
                IncomingEdge = incomingEdge,
            });
        }

        protected bool TryMatchActiveMemebership(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern.GetNodeByName(PatternNodes.ActiveMembership2),
                n => n.IsActive,
                TryMatchGroup,
                incomingEdge
            );
        }

        protected bool TryMatchGroup(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<Group>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.Group2),
                IncomingEdge = incomingEdge,
                Predicate = n => true,
            });
        }

        protected bool TryMatchActiveMembershipForSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern.GetNodeByName(PatternNodes.ActiveMembership1),
                n => n.IsActive,
                TryMatchGroupForSemesterValuation,
                incomingEdge
            );
        }

        protected bool TryMatchGroupForSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Group>(
                node,
                pattern.GetNodeByName(PatternNodes.Group1),
                n => true,
                TryMatchSemesterValuation,
                incomingEdge
            );
        }

        protected bool TryMatchSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<SemesterValuation>(
                node,
                pattern.GetNodeByName(PatternNodes.SemesterValuation),
                n => n.Semester.Equals(PatternCriteria.Semester),
                TryMatchNextSemesterValuation,
                incomingEdge
            );
        }

        protected bool TryMatchNextSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<SemesterValuation>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.SemesterValuationNext),
                IncomingEdge = incomingEdge,
                Predicate = n => n.Semester.Equals(PatternCriteria.Semester),
            });
        }

        private bool TryMatchNodeWithNext<T>(INode node, PatternNode patternNode, Predicate<T> predicate, MatcherFunc next, IMatchEdge incomingEdge) where T : class, INode
        {
            if (!TryMatchNode(new MatchNodeArg<T>(node, patternNode, predicate, incomingEdge)))
            {
                return false;
            }

            bool isSubpatternMatch = false;
            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(node);
                if (next(neighbour, edge))
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
            if (args.IsRemote)
            {
                return HandleRemoteNode(args);
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

        #endregion
    }
}
