using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    public enum EdgeDirection
    {
        /// <summary>
        /// The edge is unidirectional and it goes from endpoint A to endpoint B
        /// </summary>
        AToB,

        /// <summary>
        /// The edge is unidirectional and it goes from endpoint B to endpoint A
        /// </summary>
        BToA,

        /// <summary>
        /// The edge is bidirectional, so it goes from A to B and B to A.
        /// </summary>
        Both
    }
}
