using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Exceptions;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Serialization
{
    public class DeserializationContext : IContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<IId, INode> nodes;
        private IEntityFactory entityFactory;

        public IEntityFactory EntityFactory
        {
            get { return entityFactory; }
        }

        public DeserializationContext(IEntityFactory entityFactory)
            : this(entityFactory, new Dictionary<IId, INode>())
        {

        }

        public DeserializationContext(IEntityFactory entityFactory, Dictionary<IId, INode> nodes)
        {
            this.nodes = new Dictionary<IId, INode>(nodes);
            this.entityFactory = entityFactory;
        }

        public INode GetNode(IId id)
        {
            if (nodes.ContainsKey(id))
            {
                return nodes[id];
            }

            logger.Error("No node with id [{0}] was found in the context!");
            throw new NodeMissingException(id);
        }

        public void AddNode(INode node)
        {
            nodes.Add(node.Id, node);
        }
    }
}
