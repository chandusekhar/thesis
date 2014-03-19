using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Data.Interfaces
{
    /// <summary>
    /// Edge interface used in the matching module
    /// </summary>
    public interface IMatchEdge : IEdge
    {
        /// <summary>
        /// Determines whether the edge leads to a remote node.
        /// </summary>
        bool IsRemote { get; }

        /// <summary>
        /// Gets the id of the remote partition (if any) or null.
        /// </summary>
        IId RemotePartitionId { get; set; }
    }
}
