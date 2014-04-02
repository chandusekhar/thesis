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

        /// <summary>
        /// Add nodes to the pattern.
        /// </summary>
        /// <param name="nodes"></param>
        void AddNodes(params IPatternNode[] nodes);

        /// <summary>
        /// Clears the matched nodes.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets a pattern node that matches the given name.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <returns>the pattern node</returns>
        /// <exception cref="InvalidOperationException">there is more than one element OR no element</exception>
        IPatternNode GetNodeByName(string name);

        /// <summary>
        /// Determines whether the node with the given name has been matched or not.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <returns>true only if the node is matched</returns>
        /// <exception cref="InvalidOperationException">there is more than one node with given name OR no node with the given name</exception>
        bool HasMatchedNodeFor(string name);
    }
}
