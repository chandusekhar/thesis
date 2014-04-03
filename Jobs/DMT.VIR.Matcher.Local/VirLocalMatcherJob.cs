using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Matcher.Local
{
    public class VirLocalMatcherJob : VirMatcherJobBase
    {
        public override string Name
        {
            get { return "VIR case study matcher - local only implementation"; }
        }

        protected override bool SkipEdge(IMatchEdge edge)
        {
            return edge.IsRemote;
        }
    }
}
