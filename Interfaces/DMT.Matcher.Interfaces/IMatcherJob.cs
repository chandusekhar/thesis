using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Interfaces
{
    /// <summary>
    /// The outer interface for a matcher job. This is the entry point for a
    /// matcher job. The implementation of this interface will be instantiated by the matcher framework.
    /// 
    /// Only one implementation should be provided per matcher job. The framework will throw an
    /// error if encounters more than one implementation in a matcher job.
    /// </summary>
    public interface IMatcherJob
    {
        /// <summary>
        /// Initialize the matcher job. This method is called after the construction of the IMatcherJob
        /// implementation.
        /// </summary>
        /// <param name="framework">a framework object to communicate with other matcher instances</param>
        void Initialize(IMatcherFramework framework);

        /// <summary>
        /// Start the matcher job.
        /// </summary>
        /// <param name="mode">mode of the search</param>
        void Start(MatchMode mode);

        /// <summary>
        /// Initiate a search for a partial pattern which already has discovered parts
        /// provided by an other matcher job.
        /// </summary>
        /// <param name="paritionId">The id of the partition to search in.</param>
        /// <returns>The matches if any.</returns>
        IEnumerable<object> FindPartialMatch(IId paritionId/*, partial match that has been already found in this partition*/);
    }
}
