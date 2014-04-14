using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;
using DMT.VIR.Matcher.Local.Partial;
using DMT.VIR.Matcher.Local.Patterns;

namespace DMT.VIR.Matcher.Local
{
    internal class VirMatcherJobFactory : IMatcherJobFactory
    {
        private const string MatcherStrategyKey = "strategy";
        private const string LocalStartegy = "local";
        private const string ProxyStrategy = "proxy";
        private const string PartialStrategy = "partial";

        public IMatcherJob CreateMatcherJob()
        {
            string strategy = Configuration.Current.GetOption(MatcherStrategyKey);

            switch (strategy)
            {
                case LocalStartegy:
                    return new VirLocalMatcherJob();
                case ProxyStrategy:
                    return new VirProxyMatcherJob();
                case PartialStrategy:
                    return new VirPartialMatcherJob();
                default:
                    throw new NotSupportedException("Not supported strategy: " + strategy);
            }
        }

        public IPattern CreateEmptyPattern()
        {
            return new Pattern();
        }
    }
}
