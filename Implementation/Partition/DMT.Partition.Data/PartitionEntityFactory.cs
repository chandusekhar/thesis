using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Partition.Interfaces;

namespace DMT.Partition.Data
{
    [Export(typeof(IPartitionEntityFactory))]
    [Export(typeof(IEntityFactory))]
    internal class PartitionEntityFactory : CoreEntityFactory, IPartitionEntityFactory, IEntityFactory
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IEntityFactory baseEntityFactory;

        public PartitionEntityFactory()
        {
            this.baseEntityFactory = this;
        }

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

        public override IEdge CreateEdge()
        {
            var edge = new PartitionEdge(this.baseEntityFactory);
            logger.Trace("Created new edge: {0}", edge);
            return edge;
        }

        public override IEdge CreateEdge(INode nodeA, INode nodeB, EdgeDirection direction)
        {
            var edge = new PartitionEdge(nodeA, nodeB, direction, this.baseEntityFactory);
            logger.Trace("Created new edge: {0}", edge);
            return edge;
        }
    }
}
