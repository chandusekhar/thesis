using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Serialization
{
    class DeserializationContext : IContext
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<IId, INode> nodes;

        [Import]
        public IEntityFactory EntityFactory { get; private set; }

        public DeserializationContext()
            : this(new Dictionary<IId, INode>())
        {

        }

        internal DeserializationContext(IEntityFactory ef)
            : this()
        {
            this.EntityFactory = ef;
        }

        public DeserializationContext(IEnumerable<INode> nodes)
            : this(new Dictionary<IId, INode>())
        {
            foreach (var node in nodes)
            {
                this.AddNode(node);
            }
        }

        protected DeserializationContext(Dictionary<IId, INode> nodes)
        {
            this.nodes = nodes;
            CompositionService.Default.InjectOnce(this);
        }

        public virtual INode GetNode(IId id)
        {
            if (nodes.ContainsKey(id))
            {
                logger.Trace("Node was found with id {0}", id);
                return nodes[id];
            }

            // create a new node, and add it to the context for reuse
            logger.Trace("Creating remote node for {0} id.", id);
            IRemoteNode rn = this.EntityFactory.CreateRemoteNode(id);
            AddNode(rn);

            return rn;
        }

        public void AddNode(INode node)
        {
            if (nodes.ContainsKey(node.Id) && System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            nodes.Add(node.Id, node);
        }
    }
}
