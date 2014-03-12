using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Interfaces
{
    public enum MatchMode
    {
        /// <summary>
        /// The matcher job should only look for the first occurence
        /// of the pattern.
        /// </summary>
        FirstOnly,

        /// <summary>
        /// The matcher job should look for all the possible matches.
        /// </summary>
        All
    }
}
