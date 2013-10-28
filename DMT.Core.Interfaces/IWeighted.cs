using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    /// <summary>
    /// Weighted means it has a weight applied to it.
    /// </summary>
    public interface IWeighted
    {
        /// <summary>
        /// Gets the weight of the object.
        /// </summary>
        /// <returns></returns>
        double GetWeight();
    }
}
