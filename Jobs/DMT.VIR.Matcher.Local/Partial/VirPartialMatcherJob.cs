using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private IModel model;
        private IMatcherFramework framework;
        private List<Func<INode, Pattern, bool>> matcherFuncs;

        public string Name
        {
            get { return "VIR Partial matcher"; }
        }

        public bool IsRunning
        {
            get { return false; }
        }

        public event MatcherJobDoneEventHandler Done;

        public VirPartialMatcherJob()
        {
            this.matcherFuncs = new List<Func<INode, Pattern, bool>>
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
            Task.Run(new Action(Start));
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> FindPartialMatch(IPattern pattern)
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            Pattern pattern = PatternFactory.CreateUnmatched();
            foreach (var person in this.model.Nodes.OfType<Person>())
            {
                pattern.Reset();
                if (TryMatchPerson(person, pattern))
                {
                    logger.Info("Match found: {0}", person.FullName);
                    OnDone(new IPattern[] { pattern });
                    return;
                }
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

        private bool TryMatchPerson(Person person, Pattern pattern)
        {
            // always match a person
            pattern.SetMatchedNodeForPatternNode(PatternNodes.Person, person);

            // let's walk the person's edges
            foreach (var edge in person.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(person);

                foreach (var match in this.matcherFuncs)
                {
                    if (match(neighbour, pattern))
                    {
                        break;
                    }
                }

                if (pattern.IsMatched)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryMatchGroupLeader(INode node, Pattern pattern)
        {
            return TryMatchNode<Membership>(node, pattern.GetNodeByName(PatternNodes.GroupLeader), n => PatternCriteria.HasGroupLeader(n.Posts));
        }

        private bool TryMatchCommunityScore(INode node, Pattern pattern)
        {
            return TryMatchNode<CommunityScore>(node, pattern.GetNodeByName(PatternNodes.CommunityScore), PatternCriteria.CheckCommunityScore);
        }

        private bool TryMatchActiveMemebership(INode node, Pattern pattern)
        {
            if (!TryMatchNode<Membership>(node, pattern.GetNodeByName(PatternNodes.ActiveMembership2), ms => ms.IsActive))
            {
                return false;
            }
            bool isSubpatternMatch = false;

            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(node);
                if (TryMatchGroup(neighbour, pattern.GetNodeByName(PatternNodes.Group2)))
                {
                    isSubpatternMatch = true;
                    break;
                }
            }

            if (!isSubpatternMatch)
            {
                // clear matched nodes
                pattern.SetMatchedNodeForPatternNode(PatternNodes.ActiveMembership2, null);
            }

            return isSubpatternMatch;
        }

        private bool TryMatchGroup(INode neighbour, PatternNode patternNode)
        {
            return TryMatchNode<Group>(neighbour, patternNode, n => true);
        }

        private bool TryMatchActiveMembershipForSemesterValuation(INode node, Pattern pattern)
        {
            // semester valuation subpattern needs an active membership first
            if (!TryMatchNode<Membership>(node, pattern.GetNodeByName(PatternNodes.ActiveMembership1), n => n.IsActive))
            {
                // return immediately if no active membersip
                return false;
            }

            bool isSubpatternMatch = false;

            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neighbour = edge.GetOtherNode(node);
                if (TryMatchGroupForSemesterValuation(neighbour, pattern))
                {
                    isSubpatternMatch = true;
                    break;
                }
            }

            if (!isSubpatternMatch)
            {
                // clear
                pattern.SetMatchedNodeForPatternNode(PatternNodes.ActiveMembership1, null);
            }

            return isSubpatternMatch;
        }

        private bool TryMatchGroupForSemesterValuation(INode node, Pattern pattern)
        {
            if (!TryMatchNode<Group>(node, pattern.GetNodeByName(PatternNodes.Group1), n => true))
            {
                return false;
            }

            bool isSubpatternMatch = false;
            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neigbour = edge.GetOtherNode(node);
                if (TryMatchSemesterValuation(neigbour, pattern))
                {
                    isSubpatternMatch = true;
                    break;
                }
            }

            if (!isSubpatternMatch)
            {
                // clear
                pattern.SetMatchedNodeForPatternNode(PatternNodes.Group1, null);
            }

            return isSubpatternMatch;
        }

        private bool TryMatchSemesterValuation(INode node, Pattern pattern)
        {
            if (!TryMatchNode<SemesterValuation>(node, pattern.GetNodeByName(PatternNodes.SemesterValuation), n => n.Semester.Equals(PatternCriteria.Semester)))
            {
                return false;
            }

            bool isSubpatternMatch = false;
            foreach (var edge in node.Edges.Cast<IMatchEdge>())
            {
                INode neigbour = edge.GetOtherNode(node);
                if (TryMatchNexSemesterValuation(neigbour, pattern))
                {
                    isSubpatternMatch = true;
                    break;
                }
            }

            if (!isSubpatternMatch)
            {
                // clear
                pattern.SetMatchedNodeForPatternNode(PatternNodes.SemesterValuation, null);
            }

            return isSubpatternMatch;
        }

        private bool TryMatchNexSemesterValuation(INode node, Pattern pattern)
        {
            return TryMatchNode<SemesterValuation>(node, pattern.GetNodeByName(PatternNodes.SemesterValuationNext), n => n.Semester.Equals(PatternCriteria.Semester));
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
