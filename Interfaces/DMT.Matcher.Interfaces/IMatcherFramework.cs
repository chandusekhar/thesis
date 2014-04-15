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
        /// <returns>async result</returns>
        IPartialMatchResult BeginFindPartialMatch(IId partitionId, IPattern pattern);

        /// <summary>
        /// The search for a parial match ended. This method sends back the result to the
        /// initiating matcher module.
        /// </summary>
        /// <param name="sessionId">the id of the session to end</param>
        /// <param name="matchedPatterns">the patterns that were found. empty if no pattern were found</param>
        void EndFindPartialMatch(Guid sessionId, IPattern matchedPattern);

        /// <summary>
        /// Gets a (remote) node from a given partition.
        /// </summary>
        /// <param name="partitionId">id of the partition</param>
        /// <param name="nodeId">id of the node</param>
        /// <returns>the node itself</returns>
        INode GetNode(IId partitionId, IId nodeId);
    }
}
