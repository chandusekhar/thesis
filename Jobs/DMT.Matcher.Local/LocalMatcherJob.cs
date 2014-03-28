using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;

namespace DMT.Matcher.Local
{
    class LocalMatcherJob : IMatcherJob
    {
        public string Name
        {
            get { return "Local matcher job"; }
        }

        public event MatcherJobDoneEventHandler Done;

        public void Initialize(IMatcherFramework framework)
        {
            Console.WriteLine("Hello from local matcher!");
        }

        public void Start(IModel matcherModel, MatchMode mode)
        {
            Console.WriteLine("matcher started");
            OnJobDone(new IPattern[0]);
        }

        public IEnumerable<object> FindPartialMatch(Core.Interfaces.IId paritionId, IPattern pattern)
        {
            throw new NotSupportedException();
        }

        private void OnJobDone(IEnumerable<IPattern> matches)
        {
            var handler = this.Done;
            if (handler != null)
            {
                handler(this, new MatcherJobDoneEventArgs(matches));
            }
        }
    }
}
