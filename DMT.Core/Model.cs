using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    internal class Model : IModel
    {
        private List<INode> componentRootList;

        public ICollection<INode> ComponentRoots
        {
            get { return componentRootList; }
        }

        /// <summary>
        /// Instantiates a new Model object.
        /// </summary>
        /// <param name="nodes">Root nodes of the components. A new list will be instantiated with the specified element.</param>
        public Model(IEnumerable<INode> nodes)
        {
            this.componentRootList = new List<INode>(nodes);
        }
    }
}
