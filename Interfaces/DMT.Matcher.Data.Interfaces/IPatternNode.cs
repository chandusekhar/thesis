using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Data.Interfaces
{
    /// <summary>
    /// A pettern node is a regular node, but it is not part of the model. Pattern nodes
    /// are there to mark how the pattern looks like. And also used for storing already
    /// matched nodes.
    ///
    /// When requesting an other matcher instance to find a partial match, pattern nodes will
    /// be sent over the network with their matched nodes.
    /// </summary>
    public interface IPatternNode : INode
    {
        /// <summary>
        /// Gets the name of the pattern node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the node, that has been matched for this
        /// particular pattern node.
        /// </summary>
        INode MatchedNode { get; set; }

        /// <summary>
        /// Determines whether the pattern node has been matched to a model node or not.
        /// </summary>
        bool IsMatched { get; }
    }
}
