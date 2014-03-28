using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Matcher.Data.Interfaces
{
    public interface IMatcherEntityFactory
    {
        /// <summary>
        /// Creates a new, empty pattern.
        /// </summary>
        /// <returns>a new pattern</returns>
        IPattern CreatePattern();

        /// <summary>
        /// Creates a new unmatched pattern node, that does not 
        /// belong to any pattern.
        /// </summary>
        /// <returns>a new pattern node</returns>
        IPatternNode CreatePatternNode();
    }
}
