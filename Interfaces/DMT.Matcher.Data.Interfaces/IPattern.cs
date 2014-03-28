using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Data.Interfaces
{
    /// <summary>
    /// This is basically a wrapper around the pattern that we are looking for. It is needed by the
    /// framework, so it can send partial matches to other matcher instances.
    /// </summary>
    public interface IPattern
    {
        /// <summary>
        /// Gets or sets the nodes for the pattern.
        /// </summary>
        IEnumerable<IPatternNode> PatternNodes { get; set; }

        /// <summary>
        /// Determines whether the pattern is fully matched. Basically when
        /// every pattern node is matched the whole pattern is matched.
        /// </summary>
        bool IsFullyMatched { get; }

        /// <summary>
        /// Gets all the matched nodes.
        /// </summary>
        /// <returns></returns>
        IEnumerable<INode> GetMatchedNodes();
    }
}
