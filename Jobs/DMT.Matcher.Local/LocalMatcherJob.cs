using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Start(MatchMode mode)
        {
            Console.WriteLine("matcher started");
        }

        public IEnumerable<object> FindPartialMatch(Core.Interfaces.IId paritionId)
        {
            throw new NotImplementedException();
        }
    }
}
