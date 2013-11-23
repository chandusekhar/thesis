using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Core.Partition
{
    internal class SuperNode : Node, ISuperNode
    {
        List<INode> nodes = new List<INode>();

        public SuperNode(IEntityFactory factory)
            : base(factory)
        {

        }

        public ICollection<INode> Nodes
        {
            get { return nodes; }
        }

        public int Size
        {
            // TODO: look into caching this value
            get { return CountChildren(); }
        }

        private int CountChildren()
        {
            int sum = 0;
            ISuperNode sn;

            foreach (var n in this.nodes)
            {
                sn = n as ISuperNode;

                if (sn != null)
                {
                    sum += sn.Size;
                }
                else
                {
                    ++sum;
                }
            }

            return sum;
        }
    }
}
