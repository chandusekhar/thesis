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

        /// <summary>
        /// Determines whether this matcher instance is running a remote or
        /// a normal match.
        /// </summary>
        protected virtual bool IsRemote
        {
            get { return false; }
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

        protected abstract NodeMatchResult HandleRemoteNode<T>(MatchNodeArg<T> args) where T : class, INode;

        /// <summary>
        /// This method is called when a remote partial match has returned but
        /// there was no full match.
        /// </summary>
        /// <returns>true of there is a full match</returns>
        protected abstract bool FollowupOnRemoteNode(INode node, PatternNode patternNode, MatcherFunc next);

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
            return TryMatchNode(new MatchNodeArg<Membership>(
                node,
                pattern.GetNodeByName(PatternNodes.GroupLeader),
                n => PatternCriteria.HasGroupLeader(n.Posts),
                incomingEdge)).IsMatched;
        }

        protected bool TryMatchCommunityScore(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<CommunityScore>(
                node,
                pattern.GetNodeByName(PatternNodes.CommunityScore),
                new Predicate<CommunityScore>(PatternCriteria.CheckCommunityScore),
                incomingEdge)).IsMatched;
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
            return TryMatchNode(new MatchNodeArg<Group>(
                node,
                pattern.GetNodeByName(PatternNodes.Group2),
                n => true,
                incomingEdge)).IsMatched;
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
            return TryMatchNode(new MatchNodeArg<SemesterValuation>(
                node,
                pattern.GetNodeByName(PatternNodes.SemesterValuationNext),
                n => n.Semester.Equals(PatternCriteria.Semester),
                incomingEdge)).IsMatched;
        }

        private bool TryMatchNodeWithNext<T>(INode node, PatternNode patternNode, Predicate<T> predicate, MatcherFunc next, IMatchEdge incomingEdge) where T : class, INode
        {
            var res = TryMatchNode(new MatchNodeArg<T>(node, patternNode, predicate, incomingEdge));
            if (!res.IsMatched)
            {
                return false;
            }

            // when already matched the whole subpattern, no need to look further
            // this can happen if returning from a remote match
            if (res.IsMatched && res.IsFullSubpatternMatched)
            {
                return true;
            }

            bool isSubpatternMatch = false;

            // patternNode.RemoteEdges is only populated when a
            // remote node returns
            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(node);
                if (next(neighbour, edge))
                {
                    isSubpatternMatch = true;
                    break;
                }
            }

            // no match found in local partition -> check remotes as well
            // but only if this is the initially started matching (not a remote one)
            if (!isSubpatternMatch && !this.IsRemote)
            {
                isSubpatternMatch = FollowupOnRemoteNode(node, patternNode, next);

                // when no match found clear the pattern node
                if (!isSubpatternMatch)
                {
                    patternNode.Reset();
                }
            }

            return isSubpatternMatch;
        }

        private NodeMatchResult TryMatchNode<T>(MatchNodeArg<T> args) where T : class, INode
        {
            // handling remote edge
            if (args.IsRemote && args.NodeToMatch is IRemoteNode)
            {
                return HandleRemoteNode(args);
            }

            T typedNode = args.NodeToMatch as T;
            if (typedNode != null 
                    && !args.PatternNode.IsMatched 
                    && args.Predicate(typedNode)
                    && !this.pattern.GetMatchedNodes().Contains(typedNode)
                    && CheckNeighbours(args.NodeToMatch, args.PatternNode))
            {
                args.MarkMatch();

                if (this.IsRemote)
                {
                    // when searching in a remote partition 
                    // on match set up pattern node's remote edges
                    args.PatternNode.CopyRemoteEdgesFrom(typedNode);
                }

                return new NodeMatchResult(true);
            }

            return new NodeMatchResult(false);
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
