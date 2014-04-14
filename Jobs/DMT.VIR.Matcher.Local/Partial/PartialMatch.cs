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
    delegate void MatchFoundEventHandler(PartialMatch sender, Pattern matchedPattern);
    delegate void MatchNotFoundEventHandler(PartialMatch sender);

    class PartialMatch
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private delegate bool MatcherFunc(INode node, IMatchEdge incomingEdge);

        private Person person;
        private Pattern pattern;
        private IMatcherFramework framework;
        private List<MatcherFunc> matcherFuncs;

        private CancellationTokenSource cts;
        private AutoResetEvent signal = new AutoResetEvent(false);

        public event MatchFoundEventHandler MatchFound;
        public event MatchNotFoundEventHandler MatchNotFound;

        public PartialMatchState State { get; private set; }

        public PartialMatch(Person person, IMatcherFramework framework)
        {
            this.person = person;
            this.framework = framework;

            this.pattern = PatternFactory.CreateUnmatched();
            this.matcherFuncs = new List<MatcherFunc>
            {
                TryMatchGroupLeader,
                TryMatchCommunityScore,
                TryMatchActiveMemebership,
                TryMatchActiveMembershipForSemesterValuation,
            };

            this.cts = new CancellationTokenSource();
            this.State = PartialMatchState.ReadyToStart;
        }

        public void Cancel()
        {
            this.cts.Cancel();
            this.State = PartialMatchState.Cancelled;
        }

        public void Wait()
        {
            this.signal.WaitOne();
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

        private void Start()
        {
            if (State != PartialMatchState.ReadyToStart)
            {
                logger.Warn("Tried to start and already started matcher.");
                return;
            }

            this.State = PartialMatchState.Running;

            if (cts.Token.IsCancellationRequested)
            {
                logger.Warn("Cancelled before start.");
                return;
            }

            if (TryMatchPerson(this.person))
            {
                logger.Info("Match found: {0}", this.person.FullName);
                OnMatchFound();
                this.State = PartialMatchState.MatchFound;
            }
            else
            {
                OnMatchNotFound();
                this.State = PartialMatchState.MatchNotFound;
            }

            this.signal.Set();
        }

        private void OnMatchFound()
        {
            var handler = this.MatchFound;
            if (handler != null)
            {
                handler(this, this.pattern);
            }
        }

        private void OnMatchNotFound()
        {
            var handler = this.MatchNotFound;
            if (handler != null)
            {
                handler(this);
            }
        }

        #region matcher functions

        private bool TryMatchPerson(Person person)
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

        private bool TryMatchGroupLeader(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<Membership>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.GroupLeader),
                Predicate = n => PatternCriteria.HasGroupLeader(n.Posts),
                IncomingEdge = incomingEdge,
            });
        }

        private bool TryMatchCommunityScore(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<CommunityScore>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.CommunityScore),
                Predicate = new Predicate<CommunityScore>(PatternCriteria.CheckCommunityScore),
                IncomingEdge = incomingEdge,
            });
        }

        private bool TryMatchActiveMemebership(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern.GetNodeByName(PatternNodes.ActiveMembership2),
                n => n.IsActive,
                TryMatchGroup,
                incomingEdge
            );
        }

        private bool TryMatchGroup(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNode(new MatchNodeArg<Group>
            {
                NodeToMatch = node,
                PatternNode = pattern.GetNodeByName(PatternNodes.Group2),
                IncomingEdge = incomingEdge,
                Predicate = n => true,
            });
        }

        private bool TryMatchActiveMembershipForSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Membership>(
                node,
                pattern.GetNodeByName(PatternNodes.ActiveMembership1),
                n => n.IsActive,
                TryMatchGroupForSemesterValuation,
                incomingEdge
            );
        }

        private bool TryMatchGroupForSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<Group>(
                node,
                pattern.GetNodeByName(PatternNodes.Group1),
                n => true,
                TryMatchSemesterValuation,
                incomingEdge
            );
        }

        private bool TryMatchSemesterValuation(INode node, IMatchEdge incomingEdge)
        {
            return TryMatchNodeWithNext<SemesterValuation>(
                node,
                pattern.GetNodeByName(PatternNodes.SemesterValuation),
                n => n.Semester.Equals(PatternCriteria.Semester),
                TryMatchNexSemesterValuation,
                incomingEdge
            );
        }

        private bool TryMatchNexSemesterValuation(INode node, IMatchEdge incomingEdge)
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

        private bool HandleRemoteNode<T>(MatchNodeArg<T> args) where T : class, INode
        {
            // prepare pattern for remote partial search
            Pattern pattern = this.pattern.Copy();
            pattern.CurrentNode = args.NodeToMatch.Id;
            pattern.CurrentPatternNodeName = args.PatternNode.Name;

            // create block signal for remote search
            AutoResetEvent remoteSignal = new AutoResetEvent(false);

            bool result = false;

            var partialMatchDone = new FoundPartialMatchEventHandler((s, e) =>
            {
                // save match result
                result = e.Matches.Any();

                var match = e.Matches.FirstOrDefault();
                if (match != null)
                {
                    logger.Debug("Found partial match");
                    this.pattern.Merge((Pattern)match);
                }

                // unlock remote block
                remoteSignal.Set();
            });
            
            framework.FoundPartialMatch += partialMatchDone;
            framework.BeginFindPartialMatch(args.IncomingEdge.RemotePartitionId, pattern);
            this.State = PartialMatchState.Pending;
            // release outside world's lock
            this.signal.Set();

            // wait for the remote search to end
            remoteSignal.WaitOne();
            framework.FoundPartialMatch -= partialMatchDone;

            return result;
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
