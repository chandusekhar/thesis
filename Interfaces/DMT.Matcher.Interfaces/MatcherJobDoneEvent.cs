using System;
using System.Collections.Generic;

namespace DMT.Matcher.Interfaces
{
    public delegate void MatcherJobDoneEventHandler(object sender, MatcherJobDoneEventArgs e);

    public class MatcherJobDoneEventArgs : EventArgs
    {
        public IEnumerable<object> Matches { get; private set; }

        public MatcherJobDoneEventArgs(IEnumerable<object> matches)
        {
            this.Matches = matches;
        }
    }
}