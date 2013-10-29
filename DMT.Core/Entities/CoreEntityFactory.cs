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
    [Export(typeof(IEntityFactory))]
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
            return new Node();
        }

        public virtual IEdge CreateEdge()
        {
            logger.Trace("Created new edge.");
            return new Edge();
        }

        public virtual IEdge CreateEdge(INode start, INode end)
        {
            logger.Trace("Created new edge.");
            return new Edge(start, end);
        }
    }
}
