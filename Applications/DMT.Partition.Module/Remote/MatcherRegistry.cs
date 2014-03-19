using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Module.Exceptions;
using DMT.Module.Common.Service;
using DMT.Core.Interfaces;
using DMT.Matcher.Interfaces;

namespace DMT.Partition.Module.Remote
{
    class MatcherRegistry
    {
        private object syncObject = new object();
        private List<MatcherInfo> matchers;

        public event EventHandler MatchersReady;

        public event EventHandler MatchersDone;

        public MatcherRegistry()
        {
            this.matchers = new List<MatcherInfo>();
        }

        public void AddMatcher(MatcherInfo matcherinfo)
        {
            lock (syncObject)
            {
                this.matchers.Add(matcherinfo);
            }
        }

        public bool RemoveMatcher(Guid id)
        {
            lock (syncObject)
            {
                return this.matchers.RemoveAll(m => m.Id == id) > 0;
            }
        }

        public MatcherInfo GetById(Guid id)
        {
            var matcher = this.matchers.FirstOrDefault(m => m.Id == id);
            if (matcher == null)
            {
                throw new MatcherNotFoundException(string.Format("Matcher with {0} id was not found.", id));
            }

            return matcher;
        }

        public void MarkReady(Guid id)
        {
            this.GetById(id).Ready = true;

            if (matchers.All(m => m.Ready))
            {
                OnMatchersReady();
            }
        }

        public void MarkDone(Guid id)
        {
            this.GetById(id).Done = true;

            if (matchers.All(m => m.Done))
            {
                this.matchers.ForEach(m => m.Ready = false);
                OnMatchersDone();
            }
        }

        public async void ReleaseMatchers()
        {
            var tasks = this.matchers.Select(m => new MatcherServiceClient(m.Url).ReleaseMatcher());
            await Task.WhenAll(tasks);
        }

        public async void StartMatchers(MatchMode mode)
        {
            // mark every matcher unfinished
            this.matchers.ForEach(m => m.Done = false);
            var tasks = this.matchers.Select(m => new MatcherServiceClient(m.Url).StartMatcher(mode));
            await Task.WhenAll(tasks);
        }

        public MatcherInfo GetByPartitionId(IId id)
        {
            return this.matchers.Find(m => m.Partition != null && m.Partition.Id.Equals(id));
        }

        private void OnMatchersReady()
        {
            var handler = this.MatchersReady;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnMatchersDone()
        {
            var handler = this.MatchersDone;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
