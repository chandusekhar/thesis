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

        private IModel model;
        private IMatcherFramework framework;
        private CancellationTokenSource cts;

        List<PartialMatch> matchers;

        public string Name
        {
            get { return "VIR Partial matcher"; }
        }

        public bool IsRunning
        {
            get
            {
                return this.matchers.Any(m => m.State == PartialMatchState.Running || m.State == PartialMatchState.Pending);
            }
        }

        public event MatcherJobDoneEventHandler Done;

        public VirPartialMatcherJob()
        {
            this.matchers = new List<PartialMatch>();
        }

        public void Initialize(IMatcherFramework framework)
        {
            this.framework = framework;
        }

        public void StartAsync(IModel matcherModel, MatchMode mode)
        {
            this.model = matcherModel;
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
            this.matchers.ForEach(m => m.Cancel());
            this.cts.Cancel();
        }

        public void FindPartialMatch(Guid sessionId, IPattern pattern)
        {
            var p = (Pattern)pattern;
            var node = this.model.GetNodeDictionary()[p.CurrentNode];
            var lpm = new RemotePartialMatch(sessionId, node, p, framework);
            lpm.StartAsync();
        }

        private void Start(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                logger.Warn("Cancelled before start.");
                return;
            }

            foreach (var person in this.model.Nodes.OfType<Person>())
            {
                var pm = new PartialMatch(person, framework, model);
                pm.MatchFound += ReportMatchFound;
                pm.MatchNotFound += UnsubscribeFromMatcher;

                pm.StartAsync();
                pm.Wait();

                switch (pm.State)
                {
                    case PartialMatchState.MatchFound:
                        return;
                    case PartialMatchState.Pending:
                        this.matchers.Add(pm);
                        continue;
                }

                ct.ThrowIfCancellationRequested();
            }
            OnDone(new IPattern[0]);
        }

        private void ReportMatchFound(PartialMatch sender, Pattern match)
        {
            UnsubscribeFromMatcher(sender);
            OnDone(new IPattern[] { match });
        }

        private void UnsubscribeFromMatcher(PartialMatch sender)
        {
            sender.MatchFound -= ReportMatchFound;
            sender.MatchNotFound -= UnsubscribeFromMatcher;

            lock (this)
            {
                this.matchers.Remove(sender);
            }
        }

        private void OnDone(IEnumerable<IPattern> patterns)
        {
            var handler = this.Done;
            if (handler != null)
            {
                handler(this, new MatcherJobDoneEventArgs(patterns));
            }
        }
    }
}
