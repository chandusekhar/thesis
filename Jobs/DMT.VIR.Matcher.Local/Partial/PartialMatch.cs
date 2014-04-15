using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Data;
using DMT.VIR.Matcher.Local.Patterns;

namespace DMT.VIR.Matcher.Local.Partial
{
    delegate void MatchFoundEventHandler(PartialMatch sender, Pattern matchedPattern);
    delegate void MatchNotFoundEventHandler(PartialMatch sender);

    class PartialMatch : PartialMatchBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Person person;
        private AutoResetEvent signal = new AutoResetEvent(false);

        public PartialMatchState State { get; private set; }

        public event MatchFoundEventHandler MatchFound;
        public event MatchNotFoundEventHandler MatchNotFound;

        public PartialMatch(Person person, IMatcherFramework framework)
            : base(framework)
        {
            this.person = person;
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

        protected override void Start()
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

        protected override bool HandleRemoteNode<T>(MatchNodeArg<T> args)
        {
            // prepare pattern for remote partial search
            Pattern pattern = this.pattern.Copy();
            pattern.CurrentNode = args.NodeToMatch.Id;
            pattern.CurrentPatternNodeName = args.PatternNode.Name;

            var result = Framework.BeginFindPartialMatch(args.IncomingEdge.RemotePartitionId, pattern);
            this.State = PartialMatchState.Pending;
            result.Wait();

            this.pattern.Merge((Pattern)result.MatchedPattern);

            this.State = PartialMatchState.Running;
            // return result.HasMatches;
            // TODO: determine whether there was a match or not
            throw new NotImplementedException();
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
    }
}
