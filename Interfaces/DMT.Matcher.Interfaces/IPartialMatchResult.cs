using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Interfaces
{
    /// <summary>
    /// Async result for partial queries.
    /// This interface is implemented in the framework.
    /// </summary>
    public interface IPartialMatchResult
    {
        IPattern MatchedPattern { get; }

        /// <summary>
        /// Wait for completion.
        /// </summary>
        void Wait();
    }
}
