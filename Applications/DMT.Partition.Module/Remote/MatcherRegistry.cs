﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Partition.Module.Exceptions;
using DMT.Module.Common.Service;

namespace DMT.Partition.Module.Remote
{
    class MatcherRegistry
    {
        private List<MatcherInfo> matchers;

        public event EventHandler MatchersReady;

        public MatcherRegistry()
        {
            this.matchers = new List<MatcherInfo>();
        }

        public void AddMatcher(MatcherInfo matcherinfo)
        {
            this.matchers.Add(matcherinfo);
        }

        public bool RemoveMatcher(Guid id)
        {
            return this.matchers.RemoveAll(m => m.Id == id) > 0;
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
            this.GetById(id).MarkReady();

            if (matchers.All(m => m.Ready))
            {
                OnMatchersReady();
            }
        }

        public async void ReleaseMatchers()
        {
            var tasks = this.matchers.Select(m => new MatcherServiceClient(m.Url).ReleaseMatcher());
            await Task.WhenAll(tasks);
        }

        private void OnMatchersReady()
        {
            var handler = this.MatchersReady;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
