using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Core.Serialization;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Data.Serialization
{
    class Context : DeserializationContext
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Context()
            : base()
        {

        }

        public override INode GetNode(IId id)
        {
            var node = base.GetNode(id);

            if (node != null)
            {
                return node;
            }

            // in this case the node must be replaced with a surrogate
            // because it is a remote node
            logger.Trace("Creating remote node for id: {0}", id);
            return new RemoteNode(id, this.EntityFactory);
        }
    }
}
