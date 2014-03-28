using System;
using System.Collections.Generic;
using System.Linq;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Interfaces
{
    public delegate void MatcherJobDoneEventHandler(object sender, MatcherJobDoneEventArgs e);

    public class MatcherJobDoneEventArgs : EventArgs
    {
        /// <summary>
        /// Determines whether there are any matches or not.
        /// </summary>
        public bool HasMatches
        {
            get { return this.Matches.Any(); }
        }

        /// <summary>
        /// Gets the matched patterns.
        /// </summary>
        public IEnumerable<IPattern> Matches { get; private set; }

        public MatcherJobDoneEventArgs(IEnumerable<IPattern> matches)
        {
            this.Matches = matches;
        }
    }
}