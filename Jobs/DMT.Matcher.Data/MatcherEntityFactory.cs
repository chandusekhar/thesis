using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;

namespace DMT.Matcher.Data
{
    [Export(typeof(IEntityFactory))]
    internal class MatcherEntityFactory : CoreEntityFactory
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import]
        private IEntityFactory baseEntityFactory;

        public MatcherEntityFactory()
        {
            this.baseEntityFactory = this;
        }

        public override IEdge CreateEdge(INode nodeA, INode nodeB, EdgeDirection direction)
        {
            logger.Trace("Created MatchEdge");
            return new MatchEdge(nodeA, nodeB, direction, this.baseEntityFactory);
        }
    }
}
