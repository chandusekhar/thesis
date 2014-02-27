using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public DeserializationContext(IEntityFactory ef, IEnumerable<INode> nodes)
            : this(ef, new Dictionary<IId, INode>())
        {
            foreach (var node in nodes)
            {
                this.AddNode(node);
            }
        }

        protected DeserializationContext(IEntityFactory entityFactory, Dictionary<IId, INode> nodes)
        {
            this.nodes = nodes;
            this.entityFactory = entityFactory;
        }

        public INode GetNode(IId id)
        {
            if (nodes.ContainsKey(id))
            {
                logger.Trace("Node was found with id {0}", id);
                return nodes[id];
            }

            logger.Warn("No node with id [{0}] was found in the context!");
            return null;
        }

        public void AddNode(INode node)
        {
            nodes.Add(node.Id, node);
        }
    }
}
