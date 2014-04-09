using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Matcher.Local.Pattern;

namespace DMT.VIR.Matcher.Local
{
    internal class VirMatcherJobFactory : IMatcherJobFactory
    {
        private const string MatcherStrategyKey = "strategy";
        private const string LocalStartegy = "local";
        private const string ProxyStrategy = "proxy";

        public IMatcherJob CreateMatcherJob()
        {
            string strategy = Configuration.Current.GetOption(MatcherStrategyKey);

            switch (strategy)
            {
                case LocalStartegy:
                    return new VirLocalMatcherJob();
                case ProxyStrategy:
                    return new VirProxyMatcherJob();
                default:
                    throw new NotSupportedException("Not supported strategy: " + strategy);
            }
        }

        public IPattern CreateEmptyPattern()
        {
            return new Pattern.Pattern();
        }
    }
}
