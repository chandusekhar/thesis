using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Core.Partition
{
    [Export(typeof(IEntityFactory))]
    [Export(typeof(IPartitionEntityFactory))]
    internal class PartitionEntityFactory : CoreEntityFactory, IPartitionEntityFactory
    {
        public ISuperNode CreateSuperNode()
        {
            return new SuperNode();
        }


        public ISuperNode CreateSuperNode(INode node)
        {
            ISuperNode sn = CreateSuperNode();
            sn.Nodes.Add(node);
            return sn;
        }
    }
}
