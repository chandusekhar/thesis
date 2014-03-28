using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Interfaces
{
    /// <summary>
    /// IMatcherFramework is the link between a matcher job and the framework. This
    /// interface provides methods to commumincate with other matcher instances in the grid.
    /// 
    /// This interface should not be implemneted in the matcher job. 
    /// </summary>
    public interface IMatcherFramework
    {
        /// <summary>
        /// Ask a relevant matcher instance to find a partial match. This method only
        /// initiates the search.
        /// </summary>
        /// <param name="partitionId">id of the partition to search in</param>
        /// <param name="pattern">the pattern that contains the already matcehd nodes</param>
        void BeginFindPartialMatch(IId partitionId, IPattern pattern);

        /// <summary>
        /// This event is fired when the search for a partial match is done.
        /// </summary>
        event FoundPartialMatchEventHandler FoundPartialMatch;

    }
}
