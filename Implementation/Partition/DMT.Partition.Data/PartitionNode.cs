using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Data
{
    internal class PartitionNode : Node, IPartitionNode
    {
        public IPartition Partition { get; set; }

        public PartitionNode(IEntityFactory factory) : base(factory)
        {

        }
    }
}
