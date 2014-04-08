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
    /// The outer interface for a matcher job. This is the entry point for a
    /// matcher job. The implementation of this interface will be instantiated by the matcher framework.
    /// 
    /// Only one implementation should be provided per matcher job. The framework will throw an
    /// error if encounters more than one implementation in a matcher job.
    /// </summary>
    public interface IMatcherJob
    {
        /// <summary>
        /// Name of the matcher job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determines whether the job is currently running or not.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Fires when the search is over.
        /// </summary>
        event MatcherJobDoneEventHandler Done;

        /// <summary>
        /// Initialize the matcher job. This method is called after the construction of the IMatcherJob
        /// implementation.
        /// </summary>
        /// <param name="framework">a framework object to communicate with other matcher instances</param>
        void Initialize(IMatcherFramework framework);

        /// <summary>
        /// Start the matcher job asyncronously.
        /// </summary>
        /// <param name="matcherModel">The model that contains the nodes for the matcher instance.</param>
        /// <param name="mode">mode of the search</param>
        void StartAsync(IModel matcherModel, MatchMode mode);

        /// <summary>
        /// Cancel a running job.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Initiate a search for a partial pattern which already has discovered parts
        /// provided by an other matcher job.
        /// </summary>
        /// <param name="paritionId">The id of the partition to search in.</param>
        /// <param name="pattern">The partially matched pattern.</param>
        /// <returns>The matches if any.</returns>
        IEnumerable<object> FindPartialMatch(IId paritionId, IPattern pattern);
    }
}
