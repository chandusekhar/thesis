using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Core.Interfaces
{
    public interface IModelFactory
    {
        /// <summary>
        /// Creates a new empty object that implements IModel.
        /// </summary>
        /// <returns></returns>
        IModel CreateEmpty();

        /// <summary>
        /// Create a new object that implements IModel and sets the given nodes as
        /// the nodes of the model.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        IModel Create(IEnumerable<INode> nodes);
    }
}
