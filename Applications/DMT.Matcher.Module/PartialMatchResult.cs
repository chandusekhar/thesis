using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;

namespace DMT.Matcher.Module
{
    class PartialMatchResult : IPartialMatchResult
    {
        private AutoResetEvent signal;

        public Guid Id { get; private set; }

        public IPattern MatchedPattern { get; set; }

        public PartialMatchResult()
        {
            this.Id = Guid.NewGuid();
            this.signal = new AutoResetEvent(false);
        }

        public void Release()
        {
            this.signal.Set();
        }

        public void Wait()
        {
            this.signal.WaitOne();
        }
    }
}
