using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    public class Model : IModel
    {
        private List<INode> nodesList;

        public ICollection<INode> Nodes
        {
            get { return nodesList; }
        }

        public Model()
            : this(new List<INode>())
        {

        }

        /// <summary>
        /// Instantiates a new Model object.
        /// </summary>
        /// <param name="nodes">Root nodes of the components. A new list will be instantiated with the specified element.</param>
        public Model(IEnumerable<INode> nodes)
        {
            this.nodesList = new List<INode>(nodes);
        }
    }
}
