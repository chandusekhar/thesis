using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Entities
{
    public class CoreEntityFactory : IEntityFactory
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public virtual IId CreateId()
        {
            logger.Trace("Created new id.");
            return DMTId.NewId();
        }

        public virtual INode CreateNode()
        {
            logger.Trace("Created new node.");
            return new Node(this);
        }

        public virtual IEdge CreateEdge()
        {
            logger.Trace("Created new edge with null nodes.");
            return new Edge(null, null, EdgeDirection.Both, this);
        }

        public virtual IEdge CreateEdge(INode nodeA, INode nodeB, EdgeDirection direction)
        {
            logger.Trace("Created edge between {0} and {1} nodes for direction {2}.", nodeA, nodeB, direction);
            return new Edge(nodeA, nodeB, direction, this);
        }
    }
}
