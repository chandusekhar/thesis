using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Data.Interfaces
{
    /// <summary>
    /// Matcher specific extensions for nodes.
    /// 
    /// TODO: naming sucks, try to find a better one.
    /// </summary>
    public interface IMatchNode : INode
    {
        /// <summary>
        /// Sort edges in a way that helps the matching.
        /// Eg local first.
        /// </summary>
        void SortEdges();
    }
}
