using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Interfaces
{
    public interface IMatcherJobFactory
    {
        /// <summary>
        /// Creates a matcher job
        /// </summary>
        /// <returns></returns>
        IMatcherJob CreateMatcherJob();
    }
}
