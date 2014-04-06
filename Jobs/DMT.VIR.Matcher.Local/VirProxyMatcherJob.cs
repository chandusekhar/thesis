using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;
using DMT.Matcher.Interfaces;

namespace DMT.VIR.Matcher.Local
{
    public class VirProxyMatcherJob : VirMatcherJobBase
    {
        private IMatcherFramework framework;
        
        public override string Name
        {
            get { return "VIR case study matcher - proxy implementation"; }
        }

        public override void Initialize(IMatcherFramework framework)
        {
            base.Initialize(framework);
            this.framework = framework;
        }

        protected override T ConvertNode<T>(IMatchEdge incomingEdge, INode node)
        {
            if (incomingEdge.IsRemote)
            {
                return this.framework.GetNode(incomingEdge.RemotePartitionId, node.Id) as T;
            }

            return node as T;
        }
    }
}
