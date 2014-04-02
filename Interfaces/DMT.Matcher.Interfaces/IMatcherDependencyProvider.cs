using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Interfaces
{
    public interface IMatcherDependencyProvider
    {
        /// <summary>
        /// Gets a list of dependencies that are probably not in the probing path.
        /// </summary>
        string[] GetDependencies();
    }
}
