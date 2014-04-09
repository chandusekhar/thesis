using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Data.Interfaces;

namespace DMT.Matcher.Interfaces
{
    public interface IMatcherJobFactory
    {
        /// <summary>
        /// Creates a matcher job
        /// </summary>
        /// <returns></returns>
        IMatcherJob CreateMatcherJob();

        /// <summary>
        /// Creates an empty pattern object: it does not have any pattern nodes in it.
        /// </summary>
        /// <returns>an empty pattern</returns>
        IPattern CreateEmptyPattern();
    }
}
