using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Matcher.Interfaces;

namespace DMT.VIR.Matcher.Local
{
    internal class VirMatcherJobFactory : IMatcherJobFactory
    {
        private const string MatcherStrategyKey = "strategy";
        private const string LocalStartegy = "local";

        public IMatcherJob CreateMatcherJob()
        {
            string strategy = Configuration.Current.GetOption(MatcherStrategyKey);

            if (strategy == LocalStartegy)
            {
                return new VirMatcherJob();
            }

            throw new InvalidOperationException("Not supported strategy: " + strategy);
        }
    }
}
