using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces.Serialization;
using NLog;

namespace DMT.Core.Serialization
{
    public class CoreEntityFactory : IEntityFactory
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Interfaces.IId CreateId()
        {
            logger.Trace("Created new id.");
            return Id.NewId();
        }

        public Interfaces.INode CreateNode()
        {
            logger.Trace("Created new node.");
            return new Node();
        }

        public Interfaces.IEdge CreateEdge()
        {
            logger.Trace("Created new edge.");
            return new Edge();
        }

        public Interfaces.IEdge CreateEdge(Interfaces.INode start, Interfaces.INode end)
        {
            logger.Trace("Created new edge.");
            return new Edge(start, end);
        }
    }
}
