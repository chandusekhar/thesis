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
    [Export(typeof(IPartitionEntityFactory))]
    [Export(typeof(IEntityFactory))]
    internal class PartitionEntityFactory : CoreEntityFactory, IPartitionEntityFactory, IEntityFactory
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IEntityFactory baseEntityFactory;

        public PartitionEntityFactory() { }

        public PartitionEntityFactory(IEntityFactory entityFactory)
        {
            this.baseEntityFactory = entityFactory;
        }

        public ISuperNode CreateSuperNode()
        {
            return new SuperNode(this.baseEntityFactory);
        }

        public ISuperNode CreateSuperNode(INode node)
        {
            ISuperNode sn = CreateSuperNode();
            sn.Nodes.Add(node);
            return sn;
        }

        public IPartition CreatePartition()
        {
            return new Partition(baseEntityFactory);
        }

        public override INode CreateNode()
        {
            var node = new PartitionNode(this.baseEntityFactory);
            logger.Trace("Created new node: {0}", node);
            return node;
        }
    }
}
