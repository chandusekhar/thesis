using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Partition.TestApp
{
    internal interface IModelGenerator
    {
        /// <summary>
        /// Generate a model.
        /// </summary>
        /// <param name="n">Number of nodes in the model.</param>
        /// <param name="d">The average degree of a node.</param>
        /// <returns></returns>
        IModel Generate(int n, int d);
    }
}
