using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;

namespace DMT.Matcher.Module
{
    /// <summary>
    /// MatcherFrameworkLink is the implementation of the link between the matcher job
    /// and the matcher framework. This is a one way interaction: matcher job -> framework
    /// </summary>
    class MatcherFrameworkLink : IMatcherFramework
    {
        public void BeginFindPartialMatch(IId partitionId, IPattern pattern)
        {
            throw new NotImplementedException();
        }

        public event FoundPartialMatchEventHandler FoundPartialMatch;
    }
}
